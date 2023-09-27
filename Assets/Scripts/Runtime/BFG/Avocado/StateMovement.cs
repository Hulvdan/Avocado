using UnityEngine;

namespace BFG.Avocado {
internal class StateMovement : AvocadoState {
    public StateMovement(AvocadoState[] states) : base(states) {
    }

    public override void OnEnter(ref AvocadoController avocado) {
        avocado.Animator.SetBool(AvocadoAnimatorConsts.HashIsWalking, avocado.MoveAxisXValue != 0f);

        avocado.MovementStateFallingElapsed = 0;
    }

    public override void OnExit(ref AvocadoController avocado) {
        avocado.Animator.SetBool(AvocadoAnimatorConsts.HashIsWalking, false);
    }

    public override void OnThrow(ref AvocadoController avocado) {
        if (avocado.HasSeed) {
            SwitchState(ref avocado, AvocadoStateIndex.Throwing);
        }
    }

    public override void OnHoldingActionJumpStarted(ref AvocadoController avocado) {
        SwitchState(ref avocado, AvocadoStateIndex.Jumping);
    }

    public override void OnMove(ref AvocadoController avocado, float moveAxisXValueNew) {
        avocado.Animator.SetBool(AvocadoAnimatorConsts.HashIsWalking, moveAxisXValueNew != 0f);

        avocado.MoveAxisXValue = moveAxisXValueNew;
        UpdateHorizontalMovement(
            avocado,
            avocado.movementSpeed,
            avocado.movementAcceleration
        );
    }

    public override void OnJustGrounded(ref AvocadoController avocado) {
        base.OnJustGrounded(ref avocado);

        avocado.MovementStateFallingElapsed = 0;
    }

    public override void OnNotGrounded(ref AvocadoController avocado) {
        base.OnNotGrounded(ref avocado);

        if (avocado.MovementStateFallingElapsed < avocado.coyoteTime) {
            avocado.MovementStateFallingElapsed += Time.deltaTime;
        }
        else {
            avocado.MovementStateFallingElapsed = 0;
            SwitchState(ref avocado, AvocadoStateIndex.Falling);
        }
    }
}
}
