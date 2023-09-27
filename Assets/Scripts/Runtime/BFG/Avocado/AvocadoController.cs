﻿using System.Collections.Generic;
using UnityEngine;
using Event = AK.Wwise.Event;

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

internal delegate void OnAvocadoGrounded(float height);

internal delegate void OnAvocadoJump();

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
    AnimationClip avocadoJumpAnimationClip;

    [SerializeField]
    AnimationClip avocadoFallAnimationClip;

    [SerializeField]
    internal AnimationClip avocadoIdleNoSeedAnimationClip;

    [SerializeField]
    internal AnimationClip avocadoWalkNoSeedAnimationClip;

    [SerializeField]
    internal AnimationClip avocadoJumpNoSeedAnimationClip;

    [SerializeField]
    internal AnimationClip avocadoFallNoSeedAnimationClip;

    [Header("Movement")]
    [SerializeField]
    [Min(0)]
    public float movementSpeed = 1f;

    [SerializeField]
    [Min(0)]
    public float movementAcceleration = 1f;

    [SerializeField]
    [Min(0)]
    public float airMovementAcceleration = .5f;

    [SerializeField]
    [Min(1)]
    public float jumpCutoff = 3f;

    [SerializeField]
    [Min(0)]
    public float movementSpeedWhileThrowing;

    [SerializeField]
    [Min(0)]
    internal float fallingSpeedIncreaseDuration = 1f;

    [SerializeField]
    [Min(1)]
    internal float fallingSpeedIncreaseInitialScale = 1f;

    [SerializeField]
    [Min(1)]
    internal float fallingSpeedIncreaseMaxScale = 3f;

    [SerializeField]
    [Min(0)]
    public float maxVerticalSpeed = 1f;

    [SerializeField]
    float groundCheckOffset = -0.5f;

    [SerializeField]
    [Min(0)]
    float groundCheckHorizontalOffset = 0.5f;

    [SerializeField]
    float raycastDistance = 0.7f;

    [SerializeField]
    LayerMask layerTerrain;

    [SerializeField]
    Event onJumpWWiseEvent;

    [SerializeField]
    Event onGroundedWWiseEvent;

    AvocadoState _state;
    AvocadoState[] _states;

    bool _wasGroundedOnPreviousFrame = true;
    internal Animator Animator;
    internal AnimatorOverrideController AnimatorOverrideController;

    internal AnimationClipOverrides ClipOverrides;
    internal bool HasSeed = true;

    internal float highestVerticalPosition;

    internal float MoveAxisXValue;

    internal OnAvocadoGrounded OnAvocadoGrounded;

    internal OnAvocadoJump OnAvocadoJumped;
    internal Rigidbody2D Rigidbody;

    internal GameObject Seed;

    internal float verticalVelocity;

    void Start() {
        CreateStates();

        highestVerticalPosition = transform.position.y;
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

        OnAvocadoJumped += () => { onJumpWWiseEvent.Post(gameObject); };
        OnAvocadoGrounded += height => {
            if (height > 2.2f) {
                onGroundedWWiseEvent.Post(gameObject);
            }
        };
    }

    void Update() {
        var me = this;
        _state.Update(ref me);
        // _state.OnGravity(ref me, gravity);
    }

    void FixedUpdate() {
        UpdateIsGrounded();

        var vel = Rigidbody.velocity;
        Rigidbody.velocity = new Vector2(
            vel.x,
            Mathf.Clamp(vel.y, -maxVerticalSpeed, maxVerticalSpeed)
        );
    }

    void OnDrawGizmos() {
        var pos = transform.position;
        var from1 = pos
                    + Vector3.right * groundCheckHorizontalOffset
                    + Vector3.down * groundCheckOffset;
        var from2 = pos
                    - Vector3.right * groundCheckHorizontalOffset
                    + Vector3.down * groundCheckOffset;

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(from1, from1 + Vector3.down * raycastDistance);
        Gizmos.DrawLine(from2, from2 + Vector3.down * raycastDistance);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("SeedTrigger")) {
            OnSeedPickup();
            other.gameObject.GetComponentInParent<Seed>().gameObject.SetActive(false);
        }
    }

    void UpdateIsGrounded() {
        var pos = transform.position;
        var from1 = pos
                    + Vector3.right * groundCheckHorizontalOffset
                    + Vector3.down * groundCheckOffset;
        var from2 = pos
                    - Vector3.right * groundCheckHorizontalOffset
                    + Vector3.down * groundCheckOffset;

        var res1 = Physics2D.Raycast(from1, Vector2.down, raycastDistance, layerTerrain);
        var res2 = Physics2D.Raycast(from2, Vector2.down, raycastDistance, layerTerrain);

        var isGrounded = res1.collider != null || res2.collider != null;
        var me = this;
        if (isGrounded) {
            _state.OnGroundFound(ref me);
            if (!_wasGroundedOnPreviousFrame) {
                _state.OnGrounded(ref me);
            }
        }
        else {
            _state.OnNoGroundFound(ref me);
            if (_wasGroundedOnPreviousFrame) {
                _state.OnNotGrounded(ref me);
            }
        }

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
        ClipOverrides[AvocadoAnimatorConsts.AvocadoIdleState] = avocadoIdleAnimationClip;
        ClipOverrides[AvocadoAnimatorConsts.AvocadoWalkState] = avocadoWalkAnimationClip;
        ClipOverrides[AvocadoAnimatorConsts.AvocadoFallState] = avocadoFallAnimationClip;
        ClipOverrides[AvocadoAnimatorConsts.AvocadoJumpState] = avocadoJumpAnimationClip;
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
