using System;
using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Maps;
using NonebNi.Core.Pathfinding;
using NonebNi.Core.Sequences;
using Unity.Logging;

namespace NonebNi.Core.Effects
{
    [Serializable]
    public class MoveEffect : Effect
    {
        public class Evaluator : Evaluator<MoveEffect>
        {
            private readonly IPathfindingService _pathfindingService;

            public Evaluator(IPathfindingService pathfindingService)
            {
                _pathfindingService = pathfindingService;
            }

            protected override EffectResult OnEvaluate(
                MoveEffect effect,
                EffectContext context)
            {
                return new EffectResult(FindSequences());

                IEnumerable<ISequence> FindSequences()
                {
                    var targetParam = context.TargetGroups.FirstOrDefault()?.Targets.FirstOrDefault();
                    if (targetParam == null)
                    {
                        Log.Error("Move effect without any target makes no sense!");
                        yield break;
                    }

                    if (targetParam is not Coordinate targetCoord)
                    {
                        Log.Error(
                            "{targetParam} is not a Coordinate - MoveEffect must takes one coordinate as parameter!",
                            targetParam
                        );
                        yield break;
                    }

                    var (isPathExist, path) = _pathfindingService.FindPath(context.ActionCaster, targetCoord);
                    if (!isPathExist)
                    {
                        Log.Error("No path exist! Is it really an error... Should UI be able to catch this before entering here? And if there's an error we need to bubble it no");
                        yield break;
                    }

                    var result = context.Map.Move(context.ActionCaster, targetCoord);

                    //TODO: access to path finding service.

                    if (result == MoveResult.Success) yield return new MoveSequence(context.ActionCaster, path);
                }
            }
        }
    }
}