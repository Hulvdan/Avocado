using UnityEngine;

namespace BFG.Avocado {
internal class StateMovement : AvocadoState {
    static readonly int HashIsWalking = Animator.StringToHash("IsWalking");

    float _moveAxisXValue;

    public StateMovement(AvocadoState[] states) : base(states) {
    }

    public override void OnThrow(ref AvocadoController avocado) {
        if (avocado.HasSeed) {
            SwitchState(ref avocado, AvocadoStateIndex.Throwing);
        }
    }

    public override void OnMove(ref AvocadoController avocado, float moveAxisXValueNew) {
        if ((_moveAxisXValue == 0f && moveAxisXValueNew != 0f)
            || (moveAxisXValueNew == 0f && _moveAxisXValue != 0f)) {
            avocado.Animator.SetBool(HashIsWalking, moveAxisXValueNew != 0f);
        }

        _moveAxisXValue = moveAxisXValueNew;
        if (_moveAxisXValue != 0f) {
            var cachedTransform = avocado.transform;
            var position = cachedTransform.position;

            position = new Vector3(
                position.x + avocado.movementSpeed * _moveAxisXValue,
                position.y,
                position.z
            );

            cachedTransform.position = position;
        }

        if (_moveAxisXValue != 0f) {
            var localScale = avocado.transform.localScale;
            localScale = new Vector3(
                Mathf.Sign(_moveAxisXValue) * Mathf.Abs(localScale.x),
                localScale.y,
                localScale.z
            );
            avocado.transform.localScale = localScale;
        }
    }
}
}
