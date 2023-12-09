using System.Linq;
using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Exceptions;
using Divine.Extensions;

namespace ESExtermination.Abilities.Items
{
    internal abstract class ItemBase : AbilityBase
    {
        protected ItemBase(Ability baseAbility)
            : base(baseAbility)
        {

        }

        public override bool CanBeCasted()
        {
            try
            {
                if (!Owner.Inventory.MainItems.Any(x => x == Base))
                {
                    return false;
                }
            }

            catch (EntityNotFoundException)
            {
                return false;
            }

            if (Context.ComboItems.GetValue(Base.Id) == false)
            {
                return false;
            }

            if (Owner.HasModifier("modifier_item_nullifier_mute"))
            {
                return false;
            }

            return base.CanBeCasted();
        }

    }
}
