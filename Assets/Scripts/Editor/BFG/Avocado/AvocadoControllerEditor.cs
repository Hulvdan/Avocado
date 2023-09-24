using BFG.Avocado;
using UnityEditor;

namespace Editor.BFG.Avocado {
[CustomEditor(typeof(AvocadoController))]
public class AvocadoControllerEditor : UnityEditor.Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        (target as AvocadoController).InitialJumpVelocity = GameManager.InitialJumpVelocity;
    }
}
}
