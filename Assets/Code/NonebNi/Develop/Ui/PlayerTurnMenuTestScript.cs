using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Noneb.UI.View;
using NonebNi.Core.Actions;
using NonebNi.Core.Coordinates;
using NonebNi.Core.FlowControl;
using NonebNi.Core.Units;
using NonebNi.Ui.ViewComponents.PlayerTurn;
using UnityEditor;
using UnityEngine;
using UnityUtils;

namespace NonebNi.Develop
{
    //TODO: fix unit order panel
    //TODO: fix default not active.
    //TODO: some canvas group/animation disable isn't working but we can move on without it - another one for the backlog 
    public class PlayerTurnMenuTestScript : MonoBehaviour
    {
        [SerializeField] private PlayerTurnMenu menu = null!;
        [SerializeField] private NonebViewBehaviour view = null!;
        [SerializeField] private GameObject stackRoot = null!;

        private readonly Lazy<UnitData> _unitA = new(() => CreateUnit("A"));
        private readonly Lazy<UnitData> _unitB = new(() => CreateUnit("B"));
        private readonly Lazy<UnitData> _unitC = new(() => CreateUnit("C"));
        private MockInputControl _control = null!;
        private UIStack _stack = null!;

        // ReSharper disable once Unity.IncorrectMethodSignature
        private async UniTaskVoid Start()
        {
            var orderer = new FakeUniTurnOrderer(_unitA.Value, _unitB.Value, _unitC.Value);
            var presenter = new PlayerTurnPresenter(menu, orderer);
            _control = new MockInputControl();
            menu.Inject(presenter, _control);

            _stack = new UIStack(stackRoot);
            await _stack.Push(view);
        }

        private void OnGUI()
        {
            var rect = new Rect(Screen.width - 170, 10, 150, 25);

            GUI.Label(rect, _control.mode);
            rect.y += 25;
        }

        private static UnitData CreateUnit(string unitName) =>
            new(
                Guid.NewGuid(),
                new[] { ActionDatas.Bash, ActionDatas.Lure, ActionDatas.Shoot },
                AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd"),
                unitName,
                "fake-faction",
                100,
                40,
                10,
                5,
                10,
                10,
                10,
                3,
                10,
                50
            );

        private async UniTask ShowPattern(params UnitData[] units)
        {
            // await widget.Show(units);
        }

        private class FakeUniTurnOrderer : IUnitTurnOrderer
        {
            private readonly CircularBuffer<UnitData> _buffer;

            public FakeUniTurnOrderer(params UnitData[] unitsInOrder)
            {
                _buffer = new CircularBuffer<UnitData>(unitsInOrder);
            }

            public UnitData CurrentUnit => _buffer.Current;

            public IEnumerable<UnitData> UnitsInOrder => _buffer;
            public UnitData ToNextUnit() => _buffer.MoveNext();
        }

        private class MockInputControl : IPlayerTurnWorldSpaceInputControl
        {
            public string mode;
            public Coordinate? FindHoveredCoordinate() => null;

            public UniTask ToTargetSelectionMode(UnitData caster, NonebAction action)
            {
                mode = "target-selection";
                return UniTask.CompletedTask;
            }

            public UniTask ToTileInspectionMode()
            {
                mode = "tile-inspection";
                return UniTask.CompletedTask;
            }

            public void UpdateTargetSelection() { }
        }
    }
}