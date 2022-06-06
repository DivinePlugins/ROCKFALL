using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Units;
using Divine.Extensions;

namespace ESExtermination.Abilities.Items
{
    internal class Hex : ItemBase
    {
        public Hex(Ability baseAbility)
            : base(baseAbility)
        {

        }

        public override bool UseAbility(Unit target)
        {
            if (target.IsStunned()
                || target.IsHexed()
                || target.IsInvulnerable()
                || target.IsReflectingAbilities())
            {
                return false;
            }

            return base.UseAbility(target);
        }
    }
}
