﻿using UnityEngine;

namespace BFG.Avocado {
internal class StateThrowing : AvocadoState {
    static readonly int HashIsThrowing = Animator.StringToHash("IsThrowing");

    public StateThrowing(AvocadoState[] states) : base(states) {
    }

    public override void OnEnter(ref AvocadoController avocado) {
        avocado.Animator.SetBool(HashIsThrowing, true);
    }

    public override void OnThrowEnded(ref AvocadoController avocado) {
        avocado.Animator.SetBool(HashIsThrowing, false);

        var cachedTransform = avocado.transform;
        var position = cachedTransform.position;

        var newSeedPosition = new Vector3(
            position.x + avocado.seedSpawnOffsetX,
            position.y + avocado.seedSpawnOffsetY,
            position.z - 1
        );
        if (avocado.Seed == null) {
            avocado.Seed = Object.Instantiate(
                avocado.seedProjectile,
                newSeedPosition,
                cachedTransform.rotation
            );
        }
        else {
            avocado.Seed.SetActive(true);
            avocado.Seed.transform.position = newSeedPosition;
            avocado.Seed.GetComponent<Seed>().Reset();
        }

        avocado.Seed.GetComponent<Rigidbody2D>().AddForce(
            new Vector2(
                avocado.seedSpawnForceX * Mathf.Sign(avocado.transform.localScale.x),
                avocado.seedSpawnForceY
            ),
            ForceMode2D.Impulse
        );

        avocado.HasSeed = false;
        avocado.ClipOverrides["AvocadoIdle"] = avocado.avocadoIdleNoSeedAnimationClip;
        avocado.ClipOverrides["AvocadoWalk"] = avocado.avocadoWalkNoSeedAnimationClip;
        avocado.AnimatorOverrideController.ApplyOverrides(avocado.ClipOverrides);

        SwitchState(ref avocado, AvocadoStateIndex.Movement);
    }
}
}
