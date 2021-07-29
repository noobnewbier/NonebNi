using NonebNi.Core.BoardItems;
using UnityEngine;
using UnityUtils.Constants;

namespace NonebNi.Core.Constructs
{
    [CreateAssetMenu(menuName = MenuName.Data + nameof(Construct), fileName = nameof(ConstructDataScriptable))]
    public class ConstructDataScriptable : BoardItemDataScriptable
    {
        [SerializeField] private string constructName;

        public ConstructData ToData() => new ConstructData(Icon, constructName, this);
    }
}