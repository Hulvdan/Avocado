using UnityEngine;

namespace BFG.Avocado {
[ExecuteAlways]
public class GameManager : MonoBehaviour {
    static float _gravity;
    public static float InitialJumpVelocity;

    [SerializeField]
    [Range(0.01f, 3f)]
    float secondsNeededToReachMaxJumpHeight = 0.6f;

    [SerializeField]
    [Range(0.01f, 10f)]
    float jumpHeight = 3f;

    void Start() {
        RecalculatePhysics();
    }

#if UNITY_EDITOR
    void Update() {
        RecalculatePhysics();
    }
#endif

    void RecalculatePhysics() {
        var t = secondsNeededToReachMaxJumpHeight;
        _gravity = -2f * jumpHeight / t / t;
        InitialJumpVelocity = 2f * jumpHeight / t;

        Physics2D.gravity = Vector2.up * _gravity;
    }
}
}
