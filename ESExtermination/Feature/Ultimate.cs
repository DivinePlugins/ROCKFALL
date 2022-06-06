using System.Linq;
using Divine.Entity;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Game;
using Divine.Helpers;
using Divine.Menu.Items;
using Divine.Update;
using ESExtermination.Abilities.Spells;
using ESExtermination.Extensions;

namespace ESExtermination.Feature
{
    internal class Ultimate : FeatureBase
    {
        private Menu ultimateFeaturesMenu;
        private MenuSlider autoUseMagnetize;
        private MenuSwitcher extendMagnetizeTime;
        private MenuSlider minStones;

        private Stone stone;
        private Magnetize magnetize;
        private Smash smash;
        private Grip grip;

        private Sleeper placeSleeper = new Sleeper();

        public Ultimate(Context context)
            : base(context)
        {
            stone = context.Combo.Stone;
            magnetize = context.Combo.Magnetize;
            smash = context.Combo.Smash;
            grip = context.Combo.Grip;

            ultimateFeaturesMenu = rootMenu.CreateMenu("Ultimate")
                .SetTooltip("Ultimate features")
                .SetAbilityImage(AbilityId.earth_spirit_magnetize);

            autoUseMagnetize = ultimateFeaturesMenu.CreateSlider("Min. enemies for auto ultimate", 2, 0, 5)
                                                   .SetTooltip("Set 0 for disable this feature");

            extendMagnetizeTime = ultimateFeaturesMenu.CreateSwitcher("Extend magnetize debuff")
                                                                  .SetTooltip("Place stone for update magnetize");

            minStones = ultimateFeaturesMenu.CreateSlider("Min stones for extend magnetize debuff", 2, 0, 5)
                                            .SetTooltip("If stones charges < this value then no stones will be placed");

            minStones.IsHidden = true;
        }

        public override void Start()
        {
            autoUseMagnetize.ValueChanged += AutoUseMagnetize_ValueChanged;
            extendMagnetizeTime.ValueChanged += AutoPlaceStoneForUpdateUltimate_ValueChanged;
        }
        public override void Dispose()
        {
            extendMagnetizeTime.ValueChanged -= AutoPlaceStoneForUpdateUltimate_ValueChanged;
            autoUseMagnetize.ValueChanged -= AutoUseMagnetize_ValueChanged;
            UpdateManager.DestroyIngameUpdate(ExtendUpdater);
            UpdateManager.DestroyIngameUpdate(AutoUlt);

        }

        private void AutoPlaceStoneForUpdateUltimate_ValueChanged(MenuSwitcher switcher, Divine.Menu.EventArgs.SwitcherEventArgs e)
        {
            if (e.Value)
            {
                minStones.IsHidden = false;
                UpdateManager.CreateIngameUpdate(200, ExtendUpdater);
            }
            else
            {
                minStones.IsHidden = true;
                UpdateManager.DestroyIngameUpdate(ExtendUpdater);
            }
        }

        private void AutoUseMagnetize_ValueChanged(MenuSlider slider, Divine.Menu.EventArgs.SliderEventArgs e)
        {
            if (e.NewValue > 0)
            {
                UpdateManager.CreateIngameUpdate(200, AutoUlt);
            }
            else
            {
                UpdateManager.DestroyIngameUpdate(AutoUlt);
            }
        }

        private void AutoUlt()
        {
            if (!magnetize.CanBeCasted())
            {
                return;
            }

            var enemyesAround = EntityManager.GetEntities<Hero>().Where(x => x.Distance2D(localHero) < magnetize.Radius
                                                                            && x.IsEnemy(localHero)
                                                                            && !x.IsIllusion
                                                                            && x.IsVisible
                                                                            && x.IsAlive
                                                                            && !x.IsMagicImmune());

            if (enemyesAround.Count() >= autoUseMagnetize.Value)
            {
                magnetize.Base.Cast();
            }
        }

        private void ExtendUpdater()
        {
            if (!stone.CanBeCasted()
                || minStones > stone.Base.CurrentCharges
                || placeSleeper.Sleeping)
            {
                return;
            }

            if (context.ComboKey.Value && smash.Base.Cooldown < 5 && grip.Base.Cooldown < 5)
            {
                return;
            }

            var magnetizedEnemies = EntityManager.GetEntities<Hero>().Where(x => x.Distance2D(localHero) < stone.Range
                                                                            && x.IsVisible
                                                                            && x.IsEnemy(localHero)
                                                                            && x.IsMagnetized());

            if (!magnetizedEnemies.Any())
            {
                return;
            }

            var gametime = GameManager.GameTime;

            foreach (var enemy in magnetizedEnemies)
            {
                if (enemy.GetModifierByName(magnetize.DebuffName).DieTime - gametime < 1.75f) //todo
                {
                    stone.Base.Cast(enemy.Position);
                    placeSleeper.Sleep(500);
                    return;
                }
            }
        }


    }
}
