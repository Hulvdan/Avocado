using System.Collections.Generic;
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

internal delegate void OnAvocadoJustGrounded(float height);

internal delegate void OnAvocadoJumped();

internal delegate void OnFootstepObserver();

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
    [Min(0)]
    internal float coyoteTime = .2f;

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

    [Header("Falling Check")]
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
    Event onFootstepWwiseEvent;

    [SerializeField]
    Event onJumpWwiseEvent;

    [SerializeField]
    Event onGroundedWwiseEvent;

    [SerializeField]
    GameObject onGroundedEventEmitter;

    [SerializeField]
    [Min(0)]
    float fallSoundStartHeight = 3.2f;

    [SerializeField]
    [Min(0)]
    float fallSoundMaxHeight = 6f;

    AvocadoState _state;
    AvocadoState[] _states;

    bool _wasGroundedOnPreviousFrame = true;
    internal Animator Animator;
    internal AnimatorOverrideController AnimatorOverrideController;

    internal AnimationClipOverrides ClipOverrides;
    internal bool HasSeed = true;

    internal float HighestVerticalPosition;

    internal float MoveAxisXValue;
    internal float MovementStateFallingElapsed;

    internal OnAvocadoGrounded OnAvocadoGrounded;
    internal OnAvocadoJumped OnAvocadoJumped;
    internal OnAvocadoJustGrounded OnAvocadoJustGrounded;
    internal OnFootstepObserver OnFootstepObserver;

    internal Rigidbody2D Rigidbody;

    internal GameObject Seed;

    void Start() {
        CreateStates();

        HighestVerticalPosition = transform.position.y;
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

        OnFootstepObserver += () => { onFootstepWwiseEvent.Post(gameObject); };
        OnAvocadoJumped += () => { onJumpWwiseEvent.Post(gameObject); };
        OnAvocadoJustGrounded += height => {
            var c = (height - fallSoundStartHeight) / (fallSoundMaxHeight - fallSoundStartHeight);
            var distance = 1 - Mathf.Clamp(c, 0, 1);
            if (distance >= 1) {
                return;
            }

            var localPos = onGroundedEventEmitter.transform.localPosition;
            onGroundedEventEmitter.transform.localPosition = new Vector3(
                localPos.x, localPos.y, distance
            );
            onGroundedWwiseEvent.Post(onGroundedEventEmitter);
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
            _state.OnGrounded(ref me);
            if (!_wasGroundedOnPreviousFrame) {
                _state.OnJustGrounded(ref me);
            }
        }
        else {
            _state.OnNotGrounded(ref me);
            if (_wasGroundedOnPreviousFrame) {
                _state.OnJustNotGrounded(ref me);
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

    public void OnHoldingActionJumpStarted() {
        var me = this;
        _state.OnHoldingActionJumpStarted(ref me);
    }

    public void OnHoldingActionJumpEnded() {
        var me = this;
        _state.OnHoldingActionJumpEnded(ref me);
    }

    public void Throw() {
        var me = this;
        _state.OnThrow(ref me);
    }

    void OnThrowAnimationEnded() {
        var me = this;
        _state.OnThrowAnimationEnded(ref me);
    }

    void OnFootstep() {
        OnFootstepObserver?.Invoke();
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
