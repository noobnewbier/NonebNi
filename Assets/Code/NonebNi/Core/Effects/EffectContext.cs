using System.Collections.Generic;
using NonebNi.Core.Actions;
using NonebNi.Core.Entities;
using NonebNi.Core.Maps;

namespace NonebNi.Core.Effects
{
    //TODO: might need a "data" object that can store everything... kinda context within a context?
    public record EffectContext(IMap Map, EntityData ActionCaster, IEnumerable<IActionTarget> Targets);
}