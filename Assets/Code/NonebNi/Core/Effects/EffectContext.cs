using System.Collections.Generic;
using NonebNi.Core.Commands;
using NonebNi.Core.Entities;
using NonebNi.Core.Maps;

namespace NonebNi.Core.Effects
{
    //TODO: might need a "data" object that can store everything... kinda context within a context?
    public record EffectContext(IMap Map, ActionCommand Command, IEnumerable<EffectTargetGroup> TargetGroups)
    {
        public EntityData ActionCaster => Command.ActorEntity;
    }
}