using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Units;

namespace ESExtermination.Abilities.Spells
{
    internal class Grip : SpellBase
    {
        public override float Speed { get; } = 900;

        public override string DebuffName { get; } = "modifier_earth_spirit_geomagnetic_grip_debuff";

        public Grip(Ability baseAbility)
            : base(baseAbility)
        {

        }

        public override bool UseAbility(Unit target)
        {
            if (!base.UseAbility(target))
            {
                return false;
            }

            return true;
        }
    }
}
