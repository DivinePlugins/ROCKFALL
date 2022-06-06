using Divine.Entity.Entities.Units;
using ESExtermination.Extensions;

namespace ESExtermination.Abilities.Events.GripUsedEvent
{
    internal class GripUsedEventArgs
    {
        public Unit Target { get; }
        public bool TargetIsStone { get; }

        public GripUsedEventArgs(Unit target)
        {
            Target = target;
            TargetIsStone = target.IsStone();
        }
    }
}
