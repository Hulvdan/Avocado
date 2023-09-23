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
                this[index] = new KeyValuePair<AnimationClip, AnimationClip>(
                    this[index].Key, value
                );
            }
        }
    }
}

[RequireComponent(typeof(Animator))]
public class AvocadoController : MonoBehaviour {
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

    [SerializeField]
    AnimationClip avocadoIdleAnimationClip;

    [SerializeField]
    AnimationClip avocadoWalkAnimationClip;

    [SerializeField]
    internal AnimationClip avocadoIdleNoSeedAnimationClip;

    [SerializeField]
    internal AnimationClip avocadoWalkNoSeedAnimationClip;

    [SerializeField]
    [Range(0, 10)]
    internal float movementSpeed = 1f;

    AvocadoState _state;
    AvocadoState[] _states;

    internal Animator Animator;
    internal AnimatorOverrideController AnimatorOverrideController;

    internal AnimationClipOverrides ClipOverrides;
    internal bool HasSeed = true;

    internal GameObject Seed;

    void Start() {
        CreateStates();
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

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.CompareTag("Seed")) {
            OnSeedPickup();
            col.gameObject.SetActive(false);
        }
    }

    void CreateStates() {
        _states = new AvocadoState[] { null, null, null, null, null };
        _states[(int)AvocadoStateIndex.Movement] = new StateMovement(_states);
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
