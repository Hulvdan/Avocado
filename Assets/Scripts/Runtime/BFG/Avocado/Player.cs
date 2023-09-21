using UnityEngine;
using UnityEngine.InputSystem;

namespace BFG.Avocado {
[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour {
    static readonly int HashIsWalking = Animator.StringToHash("isWalking");

    [SerializeField]
    InputActionAsset inputActionAsset;

    [SerializeField]
    [Range(0, 10)]
    float movementSpeed = 1f;

    InputAction _actionJump;
    InputAction _actionMove;

    Animator _animator;
    InputActionMap _inputMapGameplay;
    float _move;

    void Start() {
        _animator = GetComponent<Animator>();

        _inputMapGameplay = inputActionAsset.FindActionMap("Gameplay");

        _actionJump = _inputMapGameplay.FindAction("Jump");
        _actionMove = _inputMapGameplay.FindAction("Move");

        _inputMapGameplay.Enable();
    }

    void FixedUpdate() {
        var move = _actionMove.ReadValue<float>();
        if ((_move == 0f && move != 0f)
            || (move == 0f && _move != 0f)) {
            _animator.SetBool(HashIsWalking, move == 0f);
        }

        _move = move;
        if (_move != 0f) {
            var cachedTransform = transform;
            var position = cachedTransform.position;

            position = new Vector3(
                position.x + movementSpeed * _move,
                position.y,
                position.z
            );

            cachedTransform.position = position;
        }

        var localScale = transform.localScale;
        localScale = new Vector3(
            Mathf.Sign(_move) * Mathf.Abs(localScale.x),
            localScale.y,
            localScale.z
        );
        transform.localScale = localScale;
    }
}
}
