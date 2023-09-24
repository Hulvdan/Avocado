using UnityEngine;

namespace BFG.Avocado {
internal class StateJumping : AvocadoState {
    public StateJumping(AvocadoState[] states) : base(states) {
    }

    public override void OnEnter(ref AvocadoController avocado) {
        avocado.Rigidbody.AddForce(
            Vector2.up * GameManager.InitialJumpVelocity,
            ForceMode2D.Impulse
        );
        // Debug.Log($"StateJumping: InitialJumpVelocity: {GameManager.InitialJumpVelocity}");
        avocado.Animator.SetBool(AvocadoAnimatorConsts.HashIsJumping, true);
    }

    public override void OnExit(ref AvocadoController avocado) {
        avocado.Animator.SetBool(AvocadoAnimatorConsts.HashIsJumping, false);
    }

    public override void OnJumpEnded(ref AvocadoController avocado) {
        var v = avocado.Rigidbody.velocity;
        if (v.y > 0f) {
            avocado.Rigidbody.velocity = new Vector2(v.x, v.y / 3f);
        }
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

    public override void Update(ref AvocadoController avocado) {
        var v = avocado.Rigidbody.velocity;
        if (v.y <= 0f) {
            SwitchState(ref avocado, AvocadoStateIndex.Falling);
        }
    }
}
}
