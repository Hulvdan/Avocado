using UnityEngine;

namespace BFG.Avocado {
internal class StateJumping : AvocadoState {
    public StateJumping(AvocadoState[] states) : base(states) {
    }

    public override void OnEnter(ref AvocadoController avocado) {
        avocado.JumpingVelocity = avocado.jumpingForce;
        avocado.Rigidbody.AddForce(
            Vector2.up * avocado.jumpingForce,
            ForceMode2D.Impulse
        );

        avocado.StartedFalling = false;
    }

    public override void OnExit(ref AvocadoController avocado) {
    }

    public override void OnJumpEnded(ref AvocadoController avocado) {
        var v = avocado.Rigidbody.velocity;
        if (v.y > 0f) {
            avocado.Rigidbody.velocity = new Vector2(v.x, v.y / 3f);
        }
    }

    public override void OnMove(ref AvocadoController avocado, float moveAxisValue) {
    }

    public override void Update(ref AvocadoController avocado) {
        var v = avocado.Rigidbody.velocity;
        if (v.y <= 0f) {
            SwitchState(ref avocado, AvocadoStateIndex.Falling);
        }
    }
}
}
