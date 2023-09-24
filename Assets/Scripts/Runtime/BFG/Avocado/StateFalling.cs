﻿using UnityEngine;

namespace BFG.Avocado {
internal class StateFalling : AvocadoState {
    public StateFalling(AvocadoState[] states) : base(states) {
    }

    public override void OnEnter(ref AvocadoController avocado) {
        avocado.Animator.SetBool(AvocadoAnimatorConsts.HashIsFalling, true);
    }

    public override void OnExit(ref AvocadoController avocado) {
        avocado.Animator.SetBool(AvocadoAnimatorConsts.HashIsFalling, false);
    }

    public override void OnMove(ref AvocadoController avocado, float moveAxisXValueNew) {
        avocado.MoveAxisXValue = moveAxisXValueNew;
        if (avocado.MoveAxisXValue != 0f) {
            var cachedTransform = avocado.transform;
            var position = cachedTransform.position;

            position = new Vector3(
                position.x + avocado.movementSpeed * avocado.MoveAxisXValue,
                position.y,
                position.z
            );

            cachedTransform.position = position;
        }

        if (avocado.MoveAxisXValue != 0f) {
            var localScale = avocado.transform.localScale;
            localScale = new Vector3(
                Mathf.Sign(avocado.MoveAxisXValue) * Mathf.Abs(localScale.x),
                localScale.y,
                localScale.z
            );
            avocado.transform.localScale = localScale;
        }
    }

    public override void OnGrounded(ref AvocadoController avocado) {
        SwitchState(ref avocado, AvocadoStateIndex.Movement);
    }
}
}