using SimplexNoise;
using UnityEngine;

namespace BFG.Avocado {
public class CameraShake : MonoBehaviour {
    [SerializeField]
    AvocadoController controller;

    [SerializeField]
    float shakeDecayPerSecond = 1f;

    [SerializeField]
    [Min(0)]
    float verticalShakeDisplacement = 1f;

    [SerializeField]
    [Min(0)]
    float horizontalShakeDisplacement = 1f;

    [SerializeField]
    [Min(0)]
    float rotationalShakeDisplacement = 1f;

    [SerializeField]
    [Min(0)]
    float fallShakeStartHeight = 3.2f;

    [SerializeField]
    [Min(0)]
    float fallShakeMaxHeight = 6f;

    [SerializeField]
    [Min(0)]
    float timeMultiplier = 100;

    [SerializeField]
    [Min(0)]
    float noiseScale = 100;

    float _currentShake;

    void Start() {
        controller.OnAvocadoGrounded += OnAvocadoGrounded;
    }

    void LateUpdate() {
        var currentShakeSquared = _currentShake * _currentShake;
        var cachedTransform = transform;

        transform.localPosition = new Vector3(
            currentShakeSquared * horizontalShakeDisplacement * (MakeSomeNoise(0) - .5f),
            currentShakeSquared * verticalShakeDisplacement * (MakeSomeNoise(1) - .5f),
            cachedTransform.localPosition.z
        );

        var q = cachedTransform.localRotation;
        q.eulerAngles = new Vector3(
            q.eulerAngles.x,
            q.eulerAngles.y,
            currentShakeSquared * rotationalShakeDisplacement * (MakeSomeNoise(2) - .5f)
        );
        transform.localRotation = q;

        _currentShake = Mathf.Max(0, _currentShake - shakeDecayPerSecond * Time.deltaTime);
    }

    float MakeSomeNoise(int seed) {
        Noise.Seed = seed;
        return Noise.CalcPixel1D((int)(Time.time * timeMultiplier), noiseScale) / 255f;
    }

    void AddShake(float shake) {
        if (shake <= 0) {
            return;
        }

        _currentShake = Mathf.Min(_currentShake + shake, 1f);
    }

    void OnAvocadoGrounded(float height) {
        AddShake((height - fallShakeStartHeight) * fallShakeStartHeight / fallShakeMaxHeight);
    }
}
}
