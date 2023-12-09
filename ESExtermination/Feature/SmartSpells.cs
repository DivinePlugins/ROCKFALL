using System.Linq;

using Divine.Entity;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Game;
using Divine.Input;
using Divine.Order;
using Divine.Order.Orders.Components;
using Divine.Update;

using ESExtermination.Abilities.Spells;
using ESExtermination.Extensions;

namespace ESExtermination.Feature
{
    internal class SmartSpells : FeatureBase
    {
        private Stone stone;
        private Smash smash;
        private Roll roll;
        private Grip grip;

        private string stoneName;

        private bool isCtrlPressed = false;
        private bool isAltPressed = false;

        private bool IsIgnoringInputs => isAltPressed || isCtrlPressed;

        public SmartSpells(Context context)
            : base(context)
        {
            stone = context.Combo.Stone;
            smash = context.Combo.Smash;
            roll = context.Combo.Roll;
            grip = context.Combo.Grip;

            stoneName = StoneExtensions.StoneName;
        }

        public override void Start()
        {
            Divine.Input.InputManager.KeyDown += InputManager_KeyDown;
            Divine.Input.InputManager.KeyUp += InputManager_KeyUp;
            OrderManager.OrderAdding += OrderManager_OrderAdding;
        }

        public override void Dispose()
        {

            OrderManager.OrderAdding -= OrderManager_OrderAdding;
            Divine.Input.InputManager.KeyDown -= InputManager_KeyDown;
            Divine.Input.InputManager.KeyUp -= InputManager_KeyUp;
        }

        private void InputManager_KeyUp(Divine.Input.EventArgs.KeyEventArgs e)
        {
            if (e.Key == Key.LeftAlt)
            {
                isAltPressed = false;
            }

            if (e.Key == Key.LeftCtrl)
            {
                isCtrlPressed = false;
            }
        }

        private void InputManager_KeyDown(Divine.Input.EventArgs.KeyEventArgs e)
        {
            if (e.Key == Key.LeftAlt)
            {
                isAltPressed = true;
            }

            if (e.Key == Key.LeftCtrl)
            {
                isCtrlPressed = true;
            }
        }

        private void OrderManager_OrderAdding(Divine.Order.EventArgs.OrderAddingEventArgs e)
        {
            if (e.IsCustom
                || IsIgnoringInputs
                || e.Order.Type == OrderType.UpgradeSpell)
            {
                return;
            }

            var localHeroPos = localHero.Position;

            if (context.smartSmash.Value
                && e.Order.Ability == smash.Base
                && e.Order.Type != OrderType.CastTarget)
            {
                if (StoneExtensions.FirstUnitInRange(localHeroPos, stoneName, 200) != null
                    || EntityManager.GetEntities<Hero>().Any(x => x.Distance2D(localHero) < 200
                                                            && x.IsAlive
                                                            && x.IsEnchanted()))
                {
                    return;
                }

                stone.Base.Cast(localHero.InFront(100));
                return;
            }

            if (context.smartGrip.Value && e.Order.Ability == grip.Base)
            {
                var mousePos = GameManager.MousePosition;

                if (StoneExtensions.FirstUnitInRange(mousePos, stoneName, 200) != null
                    || EntityManager.GetEntities<Hero>().Any(x => x.Distance2D(mousePos) < 200
                        && x.IsAlive
                        && (x.IsEnchanted()
                            || (x.IsAlly(localHero) && localHero.Spellbook.Talents
                                                        .FirstOrDefault(x => x.Id == AbilityId.special_bonus_unique_earth_spirit_2).Level > 0))))
                {
                    return;
                }

                stone.Base.Cast(mousePos);
                return;
            }

            if (context.smartRoll.Value && e.Order.Ability == roll.Base)
            {
                var orderPos = e.Order.Position;

                if (orderPos.Distance2D(localHeroPos) > roll.Range)
                {
                    return;
                }

                if (StoneExtensions.FirstUnitInRange(localHeroPos.Extend(orderPos, 100), stoneName, 200) != null
                    || StoneExtensions.FirstUnitInRange(localHeroPos.Extend(orderPos, 300), stoneName, 200) != null)
                {
                    return;
                }

                var startTime = 600 + (int)(localHero.TurnTime(orderPos) * 1000);

                UpdateManager.BeginInvoke(startTime, () =>
                {
                    stone.UseAbility();
                    return;
                });
            }
        }
    }
}
