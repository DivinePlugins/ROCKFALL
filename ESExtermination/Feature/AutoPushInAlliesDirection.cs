namespace ESExtermination.Feature;

using System.Linq;

using Divine.Entity;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Helpers;
using Divine.Menu;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Renderer;
using Divine.Update;

using ESExtermination.Abilities.Spells;

class AutoPushInAlliesDirection : FeatureBase
{
    private Menu featureMenu;
    private MenuSelector modeSelector;
    private MenuSlider minAllyHpPercent;

    private Smash smash;
    private Enchant enchant;

    private Sleeper sleeper = new();

    public AutoPushInAlliesDirection(Context context)
            : base(context)
    {
        smash = context.Combo.Smash;
        enchant = context.Combo.Enchant;

        featureMenu = rootMenu.AddMenu("Auto push under allies").SetImage(AbilityId.earth_spirit_boulder_smash);
        modeSelector = featureMenu.AddSelector("Mode", ["Off", "On", "Also with agha"]).SetTooltip("Auto use smash for push in allies");
        minAllyHpPercent = featureMenu.AddSlider("Min. ally hp %", 20, 0, 80);
    }

    public override void Start()
    {
        modeSelector.ValueChanged += FeatureSelector_ValueChanged;
    }

    public override void Dispose()
    {
        UpdateManager.DestroyIngameUpdate(IngameUpdate);
        modeSelector.ValueChanged -= FeatureSelector_ValueChanged;
    }

    private void FeatureSelector_ValueChanged(MenuSelector<string> sender, SelectorChangedEventArgs<string> e)
    {
        if (e.Value != "Off")
        {
            UpdateManager.CreateIngameUpdate(200, IngameUpdate);
        }
        else
        {
            UpdateManager.DestroyIngameUpdate(IngameUpdate);
        }
    }

    private void IngameUpdate()
    {
        if (!smash.CanBeCasted() || sleeper.IsSleeping)
        {
            return;
        }

        var target = EntityManager.GetEntities<Hero>().FirstOrDefault(x => x.Distance2D(localHero) < 300
                                                            && x.IsAlive
                                                            && (!x.IsIllusion || x.HasModifier("modifier_morphling_replicate"))
                                                            && x.IsEnemy(context.LocalHero)
                                                            && x.IsVisible);

        if (target == null)
        {
            return;
        }

        var allies = EntityManager.GetEntities<Hero>().Where(x => x.IsAlly(localHero)
                                                                    && x.IsAlive
                                                                    && !x.IsIllusion);

        if (modeSelector.Value == "Also with agha" && enchant.CanBeCasted())
        {
            var alliesAround = allies.Where(x => x.Distance2D(target) < 1000);

            foreach (var ally in allies.Where(x => x.Distance2D(localHero) < 3000
                                                            && x.Distance2D(localHero) > 1000))
            {
                if (ally == null)
                {
                    break;
                }

                var destinationWithAghanim = target.Position.Extend(ally.Position, 2000);
                var alliesArroundDestination = allies.Where(x => x.Distance2D(destinationWithAghanim) < 1000);

                if (alliesArroundDestination.Count() < alliesAround.Count())
                {
                    continue;
                }

                if (alliesArroundDestination.All(x => x.HealthPercent() > (float)minAllyHpPercent.Value / 100))
                {
                    enchant.Base.Cast(target);
                    sleeper.Sleep(500);

                    UpdateManager.BeginInvoke(500, () =>
                    {
                        smash.UseAbility(localHero.Position.Extend(ally.Position, 400));
                    });
                    return;
                }
            }
        }

        var range = smash.PushDistance;

        var destination = target.Position.Extend(localHero.Position, -range);

        var alliesNear = allies.Where(x => x.Distance2D(localHero) < range / 1.5f);
        var alliesNearDestination = allies.Where(x => x.Position.Distance2D(destination) < range);

        if (alliesNearDestination.Any(x => x.HealthPercent() < (float)minAllyHpPercent.Value / 100)
            || alliesNear.Count() >= alliesNearDestination.Count())
        {
            return;
        }

        smash.Base.Cast(target);
        sleeper.Sleep(500);
    }
}
