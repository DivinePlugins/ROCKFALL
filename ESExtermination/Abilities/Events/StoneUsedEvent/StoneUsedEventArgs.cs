using Divine.Numerics;

namespace ESExtermination.Abilities.Events.StoneUsedEvent
{
    internal class StoneUsedEventArgs
    {
        public Vector3 StonePosition { get; }

        public StoneUsedEventArgs(Vector3 stonePosition)
        {
            StonePosition = stonePosition;
        }
    }
}
