using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Units;
using Divine.Extensions;
using Divine.Numerics;
using Divine.Prediction;
using ESExtermination.Extensions;

namespace ESExtermination.Abilities.Spells
{
    internal class Smash : SpellBase
    {
        public override string DebuffName { get; } = "modifier_earth_spirit_boulder_smash_debuff";

        public override float Speed { get; } = 900;

        public Smash(Ability baseAbility)
            : base(baseAbility)
        {

        }

        public Vector3 GetPrediction(Unit target)
        {
            var input = new PredictionInput
            {
                Owner = this.Owner,
                Range = 2000,
                Speed = this.Speed,
            };

            input = input.WithTarget(target);

            return PredictionManager.GetPrediction(input).UnitPosition;
        }

        public float PushDistance
        {
            get
            {
                return 400 + Base.Level * 100;
            }
        }

        public bool UseAbilityOnStone(Unit target)
        {
            var stone = StoneExtensions.FirstUnitInRange(Owner.Position, StoneExtensions.StoneName, 200);

            if (stone == null)
            {
                return false;
            }

            var castPos = Owner.Position + (GetPrediction(target) - stone.Position).Normalized() * 400;

            return base.UseAbility(castPos);
        }

        public override bool CanBeCasted()
        {
            if (Sleeper.Sleeping)
            {
                return false;
            }


            if (Base.Level == 0)
            {
                return false;
            }

            if (Base.ManaCost > Owner.Mana)
            {
                return false;
            }

            if (Base.Cooldown != 0)
            {
                return false;
            }

            if (Base.Charges > 0 && Base.CurrentCharges == 0)
            {
                return false;
            }

            if (!Owner.IsEnchanted() && (Owner.IsStunned() || Owner.IsHexed() || Owner.IsMuted() || Owner.IsSilenced()))
            {
                return false;
            }

            return true;
        }
    }
}
