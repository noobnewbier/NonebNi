using System.Linq;
using Cysharp.Threading.Tasks;
using NonebNi.Core.Actions;
using NonebNi.Core.Effects;
using NonebNi.Core.Sequences;
using NonebNi.EditorHacks;
using NonebNi.Ui.Animation.MannequinFighter;
using NonebNi.Ui.Animation.Sequence;
using UnityEditor;
using UnityEngine;
using UnityUtils.Editor;

namespace NonebNi.CustomInspector.CustomDrawers
{
    [CustomEditor(typeof(MannequinFighterAnimationPlayer))]
    public class MannequinFighterAnimationPlayerEditor : Editor
    {
        private AutoCompleteField _animIdField = null!;
        private string _animIdInput = string.Empty;

        private void OnEnable()
        {
            _animIdField = new AutoCompleteField(
                s => _animIdInput = s,
                _ => { },
                () =>
                {
                    /*
                     * In theory we should have a table somewhere storing "action Id -> anim Id",
                     * but we don't now, so we just pretend those two must be the same.
                     *
                     * good enough for now and can fix later.
                     */
                    return ActionDatas.Actions
                        .Where(a => a.Effects.OfType<DamageEffect>().Any())
                        .Select(a => a.Id);
                },
                "Apply Damage Anim Id",
                _animIdInput
            );
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (serializedObject.targetObjects.Length > 1) return;
            using (new EditorGUI.DisabledScope(!Application.isPlaying))
            {
                var typedTarget = (MannequinFighterAnimationPlayer)target;

                using (new FlowLayoutScope())
                {
                    if (GUILayout.Button(nameof(DieSequence), GUILayout.ExpandWidth(false))) typedTarget.Play(new DieAnimSequence());
                    if (GUILayout.Button(nameof(DamageSequence), GUILayout.ExpandWidth(false))) typedTarget.Play(new ReceivedDamageAnimSequence());
                }

                using (new GUILayout.HorizontalScope())
                {
                    _animIdField.OnGUI();
                    if (GUILayout.Button(nameof(DamageSequence), GUILayout.ExpandWidth(false)))
                        typedTarget.Play(new ApplyDamageAnimSequence(_animIdInput)).Forget();
                }
            }
        }
    }
}