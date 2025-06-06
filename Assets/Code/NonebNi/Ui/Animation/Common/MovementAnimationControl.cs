﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Noneb.UI.Animation;
using UnityEngine;

namespace NonebNi.Ui.Animation.Common
{
    public class MovementAnimationControl : MonoBehaviour
    {
        [SerializeField] private Animator animator = null!;
        [SerializeField] private Transform movedTransform = null!;

        [SerializeField] private AnimationData startMovementData = null!;
        [SerializeField] private AnimationData stopMovementData = null!;

        [SerializeField] private float speed;

        public async UniTask WalkTo(CancellationToken ct = default, params Vector3[] waypoints)
        {
            //TODO: bezier or whatever the other line is called.
            await WalkTo(waypoints, ct);
        }

        public async UniTask WalkTo(IEnumerable<Vector3> waypoints, CancellationToken ct = default)
        {
            waypoints = waypoints.ToArray();

            animator.PlayAnimation(startMovementData).Forget();

            foreach (var position in waypoints) await WalkToPos(position);

            animator.PlayAnimation(stopMovementData).Forget();
            movedTransform.position = waypoints.Last();

            return;

            async UniTask WalkToPos(Vector3 position)
            {
                movedTransform.LookAt(position);
                var dist = Vector3.Distance(movedTransform.position, position);
                var walkDuration = dist / speed;

                var timer = 0f;
                while (timer < walkDuration)
                {
                    if (ct.IsCancellationRequested) break;

                    timer += Time.deltaTime;
                    var delta = speed * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, position, delta);
                    await UniTask.NextFrame(ct, true).SuppressCancellationThrow();
                }
            }
        }
    }
}