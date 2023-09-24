using BFG.Avocado;
using UnityEditor;

namespace Editor.BFG.Avocado {
[CustomEditor(typeof(AvocadoController))]
public class AvocadoControllerEditor : UnityEditor.Editor {
    public override void OnInspectorGUI() {
        var maxVerticalSpeed = (target as AvocadoController).maxVerticalSpeed;
        if (GameManager.InitialJumpVelocity > maxVerticalSpeed) {
            EditorGUILayout.HelpBox(
                $"Max Vertical Speed clamped at \"{maxVerticalSpeed}\", " +
                $"while Initial Jump Velocity equals \"{GameManager.InitialJumpVelocity}\"",
                MessageType.Error
            );
        }

        base.OnInspectorGUI();
    }
}
}
