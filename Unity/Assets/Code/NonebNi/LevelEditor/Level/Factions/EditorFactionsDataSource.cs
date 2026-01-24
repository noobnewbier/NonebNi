using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Factions;
using UnityEditor;
using UnityEngine;
using UnityUtils.Constants;

namespace NonebNi.LevelEditor.Level.Factions
{
    [CreateAssetMenu(
        fileName = nameof(EditorFactionsDataSource),
        menuName = MenuName.Data + nameof(EditorFactionsDataSource)
    )]
    public class EditorFactionsDataSource : ScriptableObject
    {
        [field: SerializeField] public Faction[] Factions { get; private set; } = null!;

        public void WriteData(IEnumerable<Faction> newFactions)
        {
            Factions = newFactions.ToArray();
            EditorUtility.SetDirty(this);
        }
    }
}