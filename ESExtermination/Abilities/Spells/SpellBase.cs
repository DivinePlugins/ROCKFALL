using Divine.Entity.Entities.Abilities;
using Divine.Extensions;

namespace ESExtermination.Abilities.Spells
{
    internal abstract class SpellBase : AbilityBase
    {
        protected SpellBase(Ability baseAbility)
            : base(baseAbility)
        {

        }

        public override bool CanBeCasted()
        {
            if (Owner.IsSilenced())
            {
                return false;
            }

            if (Base.Level == 0)
            {
                return false;
            }

            return base.CanBeCasted();
        }
    }
}
