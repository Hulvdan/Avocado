using UnityEngine;

public class Seed : MonoBehaviour {
    [SerializeField]
    GameObject trigger;

    [SerializeField]
    float unpickableDuration = .5f;

    float _spawnedAt;

    bool _triggerWasActivated;

    public void Reset() {
        _spawnedAt = Time.time;
        _triggerWasActivated = false;
        trigger.SetActive(false);
    }

    void Start() {
        Reset();
    }

    void Update() {
        if (!_triggerWasActivated && Time.time - _spawnedAt > unpickableDuration) {
            _triggerWasActivated = true;
            trigger.SetActive(true);
        }
    }
}
