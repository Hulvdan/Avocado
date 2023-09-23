using System.Collections.Generic;
using UnityEngine;

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
public class AvocadoController : MonoBehaviour {
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
    [Range(0, 10)]
    float movementSpeed = 1f;

    Animator _animator;
    AnimatorOverrideController _animatorOverrideController;

    AnimationClipOverrides _clipOverrides;
    bool _hasSeed = true;
    float _moveAxisXValue;
    float _moveAxisXValueNew;

    bool _needToThrow;

    GameObject _seed;

    void Start() {
        _animator = GetComponent<Animator>();
        _animatorOverrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController);

        _clipOverrides = new AnimationClipOverrides(_animatorOverrideController.overridesCount);
        _animatorOverrideController.GetOverrides(_clipOverrides);

        _animator.runtimeAnimatorController = _animatorOverrideController;
    }

    void FixedUpdate() {
        if (_needToThrow) {
            _needToThrow = false;

            if (_hasSeed) {
                _animator.SetBool(HashIsThrowing, true);
            }
        }

        if ((_moveAxisXValue == 0f && _moveAxisXValueNew != 0f)
            || (_moveAxisXValueNew == 0f && _moveAxisXValue != 0f)) {
            _animator.SetBool(HashIsWalking, _moveAxisXValueNew != 0f);
        }

        _moveAxisXValue = _moveAxisXValueNew;
        if (_moveAxisXValue != 0f) {
            var cachedTransform = transform;
            var position = cachedTransform.position;

            position = new Vector3(
                position.x + movementSpeed * _moveAxisXValue,
                position.y,
                position.z
            );

            cachedTransform.position = position;
        }

        if (_moveAxisXValue != 0f) {
            var localScale = transform.localScale;
            localScale = new Vector3(
                Mathf.Sign(_moveAxisXValue) * Mathf.Abs(localScale.x),
                localScale.y,
                localScale.z
            );
            transform.localScale = localScale;
        }
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.CompareTag("Seed")) {
            OnSeedPickup();
            col.gameObject.SetActive(false);
        }
    }

    public void Move(float x) {
        _moveAxisXValueNew = x;
    }

    public void OnJumpStarted() {
    }

    public void OnJumpEnded() {
    }

    public void Throw() {
        _needToThrow = true;
    }

    void OnThrowEnded() {
        _animator.SetBool(HashIsThrowing, false);

        var cachedTransform = transform;
        var position = cachedTransform.position;

        var newSeedPosition = new Vector3(
            position.x + seedSpawnOffsetX,
            position.y + seedSpawnOffsetY,
            position.z - 1
        );
        if (_seed == null) {
            _seed = Instantiate(
                seedProjectile,
                newSeedPosition,
                cachedTransform.rotation
            );
        }
        else {
            _seed.SetActive(true);
            _seed.transform.position = newSeedPosition;
            _seed.GetComponent<Seed>().Reset();
        }

        _seed.GetComponent<Rigidbody2D>().AddForce(
            new Vector2(
                seedSpawnForceX * Mathf.Sign(transform.localScale.x),
                seedSpawnForceY
            ),
            ForceMode2D.Impulse
        );

        _hasSeed = false;
        _clipOverrides["AvocadoIdle"] = avocadoIdleNoSeedAnimationClip;
        _clipOverrides["AvocadoWalk"] = avocadoWalkNoSeedAnimationClip;
        _animatorOverrideController.ApplyOverrides(_clipOverrides);
    }

    void OnSeedPickup() {
        _clipOverrides["AvocadoIdle"] = avocadoIdleAnimationClip;
        _clipOverrides["AvocadoWalk"] = avocadoWalkAnimationClip;
        _animatorOverrideController.ApplyOverrides(_clipOverrides);
        _hasSeed = true;
    }
}
}
