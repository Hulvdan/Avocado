using UnityEngine;

namespace BFG.Avocado {
internal abstract class AvocadoState {
    readonly AvocadoState[] _states;

    public AvocadoState(AvocadoState[] states) {
        _states = states;
    }

    public virtual void OnEnter(ref AvocadoController avocado) {
    }

    public virtual void OnExit(ref AvocadoController avocado) {
    }

    public virtual bool OnHoldingActionJumpStarted(ref AvocadoController avocado) {
        return false;
    }

    public virtual bool OnHoldingActionJumpEnded(ref AvocadoController avocado) {
        return false;
    }

    public virtual void OnMove(ref AvocadoController avocado, float moveAxisValue) {
    }

    public virtual void Update(ref AvocadoController avocado) {
    }

    public virtual void OnThrow(ref AvocadoController avocado) {
    }

    public virtual void OnThrowAnimationEnded(ref AvocadoController avocado) {
    }

    /// <summary>
    ///     Called every frame if avocado is grounded.
    /// </summary>
    public virtual void OnGrounded(ref AvocadoController avocado) {
        var y = avocado.transform.position.y;
        avocado.OnAvocadoGrounded?.Invoke(avocado.HighestVerticalPosition - y);
    }

    /// <summary>
    ///     Called once avocado grounds.
    /// </summary>
    public virtual void OnJustGrounded(ref AvocadoController avocado) {
        var y = avocado.transform.position.y;
        avocado.OnAvocadoJustGrounded?.Invoke(avocado.HighestVerticalPosition - y);
        avocado.HighestVerticalPosition = y;
    }

    /// <summary>
    ///     Called every frame if avocado is not grounded.
    /// </summary>
    public virtual void OnNotGrounded(ref AvocadoController avocado) {
        avocado.HighestVerticalPosition = Mathf.Max(
            avocado.HighestVerticalPosition,
            avocado.transform.position.y
        );
    }

    /// <summary>
    ///     Called once avocado is not grounded.
    /// </summary>
    public virtual void OnJustNotGrounded(ref AvocadoController avocado) {
    }

    protected void SwitchState(ref AvocadoController avocado, AvocadoStateIndex index) {
        // var stateName = "";
        // switch (index) {
        //     case AvocadoStateIndex.Movement:
        //         stateName = "Movement";
        //         break;
        //     case AvocadoStateIndex.Falling:
        //         stateName = "Falling";
        //         break;
        //     case AvocadoStateIndex.Jumping:
        //         stateName = "Jumping";
        //         break;
        //     case AvocadoStateIndex.Throwing:
        //         stateName = "Throwing";
        //         break;
        //     case AvocadoStateIndex.OnHit:
        //         stateName = "OnHit";
        //         break;
        // }
        // Debug.Log($"Switching to \"{stateName}\" state");

        avocado.SwitchState(_states[(int)index]);
    }

    protected static void UpdateHorizontalMovement(
        AvocadoController avocado, float maxMovementSpeed, float acceleration
    ) {
        avocado.Rigidbody.velocity = new Vector2(
            MathfUtils.RecalculateMovement(
                avocado.Rigidbody.velocity.x,
                maxMovementSpeed * avocado.MoveAxisXValue,
                acceleration
            ),
            avocado.Rigidbody.velocity.y
        );

        if (avocado.Rigidbody.velocity.x != 0f) {
            var localScale = avocado.transform.localScale;
            localScale = new Vector3(
                Mathf.Sign(avocado.Rigidbody.velocity.x) * Mathf.Abs(localScale.x),
                localScale.y,
                localScale.z
            );
            avocado.transform.localScale = localScale;
        }
    }
}
}
