using UnityEngine;

namespace BFG.Avocado {
[RequireComponent(typeof(Camera))]
public class BetterCamera : MonoBehaviour {
    [SerializeField]
    AvocadoController controller;

    [SerializeField]
    [Min(0)]
    float followingSpeed = 1f;

    [SerializeField]
    [Min(0)]
    float horizontalDeadZone = 1f;

    Transform _avocado;
    Camera _camera;

    void Start() {
        _camera = GetComponent<Camera>();
        _avocado = controller.transform;
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

        transform.position = new Vector3(x, Mathf.Lerp(pos.y, charPos.y, k), pos.z);
    }

    void OnDrawGizmos() {
        _camera = GetComponent<Camera>();

        Gizmos.color = Color.cyan;
        var pos = transform.position;
        var size = _camera.orthographicSize;
        Gizmos.DrawLine(
            new Vector3(pos.x - horizontalDeadZone, pos.y - size, pos.z),
            new Vector3(pos.x - horizontalDeadZone, pos.y + size, pos.z)
        );
        Gizmos.DrawLine(
            new Vector3(pos.x + horizontalDeadZone, pos.y - size, pos.z),
            new Vector3(pos.x + horizontalDeadZone, pos.y + size, pos.z)
        );
    }
}
}
