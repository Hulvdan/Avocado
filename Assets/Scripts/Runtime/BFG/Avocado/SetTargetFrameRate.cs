using UnityEngine;

namespace BFG.Avocado {
public class SetTargetFrameRate : MonoBehaviour {
    [SerializeField]
    int targetFrameRate = 60;

    void Awake() {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFrameRate;
    }

    void Update() {
        if (Application.targetFrameRate != targetFrameRate) {
            Application.targetFrameRate = targetFrameRate;
        }
    }
}
}
