using Divine.Entity.Entities.Abilities;
using Divine.Extensions;
using Divine.Prediction;
using Divine.Update;
using ESExtermination.Abilities.Events.StoneUsedEvent;
using ESExtermination.Extensions;

namespace ESExtermination.Abilities.Spells
{
    internal class Stone : SpellBase
    {
        public static event StoneHandle StoneUsed;

        public Stone(Ability baseAbility)
            : base(baseAbility)
        {
            Roll.RollUsed += Roll_RollUsed;
        }

        private void Roll_RollUsed(Roll sender, Event.RollUsedEvent.RollUsedEventArgs e)
        {

            UpdateManager.BeginInvoke(650, () =>
            {
                var target = TargetSelector.CurrentTarget;
                var dist = Owner.Distance2D(e.Destination);

                if (dist < 300)
                {
                    return;
                }

                var newTargetPos = PredictionManager.GetPrediction(e.PredictionInput).UnitPosition;

                if (StoneExtensions.FirstUnitBetween(Owner.Position, newTargetPos, StoneExtensions.StoneName, 250, 600) != null)
                {
                    return;
                }

                if (e.Destination.Distance2D(newTargetPos) > 200 + dist / 3)
                {
                    return;
                }

                UseAbility(Owner.Position.Extend(newTargetPos, 150));
            });
        }

        public override bool UseAbility()
        {
            var inFront = Owner.InFront(100);

            if (!base.UseAbility(inFront))
            {
                return false;
            }

            StoneUsed?.Invoke(this, new StoneUsedEventArgs(inFront));
            return true;
        }
    }
}
