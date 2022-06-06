using Divine.Numerics;
using Divine.Prediction;

namespace ESExtermination.Abilities.Event.RollUsedEvent
{
    internal class RollUsedEventArgs
    {
        public PredictionInput PredictionInput { get; }
        public Vector3 Destination { get; }

        public RollUsedEventArgs(PredictionInput predictionInput, Vector3 destination)
        {
            PredictionInput = predictionInput;
            Destination = destination;
        }
    }
}
