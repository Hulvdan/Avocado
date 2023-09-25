using UnityEngine;

namespace BFG.Avocado {
internal static class AvocadoAnimatorConsts {
    public const string AvocadoIdleState = "AvocadoIdle";
    public const string AvocadoWalkState = "AvocadoWalk";
    public const string AvocadoJumpState = "AvocadoJump";
    public const string AvocadoFallState = "AvocadoFall";

    public static readonly int HashIsWalking = Animator.StringToHash("IsWalking");
    public static readonly int HashIsThrowing = Animator.StringToHash("IsThrowing");
    public static readonly int HashIsJumping = Animator.StringToHash("IsJumping");
    public static readonly int HashIsFalling = Animator.StringToHash("IsFalling");
}
}
