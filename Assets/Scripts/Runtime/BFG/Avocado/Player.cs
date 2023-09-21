using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BFG.Avocado {
public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>> {
    public AnimationClipOverrides(int capacity) : base(capacity) {
    }

    public AnimationClip this[string name] {
        get { return Find(x => x.Key.name.Equals(name)).Value; }
        set {
            var index = FindIndex(x => x.Key.name.Equals(name));
            if (index != -1) {
                this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
            }
        }
    }
}

[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour {
    static readonly int HashIsWalking = Animator.StringToHash("IsWalking");
    static readonly int HashIsThrowing = Animator.StringToHash("IsThrowing");

    [SerializeField]
    GameObject seedProjectile;

    [SerializeField]
    float seedSpawnOffsetX;

    [SerializeField]
    float seedSpawnOffsetY;

    [SerializeField]
    float seedSpawnForceX;

    [SerializeField]
    float seedSpawnForceY;

    [SerializeField]
    AnimationClip avocadoIdleAnimationClip;

    [SerializeField]
    AnimationClip avocadoWalkAnimationClip;

    [SerializeField]
    AnimationClip avocadoIdleNoSeedAnimationClip;

    [SerializeField]
    AnimationClip avocadoWalkNoSeedAnimationClip;

    [SerializeField]
    InputActionAsset inputActionAsset;

    [SerializeField]
    [Range(0, 10)]
    float movementSpeed = 1f;

    InputAction _actionJump;
    InputAction _actionMove;
    InputAction _actionThrow;

    Animator _animator;
    AnimatorOverrideController _animatorOverrideController;
    bool _hasSeed = true;
    InputActionMap _inputMapGameplay;
    float _move;

    bool _needToThrow;
    AnimationClipOverrides clipOverrides;

    void Start() {
        _animator = GetComponent<Animator>();
        _animatorOverrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController);

        clipOverrides = new AnimationClipOverrides(_animatorOverrideController.overridesCount);
        _animatorOverrideController.GetOverrides(clipOverrides);

        _animator.runtimeAnimatorController = _animatorOverrideController;

        _inputMapGameplay = inputActionAsset.FindActionMap("Gameplay");

        _actionJump = _inputMapGameplay.FindAction("Jump");
        _actionMove = _inputMapGameplay.FindAction("Move");
        _actionThrow = _inputMapGameplay.FindAction("Throw");

        _inputMapGameplay.Enable();
    }

    void Update() {
        if (_actionThrow.WasReleasedThisFrame()) {
            _needToThrow = true;
        }
    }

    void FixedUpdate() {
        if (_needToThrow) {
            _needToThrow = false;

            if (_hasSeed) {
                _animator.SetBool(HashIsThrowing, true);
            }
        }

        var moveValue = _actionMove.ReadValue<float>();
        if ((_move == 0f && moveValue != 0f)
            || (moveValue == 0f && _move != 0f)) {
            _animator.SetBool(HashIsWalking, moveValue != 0f);
        }

        _move = moveValue;
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

        if (_move != 0f) {
            var localScale = transform.localScale;
            localScale = new Vector3(
                Mathf.Sign(_move) * Mathf.Abs(localScale.x),
                localScale.y,
                localScale.z
            );
            transform.localScale = localScale;
        }
    }

    void OnThrowEnded() {
        _animator.SetBool(HashIsThrowing, false);

        var cachedTransform = transform;
        var position = cachedTransform.position;

        var spawnedSeed = Instantiate(
            seedProjectile,
            new Vector3(
                position.x + seedSpawnOffsetX,
                position.y + seedSpawnOffsetY,
                position.z - 1
            ),
            cachedTransform.rotation
        );
        spawnedSeed.GetComponent<Rigidbody2D>().AddForce(
            new Vector2(
                seedSpawnForceX * Mathf.Sign(transform.localScale.x),
                seedSpawnForceY
            ),
            ForceMode2D.Impulse
        );

        _hasSeed = false;
        clipOverrides["AvocadoIdle"] = avocadoIdleNoSeedAnimationClip;
        clipOverrides["AvocadoWalk"] = avocadoWalkNoSeedAnimationClip;
        _animatorOverrideController.ApplyOverrides(clipOverrides);
    }
}
}
