using NonebNi.Ui.Entities;

namespace NonebNi.Ui.Animation.Sequence
{
    public record ApplyDamageAnimSequence(string AnimId, Entity? DamageReceiver = null) : IAnimSequence;
}