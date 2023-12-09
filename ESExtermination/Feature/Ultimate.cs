using System.Linq;

using Divine.Entity;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Game;
using Divine.Helpers;
using Divine.Menu;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Update;

using ESExtermination.Abilities.Spells;
using ESExtermination.Extensions;

namespace ESExtermination.Feature
{
    internal class Ultimate : FeatureBase
    {
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
        }

        public override void Start()
        {
            context.autoUseMagnetize.ValueChanged += AutoUseMagnetize_ValueChanged;
            context.extendMagnetizeTime.ValueChanged += AutoPlaceStoneForUpdateUltimate_ValueChanged;
        }
        public override void Dispose()
        {
            context.extendMagnetizeTime.ValueChanged -= AutoPlaceStoneForUpdateUltimate_ValueChanged;
            context.autoUseMagnetize.ValueChanged -= AutoUseMagnetize_ValueChanged;
            UpdateManager.DestroyIngameUpdate(ExtendUpdater);
            UpdateManager.DestroyIngameUpdate(AutoUlt);

        }

        private void AutoPlaceStoneForUpdateUltimate_ValueChanged(MenuSwitcher switcher, SwitcherChangedEventArgs e)
        {
            if (e.Value)
            {
                context.minStones.Show();
                UpdateManager.CreateIngameUpdate(200, ExtendUpdater);
            }
            else
            {
                context.minStones.Hide();
                UpdateManager.DestroyIngameUpdate(ExtendUpdater);
            }
        }

        private void AutoUseMagnetize_ValueChanged(MenuSlider slider, SliderChangedEventArgs e)
        {
            if (e.Value > 0)
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

            if (enemyesAround.Count() >= context.autoUseMagnetize.Value)
            {
                magnetize.Base.Cast();
            }
        }

        private void ExtendUpdater()
        {
            if (!stone.CanBeCasted()
                || context.minStones > stone.Base.CurrentCharges
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
