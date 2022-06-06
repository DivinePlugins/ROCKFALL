using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Units;

namespace ESExtermination.Abilities.Items
{
    internal class Shivas : ItemBase
    {
        public override float Range { get; } = 900;

        public Shivas(Ability baseAbility)
            : base(baseAbility)
        {

        }

        public override bool UseAbility(Unit target)
        {
            if (!CanHit(target))
            {
                return false;
            }

            return base.UseAbility();
        }
    }
}
