using UnityEngine;

public class Seed : MonoBehaviour {
    bool _physicsLayerWasChanged;

    float _spawnedAt;

    public void Reset() {
        _spawnedAt = Time.time;
        _physicsLayerWasChanged = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    void Start() {
        Reset();
    }

    void Update() {
        if (!_physicsLayerWasChanged && Time.time - _spawnedAt > .5f) {
            gameObject.layer = LayerMask.NameToLayer("Terrain");
            _physicsLayerWasChanged = true;
        }
    }
}
