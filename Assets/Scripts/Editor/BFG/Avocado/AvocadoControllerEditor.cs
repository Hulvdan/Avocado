using BFG.Avocado;
using UnityEditor;

namespace Editor.BFG.Avocado {
[CustomEditor(typeof(AvocadoController))]
public class AvocadoControllerEditor : UnityEditor.Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        var controller = target as AvocadoController;

        var maxVerticalSpeed = controller!.maxVerticalSpeed;
        if (GameManager.InitialJumpVelocity > maxVerticalSpeed) {
            EditorGUILayout.HelpBox(
                $"Max Vertical Speed clamped at \"{maxVerticalSpeed}\", " +
                $"while Initial Jump Velocity equals \"{GameManager.InitialJumpVelocity}\"",
                MessageType.Error
            );
        }

        if (controller.movementSpeedWhileThrowing > controller.movementSpeed) {
            EditorGUILayout.HelpBox(
                "Movement Speed While Throwing should not be bigger than regular Movement Speed",
                MessageType.Warning
            );
        }
    }
}
}
