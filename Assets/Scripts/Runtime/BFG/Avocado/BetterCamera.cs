using UnityEngine;

namespace BFG.Avocado {
public class BetterCamera : MonoBehaviour {
    [SerializeField]
    AvocadoController controller;

    [SerializeField]
    [Min(0)]
    float followingSpeed = 1f;

    Transform _character;

    void Start() {
        _character = controller.transform;
    }

    void LateUpdate() {
        var k = followingSpeed * Time.deltaTime;
        var pos = transform.position;
        var charPos = _character.position;
        transform.position = new Vector3(
            Mathf.Lerp(pos.x, charPos.x, k),
            Mathf.Lerp(pos.y, charPos.y, k),
            pos.z
        );
    }
}
}
