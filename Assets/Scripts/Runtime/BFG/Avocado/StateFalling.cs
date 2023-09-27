using UnityEngine;

namespace BFG.Avocado {
internal class StateFalling : AvocadoState {
    float elapsed;

    public StateFalling(AvocadoState[] states) : base(states) {
    }

    public override void OnEnter(ref AvocadoController avocado) {
        avocado.Animator.SetBool(AvocadoAnimatorConsts.HashIsFalling, true);

        elapsed = 0;
    }

    public override void OnExit(ref AvocadoController avocado) {
        avocado.Animator.SetBool(AvocadoAnimatorConsts.HashIsFalling, false);

        avocado.Rigidbody.gravityScale = 1f;
    }

    public override void Update(ref AvocadoController avocado) {
        elapsed += Time.deltaTime;

        avocado.Rigidbody.gravityScale = Mathf.Lerp(
            avocado.fallingSpeedIncreaseInitialScale,
            avocado.fallingSpeedIncreaseMaxScale,
            elapsed / avocado.fallingSpeedIncreaseDuration
        );
    }

    public override void OnMove(ref AvocadoController avocado, float moveAxisXValueNew) {
        avocado.MoveAxisXValue = moveAxisXValueNew;
        UpdateHorizontalMovement(
            avocado,
            avocado.movementSpeed,
            avocado.airMovementAcceleration
        );
    }

    public override void OnGrounded(ref AvocadoController avocado) {
        base.OnGrounded(ref avocado);
        SwitchState(ref avocado, AvocadoStateIndex.Movement);
    }
}
}
