using Divine.Entity.Entities.Abilities;

namespace ESExtermination.Abilities.Spells
{
    internal class Enchant : SpellBase
    {
        public override string DebuffName { get; } = "modifier_earthspirit_petrify";

        public Enchant(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}
