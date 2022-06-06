using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Units;

namespace ESExtermination.Abilities.Items
{
    internal class VeilOfDiscord : ItemBase
    {
        public VeilOfDiscord(Ability baseAbility)
            : base(baseAbility)
        {

        }

        public override bool UseAbility(Unit target)
        {
            if (!CanHit(target))
            {
                return false;
            }

            return base.UseAbility(target.Position);
        }
    }
}
