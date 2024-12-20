using System.Threading;
using Cysharp.Threading.Tasks;
using NonebNi.Ui.Entities;
using UnityEngine;

namespace NonebNi.Ui.Animation
{
    //TODO: do I even need this?
    public class PrototypeMeleeWeaponAnimationControl : MonoBehaviour
    {
        private Entity? _expectedEntity;
        private bool _hitTarget;

        private void OnCollisionEnter(Collision other)
        {
            if (_expectedEntity == null) return;

            var hitEntity = other.gameObject.GetComponentInParent<Entity>();
            if (hitEntity == null) return;

            _hitTarget = true;
        }

        //TODO: test
        public async UniTask WaitTillHitEntity(Entity entity, CancellationToken ct = default)
        {
            _expectedEntity = entity;

            await UniTask.WaitUntil(() => _hitTarget, cancellationToken: ct);

            _expectedEntity = null;
            _hitTarget = false;
        }
    }
}