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
    Camera cameraChild;

    Transform _avocado;

    void Start() {
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

        float y;
        if (pos.y - charPos.y > minimalDownDistance) {
            y = charPos.y + minimalDownDistance;
        }
        else {
            y = Mathf.Lerp(pos.y, charPos.y, k);
        }

        transform.position = new Vector3(x, y, pos.z);
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
}
}
