using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;

namespace NonebNi.Editors.AttributeDrawers
{
    internal class AnimatorInfoCache
    {
        private static readonly Dictionary<int, AnimatorInfoCache> Cache = new Dictionary<int, AnimatorInfoCache>();
        private readonly string[][] _layeredStates;

        private readonly Dictionary<AnimatorControllerParameterType, string[]> _parameters;

        private AnimatorInfoCache(RuntimeAnimatorController animatorController)
        {
            string[] FindParametersOfType(AnimatorControllerParameterType type)
            {
                var result = new List<string>();
                var controller = GetAnimatorController(animatorController);
                if (controller != null)
                    result.AddRange(from temp in controller.parameters where temp.type == type select temp.name);

                return result.ToArray();
            }

            _parameters = new Dictionary<AnimatorControllerParameterType, string[]>
            {
                [AnimatorControllerParameterType.Trigger] = FindParametersOfType(AnimatorControllerParameterType.Trigger),
                [AnimatorControllerParameterType.Bool] = FindParametersOfType(AnimatorControllerParameterType.Bool),
                [AnimatorControllerParameterType.Int] = FindParametersOfType(AnimatorControllerParameterType.Int),
                [AnimatorControllerParameterType.Float] = FindParametersOfType(AnimatorControllerParameterType.Float)
            };

            string[][] FindStates()
            {
                var ctrl = GetAnimatorController(animatorController);
                if (ctrl != null)
                {
                    var allLayer = ctrl.layers;
                    return allLayer.Select(t => t.stateMachine.states)
                                   .Select(states => states.Select(s => s.state.name).ToArray())
                                   .ToArray();
                }

                return Array.Empty<string[]>();
            }

            _layeredStates = FindStates();
        }

        internal static void ClearData()
        {
            Cache.Clear();
        }

        internal static AnimatorInfoCache GetParamTable(RuntimeAnimatorController animatorController)
        {
            var id = animatorController.GetInstanceID();
            if (!Cache.TryGetValue(id, out var result))
            {
                result = new AnimatorInfoCache(animatorController);
                Cache[id] = result;
            }

            return result;
        }

        internal string[] GetParameters(AnimatorControllerParameterType type) => _parameters[type];

        internal string[] GetStates(int layerIndex)
        {
            var isReturnAllLayers = layerIndex == -1;
            return isReturnAllLayers ? _layeredStates.SelectMany(i => i).ToArray() : _layeredStates[layerIndex];
        }

        private static AnimatorController? GetAnimatorController(RuntimeAnimatorController? controller)
        {
            AnimatorController FindRootOfOverridenController(AnimatorOverrideController o)
            {
                return o.runtimeAnimatorController switch
                {
                    AnimatorController a => a,
                    AnimatorOverrideController parent => FindRootOfOverridenController(parent),
                    _ => throw new ArgumentException("Unexpected type")
                };
            }

            return controller switch
            {
                AnimatorController animatorController => animatorController,
                AnimatorOverrideController overrideController => FindRootOfOverridenController(overrideController),
                null => null,
                _ => throw new ArgumentException("Unexpected type")
            };
        }
    }
}