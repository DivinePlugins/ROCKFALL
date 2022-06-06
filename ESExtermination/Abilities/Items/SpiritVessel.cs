using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Units;
using Divine.Extensions;

namespace ESExtermination.Abilities.Items
{
    internal class SpiritVessel : ItemBase
    {
        public SpiritVessel(Ability baseAbility)
            : base(baseAbility)
        {

        }

        public override bool UseAbility(Unit target)
        {
            if (target.IsInvulnerable() || target.IsReflectingAbilities())
            {
                return false;
            }

            return base.UseAbility(target);
        }
    }
}
