using System.Linq;
using Divine.Entity;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Helpers;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Update;
using ESExtermination.Abilities.Spells;

namespace ESExtermination.Feature
{
    class AutoPushInAlliesDirection : FeatureBase
    {
        private Menu featureMenu;
        private MenuSelector modeSelector;
        private MenuSlider minAllyHpPercent;

        private Smash smash;
        private Enchant enchant;

        private Sleeper sleeper = new Sleeper();

        public AutoPushInAlliesDirection(Context context)
                : base(context)
        {
            smash = context.Combo.Smash;
            enchant = context.Combo.Enchant;

            featureMenu = rootMenu.CreateMenu("Auto push under allies")
                .SetAbilityImage(AbilityId.earth_spirit_boulder_smash);

            modeSelector = featureMenu.CreateSelector("Mode",
                new string[3] { "Off", "On", "Also with agha" })
                    .SetTooltip("Auto use smash for push in allies");

            minAllyHpPercent = featureMenu.CreateSlider("Min. ally hp %", 20, 0, 80);
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

        private void FeatureSelector_ValueChanged(MenuSelector selector, SelectorEventArgs e)
        {
            if (e.NewValue != "Off")
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
            if (!smash.CanBeCasted() || sleeper.Sleeping)
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
}
