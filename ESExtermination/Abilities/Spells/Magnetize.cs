using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Units;
using Divine.Extensions;

namespace ESExtermination.Abilities.Spells
{
    internal class Magnetize : SpellBase
    {
        public override float Radius { get; } = 350;

        public override string DebuffName { get; } = "modifier_earth_spirit_magnetize";

        public override bool IsIgnoringBkb { get; } = true;

        public Magnetize(Ability baseAbility)
            : base(baseAbility)
        {

        }

        public override bool UseAbility(Unit target)
        {
            if (Owner.Distance2D(target) > Radius)
            {
                return false;
            }

            return base.UseAbility();
        }
    }

}
