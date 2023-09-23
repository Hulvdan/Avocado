﻿using System.Collections.Generic;
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
                this[index] = new KeyValuePair<AnimationClip, AnimationClip>(
                    this[index].Key, value
                );
            }
        }
    }
}

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class AvocadoController : MonoBehaviour {
    [Header("Throwing")]
    [SerializeField]
    internal GameObject seedProjectile;

    [SerializeField]
    internal float seedSpawnOffsetX;

    [SerializeField]
    internal float seedSpawnOffsetY;

    [SerializeField]
    internal float seedSpawnForceX;

    [SerializeField]
    internal float seedSpawnForceY;

    [Header("Animations")]
    [SerializeField]
    AnimationClip avocadoIdleAnimationClip;

    [SerializeField]
    AnimationClip avocadoWalkAnimationClip;

    [SerializeField]
    internal AnimationClip avocadoIdleNoSeedAnimationClip;

    [SerializeField]
    internal AnimationClip avocadoWalkNoSeedAnimationClip;

    [Header("Movement")]
    [SerializeField]
    [Range(0, 10)]
    internal float movementSpeed = 1f;

    [SerializeField]
    internal float jumpingForce = 1f;

    [SerializeField]
    float _groundCheckOffset = -0.5f;

    [SerializeField]
    float _raycastDistance = 0.7f;

    [SerializeField]
    LayerMask layerTerrain;

    BoxCollider2D _collider;

    float _groundCheckOffsetFromCenterLeft;
    float _groundCheckOffsetFromCenterRight;

    AvocadoState _state;
    AvocadoState[] _states;

    bool _wasGroundedOnPreviousFrame = true;
    internal Animator Animator;
    internal AnimatorOverrideController AnimatorOverrideController;

    internal AnimationClipOverrides ClipOverrides;
    internal bool HasSeed = true;

    internal float JumpingVelocity;
    internal Rigidbody2D Rigidbody;

    internal GameObject Seed;

    internal bool StartedFalling;

    void Start() {
        CreateStates();

        _collider = GetComponent<BoxCollider2D>();
        var size = _collider.size;
        _groundCheckOffsetFromCenterLeft = -size.x / 2;
        _groundCheckOffsetFromCenterRight = size.x / 2;

        Rigidbody = GetComponent<Rigidbody2D>();

        _state = _states[0];

        Animator = GetComponent<Animator>();
        AnimatorOverrideController = new AnimatorOverrideController(
            Animator.runtimeAnimatorController
        );

        ClipOverrides = new AnimationClipOverrides(AnimatorOverrideController.overridesCount);
        AnimatorOverrideController.GetOverrides(ClipOverrides);

        Animator.runtimeAnimatorController = AnimatorOverrideController;

        var me = this;
        _state.OnEnter(ref me);
    }

    void Update() {
        var me = this;
        _state.Update(ref me);
    }

    void FixedUpdate() {
        UpdateIsGrounded();
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.CompareTag("Seed")) {
            OnSeedPickup();
            col.gameObject.SetActive(false);
        }
    }

    void OnDrawGizmos() {
        if (_collider == null) {
            _collider = GetComponent<BoxCollider2D>();
        }

        var size = _collider.size;
        _groundCheckOffsetFromCenterLeft = -size.x / 2;
        _groundCheckOffsetFromCenterRight = size.x / 2;

        var from1 = transform.position
                    + Vector3.right * _groundCheckOffsetFromCenterLeft
                    + Vector3.down * _groundCheckOffset;
        var from2 = transform.position
                    + Vector3.right * _groundCheckOffsetFromCenterRight
                    + Vector3.down * _groundCheckOffset;

        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(from1, Vector3.down);
        Gizmos.DrawRay(from2, Vector3.down);
    }

    void UpdateIsGrounded() {
        var from1 = transform.position
                    + Vector3.right * _groundCheckOffsetFromCenterLeft
                    + Vector3.down * _groundCheckOffset;
        var from2 = transform.position
                    + Vector3.right * _groundCheckOffsetFromCenterRight
                    + Vector3.down * _groundCheckOffset;

        var res1 = Physics2D.Raycast(from1, Vector2.down, _raycastDistance, layerTerrain);
        var res2 = Physics2D.Raycast(from2, Vector2.down, _raycastDistance, layerTerrain);

        var isGrounded = res1.collider != null || res2.collider != null;
        var me = this;
        if (isGrounded) {
            _state.OnGrounded(ref me);
        }
        else {
            _state.OnNotGrounded(ref me);
        }

        // if (isGrounded && !_wasGroundedOnPreviousFrame) {
        //     _state.OnGrounded(ref me);
        // }
        // else if (!isGrounded && _wasGroundedOnPreviousFrame) {
        //     _state.OnNotGrounded(ref me);
        // }

        _wasGroundedOnPreviousFrame = isGrounded;
    }

    void CreateStates() {
        _states = new AvocadoState[] { null, null, null, null, null };
        _states[(int)AvocadoStateIndex.Movement] = new StateMovement(_states);
        _states[(int)AvocadoStateIndex.Falling] = new StateFalling(_states);
        _states[(int)AvocadoStateIndex.Jumping] = new StateJumping(_states);
        _states[(int)AvocadoStateIndex.Throwing] = new StateThrowing(_states);
    }

    public void Move(float x) {
        var me = this;
        _state.OnMove(ref me, x);
    }

    public void OnJumpStarted() {
        var me = this;
        _state.OnJumpStarted(ref me);
    }

    public void OnJumpEnded() {
        var me = this;
        _state.OnJumpEnded(ref me);
    }

    public void Throw() {
        var me = this;
        _state.OnThrow(ref me);
    }

    void OnThrowEnded() {
        var me = this;
        _state.OnThrowEnded(ref me);
    }

    void OnSeedPickup() {
        ClipOverrides["AvocadoIdle"] = avocadoIdleAnimationClip;
        ClipOverrides["AvocadoWalk"] = avocadoWalkAnimationClip;
        AnimatorOverrideController.ApplyOverrides(ClipOverrides);
        HasSeed = true;
    }

    internal void SwitchState(AvocadoState newState) {
        var me = this;

        _state.OnExit(ref me);
        _state = newState;
        newState.OnEnter(ref me);
    }
}
}
