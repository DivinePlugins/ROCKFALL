namespace ESExtermination.Feature;

using System.Linq;

using Divine.Entity;
using Divine.Entity.Entities.Units.Buildings;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Update;

using ESExtermination.Abilities.Spells;
using ESExtermination.Extensions;

internal class AutoPushInTowerDirection : FeatureBase
{
    public bool CanPushInTower { get; private set; } = false;

    private Smash smash;
    private Enchant enchant;

    public AutoPushInTowerDirection(Context context)
        : base(context)
    {
        smash = context.Combo.Smash;
        enchant = context.Combo.Enchant;
    }

    public override void Start()
    {
        context.underTowersMode.ValueChanged += FeatureSelector_ValueChanged;
        context.ComboKey.ValueChanged += ComboKey_ValueChanged;
    }

    public override void Dispose()
    {
        UpdateManager.DestroyIngameUpdate(IngameUpdate);
        UpdateManager.IngameUpdate -= IngameUpdate;
        context.underTowersMode.ValueChanged -= FeatureSelector_ValueChanged;
        context.ComboKey.ValueChanged -= ComboKey_ValueChanged;
    }

    private void ComboKey_ValueChanged(MenuHoldKey holdKey, HoldKeyChangedEventArgs e)
    {
        if (context.underTowersMode.Value == "Off")
        {
            return;
        }

        if (e.Value)
        {
            UpdateManager.DestroyIngameUpdate(IngameUpdate);
            UpdateManager.IngameUpdate += IngameUpdate;
        }
        else
        {
            UpdateManager.IngameUpdate -= IngameUpdate;
            UpdateManager.CreateIngameUpdate(200, IngameUpdate);
        }
    }

    private void FeatureSelector_ValueChanged(MenuSelector sender, SelectorChangedEventArgs e)
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

    private Tower GetGoodTowerForAgha()
    {
        return EntityManager.GetEntities<Tower>()
                .Where(x => x.Distance2D(localHero) <= 2500 + context.underTowersAadditionalRange / 2
                        && x.Distance2D(localHero) >= 1300 - context.underTowersAadditionalRange / 2
                        && x.IsAlly(localHero)
                        && x.IsAlive)
                .OrderByDescending(x => x.Distance2D(localHero))
                .FirstOrDefault();
    }

    private void IngameUpdate()
    {
        if (!smash.CanBeCasted())
        {
            CanPushInTower = false;
            return;
        }

        var nearestEnemyes = EntityManager.GetEntities<Hero>()
            .Where(x => x.Distance2D(localHero) < 350
                    && x.IsEnemy(localHero)
                    && !x.IsMagicImmune()
                    && !x.IsIllusion
                    && x.IsVisible
                    && x.IsAlive);

        if (!nearestEnemyes.Any())
        {
            return;
        }

        if (context.underTowersMode.Value == "Also with agha")
        {
            var tower = GetGoodTowerForAgha();

            if (tower != null)
            {
                if (enchant.CanBeCasted())
                {
                    CanPushInTower = true;
                    var firstEnemy = nearestEnemyes.First();

                    if (enchant.UseAbility(firstEnemy))
                    {
                        return;
                    }
                }

                if (nearestEnemyes.Any(x => x.IsEnchanted()))
                {
                    CanPushInTower = true;
                    smash.Base.Cast(localHero.Position.Extend(tower.Position, 400));
                    return;
                }

            }
        }

        var target = nearestEnemyes.First();

        var nearestTower = EntityManager.GetEntities<Tower>()
            .FirstOrDefault(x => x.Distance2D(target) < 700 + smash.PushDistance + context.underTowersAadditionalRange.Value //700 is tower attack range
                            && x.IsAlly(localHero)
                            && x.IsAlive);


        if (nearestTower == null)
        {
            CanPushInTower = false;
            return;
        }
        else if (localHero.Distance2D(target.Position.Extend(nearestTower.Position, -150)) < 250)
        {
            CanPushInTower = true;
        }

        var pushTargets = nearestEnemyes.Where(x => x.Position.Extend(localHero.Position, -smash.PushDistance).Distance2D(nearestTower.Position) < 700 + context.underTowersAadditionalRange.Value); //700 is tower attack range

        var pushTarget = pushTargets.FirstOrDefault(x => x.Position.Extend(localHero.Position, -smash.PushDistance).Distance2D(nearestTower.Position) < x.Distance2D(nearestTower));

        if (pushTarget != null)
        {
            smash.Base.Cast(pushTarget);
        }
    }
}
