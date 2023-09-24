using UnityEngine;

namespace BFG.Avocado {
internal static class AvocadoAnimatorConsts {
    public static readonly string AvocadoIdleState = "AvocadoIdle";
    public static readonly string AvocadoWalkState = "AvocadoWalk";
    public static readonly string AvocadoJumpState = "AvocadoJump";
    public static readonly string AvocadoFallState = "AvocadoFall";

    public static readonly int HashIsWalking = Animator.StringToHash("IsWalking");
    public static readonly int HashIsThrowing = Animator.StringToHash("IsThrowing");
    public static readonly int HashIsJumping = Animator.StringToHash("IsJumping");
    public static readonly int HashIsFalling = Animator.StringToHash("IsFalling");
}
}
