using System;
using Cysharp.Threading.Tasks;
using Noneb.UI.View;
using NonebNi.Core.Actions;
using NonebNi.Core.Units;
using NonebNi.Ui.ViewComponents.PlayerTurn;
using UnityEngine;

namespace NonebNi.Develop
{
    public class UnitDetailsTestScript : MonoBehaviour
    {
        [SerializeField] private UnitDetailsPanel panel = null!;
        [SerializeField] private NonebViewBehaviour view = null!;
        [SerializeField] private GameObject stackRoot = null!;

        private readonly Lazy<UnitData> _unitA = new(
            () => new UnitData(
                Guid.NewGuid(),
                Array.Empty<NonebAction>(),
                Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.zero),
                "A",
                "fake-faction",
                100,
                50,
                30,
                3,
                3,
                3,
                3,
                3,
                0,
                10,
                15,
                1
            )
        );

        private readonly Lazy<UnitData> _unitB = new(
            () => new UnitData(
                Guid.NewGuid(),
                Array.Empty<NonebAction>(),
                Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.zero),
                "B",
                "fake-faction",
                40,
                30,
                20,
                3,
                3,
                3,
                3,
                3,
                0,
                10,
                15,
                1
            )
        );

        private UIStack _stack = null!;

        // ReSharper disable once Unity.IncorrectMethodSignature
        private async UniTaskVoid Start()
        {
            _stack = new UIStack(stackRoot);
            await _stack.Push(view);
        }

        private void OnGUI()
        {
            var rect = new Rect(10, 10, 150, 25);
            if (GUI.Button(rect, "Show A")) ShowPattern(_unitA.Value);
            rect.y += 25;

            if (GUI.Button(rect, "Show B")) ShowPattern(_unitB.Value);
            rect.y += 25;
        }

        private void ShowPattern(UnitData unit)
        {
            panel.Show(unit);
        }
    }
}