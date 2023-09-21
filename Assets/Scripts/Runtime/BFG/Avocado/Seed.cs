using UnityEngine;

public class Seed : MonoBehaviour {
    [SerializeField]
    LayerMask setToLayerMask;

    float _startTime;
    bool sett;

    void Start() {
        _startTime = Time.time;
    }

    void Update() {
        if (!sett && Time.time - _startTime > .5f) {
            gameObject.layer = LayerMask.NameToLayer("Terrain");
            sett = true;
        }
    }
}
