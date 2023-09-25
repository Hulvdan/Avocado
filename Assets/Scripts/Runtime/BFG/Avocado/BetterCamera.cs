using SimplexNoise;
using UnityEngine;

namespace BFG.Avocado {
public class BetterCamera : MonoBehaviour {
    [SerializeField]
    AvocadoController controller;

    [SerializeField]
    [Min(0)]
    float followingSpeed = 1f;

    [SerializeField]
    [Min(0)]
    float horizontalDeadZone = 1f;

    [SerializeField]
    float minimalDownDistance = 1f;

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
    Camera cameraChild;

    [SerializeField]
    [Min(0)]
    float timeMultiplier = 100;

    [SerializeField]
    [Min(0)]
    float noiseScale = 100;

    [SerializeField]
    [Range(0, 1)]
    float currentShake;

    Transform _avocado;
    Transform _cameraChildTransform;

    void Start() {
        _avocado = controller.transform;
        _cameraChildTransform = cameraChild.transform;

        _avocado.GetComponent<AvocadoController>().OnAvocadoGrounded += OnAvocadoGrounded;
    }

    void LateUpdate() {
        var k = followingSpeed * Time.deltaTime;
        var pos = transform.position;
        var charPos = _avocado.position;

        var x = pos.x;
        if (charPos.x > pos.x + horizontalDeadZone) {
            x = Mathf.Lerp(pos.x + horizontalDeadZone, charPos.x, k) - horizontalDeadZone;
        }
        else if (charPos.x < pos.x - horizontalDeadZone) {
            x = Mathf.Lerp(pos.x - horizontalDeadZone, charPos.x, k) + horizontalDeadZone;
        }

        float y;
        if (pos.y - charPos.y > minimalDownDistance) {
            y = charPos.y + minimalDownDistance;
        }
        else {
            y = Mathf.Lerp(pos.y, charPos.y, k);
        }

        transform.position = new Vector3(x, y, pos.z);

        var currentShakeSquared = currentShake * currentShake;
        _cameraChildTransform.localPosition = new Vector3(
            currentShakeSquared * horizontalShakeDisplacement * (MakeSomeNoise(0) - .5f),
            currentShakeSquared * verticalShakeDisplacement * (MakeSomeNoise(1) - .5f),
            _cameraChildTransform.localPosition.z
        );

        var q = _cameraChildTransform.localRotation;
        q.eulerAngles = new Vector3(
            q.eulerAngles.x,
            q.eulerAngles.y,
            currentShakeSquared * rotationalShakeDisplacement * (MakeSomeNoise(2) - .5f)
        );
        _cameraChildTransform.localRotation = q;

        currentShake = Mathf.Max(0, currentShake - shakeDecayPerSecond * Time.deltaTime);
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
        var pos = transform.position;
        var size = cameraChild.orthographicSize;
        Gizmos.DrawLine(
            new Vector3(pos.x - horizontalDeadZone, pos.y - size, pos.z),
            new Vector3(pos.x - horizontalDeadZone, pos.y + size, pos.z)
        );
        Gizmos.DrawLine(
            new Vector3(pos.x + horizontalDeadZone, pos.y - size, pos.z),
            new Vector3(pos.x + horizontalDeadZone, pos.y + size, pos.z)
        );

        Gizmos.DrawLine(
            new Vector3(pos.x - horizontalDeadZone, pos.y - minimalDownDistance, pos.z),
            new Vector3(pos.x + horizontalDeadZone, pos.y - minimalDownDistance, pos.z)
        );
    }

    float MakeSomeNoise(int seed) {
        Noise.Seed = seed;
        return Noise.CalcPixel1D((int)(Time.time * timeMultiplier), noiseScale) / 255f;
    }

    void AddShake(float shake) {
        if (shake <= 0) {
            return;
        }

        currentShake = Mathf.Min(currentShake + shake, 1f);
    }

    void OnAvocadoGrounded(float height) {
        AddShake((height - fallShakeStartHeight) * fallShakeStartHeight / fallShakeMaxHeight);
    }
}
}
