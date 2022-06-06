using System.Linq;
using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Units;
using Divine.Extensions;
using Divine.Game;
using Divine.Modifier.Modifiers;
using Divine.Prediction;
using ESExtermination.Abilities.Event.RollUsedEvent;
using ESExtermination.Extensions;
using HeroExtensions = ESExtermination.Extensions.HeroExtensions;

namespace ESExtermination.Abilities.Spells
{
    internal class Roll : SpellBase
    {
        public static event RollHandler RollUsed;

        public override float Speed { get; } = 800;

        public override float Radius { get; } = 180;

        public override string DebuffName { get; } = "modifier_stunned";

        public override float Range
        {
            get
            {
                float range = 750;

                if (Owner.Spellbook.Talents
                    .Any(x => x.Name == "special_bonus_unique_earth_spirit_4"
                                                                && x.Level == 1))
                {
                    range += 325;
                }

                return range;
            }
        }

        public Roll(Ability baseAbility)
            : base(baseAbility)
        {

        }

        public override bool UseAbility(Unit target)
        {
            if (!CanBeCasted() || target.IsMagicImmune())
            {
                return false;
            }

            var rudePrediction = target.IsMoving ? target.InFront(target.Speed) : target.Position;

            float range = this.Range;
            float speed = this.Speed;

            bool willGetStoneBuff = (Owner.Spellbook.Spell4.CurrentCharges > 0
                || StoneExtensions.FirstUnitBetween(Owner.Position, rudePrediction, StoneExtensions.StoneName, 250, 400) != null);

            if (willGetStoneBuff)
            {
                range += 750;
                speed += 800;
            }

            if (range < Owner.Distance2D(target))
            {
                return false;
            }

            var input = new PredictionInput
            {
                Delay = Owner.TurnTime(rudePrediction) + 0.6f, // 0.6f is "cast point"
                Range = range,
                Speed = speed,
                Radius = this.Radius,
                Owner = this.Owner
            };

            input = input.WithTarget(target);

            var predictionPoint = PredictionManager.GetPrediction(input).CastPosition;



            Base.Cast(predictionPoint);

            RollUsed?.Invoke(this, new RollUsedEventArgs(input, predictionPoint));

            return true;
        }

        public override bool CanBeCasted()
        {
            var target = TargetSelector.CurrentTarget;

            Modifier modifier = null;

            foreach (var mod in target.Modifiers)
            {
                modifier = target.GetModifierByName(HeroExtensions.InvulnerableModifiers.FirstOrDefault(x => x == mod.Name));
                if (modifier != null)
                {
                    break;
                }
            }

            if (modifier != null && modifier.DieTime - GameManager.RawGameTime + 0.1f > target.Distance2D(Owner) / 1600f + 0.6f + Owner.TurnTime(target.Position))
            {

                return false;
            }

            return base.CanBeCasted();
        }

    }
}
