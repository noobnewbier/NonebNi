using System;
using Noneb.Localization.Runtime;
using UnityEngine;

namespace NonebNi.Ui.Tooltips
{
    /// <summary>
    /// At some point we might want to separate the content of a tooltip, and the request of a tooltip.
    /// this allow us to have the request solely handling shits like priority, positioning etc.
    /// </summary>
    [Serializable]
    public abstract record TooltipRequest
    {
        [Serializable]
        public record Text(NonebLocString Message) : TooltipRequest
        {
            [field: SerializeField] public NonebLocString Message { get; private set; } = Message;
        }
    }
}