using System.Threading.Tasks;
using UnityEngine;

namespace NonebNi.Core.Maps
{
    /// <summary>
    /// A temporary class for holding a map such that others(editor tools etc) can interact with the map.
    /// Should be refactored/removed after we decide how to handle various dependencies
    /// </summary>
    public class MapHolder : MonoBehaviour
    {
        [SerializeField] private MapConfig? mapConfig;
        [SerializeField] private WorldConfig? worldConfig;
        // public LazyTask
    }
}