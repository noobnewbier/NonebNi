using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Maps;
using NonebNi.Core.Sequences;
using Unity.Logging;

namespace NonebNi.Core.Effects
{
    public class MoveOverEffect : Effect
    {
        public class Evaluator : Evaluator<MoveOverEffect>
        {
            protected override IEnumerable<ISequence> OnEvaluate(
                MoveOverEffect effect,
                EffectContext context)
            {
                if (context.Targets.FirstOrDefault() is not Coordinate targetCoord)
                {
                    Log.Error($"{nameof(MoveOverEffect)} without a coordinate parameter makes no sense!");
                    yield break;
                }

                if (!context.Map.TryFind(context.ActionCaster, out Coordinate actorCoord))
                {
                    Log.Error($"{context.ActionCaster.Name} is not on the map!");
                    yield break;
                }

                if (!actorCoord.IsOnSameLineWith(targetCoord))
                {
                    Log.Error(
                        $"{targetCoord} is not on the same line with {context.ActionCaster.Name} - effect is undefined!"
                    );
                    yield break;
                }

                var direction = (targetCoord - actorCoord).Normalized();
                var actorGoalCoord = targetCoord + direction;
                var result = context.Map.Move(context.ActionCaster, actorGoalCoord);
                if (result != MoveResult.Success)
                {
                    Log.Warning($"Failed movement! Reason: {result}.");
                    yield break;
                }

                //TODO: we need a proper pathing? with this one, but for now this suffice
                yield return new MoveSequence(context.ActionCaster, new[] { actorCoord, actorGoalCoord });
            }
        }
    }
}