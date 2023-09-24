using UnityEngine;
using UnityEngine.InputSystem;

namespace BFG.Avocado {
public class PlayerInputHandler : MonoBehaviour {
    [SerializeField]
    InputActionAsset inputActionAsset;

    [SerializeField]
    AvocadoController avocadoController;

    InputAction _actionJump;
    InputAction _actionMove;
    InputAction _actionThrow;
    InputActionMap _inputMapGameplay;

    void Start() {
        _inputMapGameplay = inputActionAsset.FindActionMap("Gameplay");

        _actionJump = _inputMapGameplay.FindAction("Jump");
        _actionMove = _inputMapGameplay.FindAction("Move");
        _actionThrow = _inputMapGameplay.FindAction("Throw");

        _inputMapGameplay.Enable();
    }

    void Update() {
        if (_actionThrow.WasReleasedThisFrame()) {
            avocadoController.Throw();
        }

        if (_actionJump.WasPressedThisFrame() && _actionJump.IsPressed()) {
            avocadoController.OnJumpStarted();
        }
        else if (_actionJump.WasReleasedThisFrame()) {
            avocadoController.OnJumpEnded();
        }
    }

    void FixedUpdate() {
        avocadoController.Move(_actionMove.ReadValue<float>());
    }

    void OnEnable() {
        _inputMapGameplay?.Enable();
    }

    void OnDisable() {
        _inputMapGameplay?.Disable();
    }
}
}
