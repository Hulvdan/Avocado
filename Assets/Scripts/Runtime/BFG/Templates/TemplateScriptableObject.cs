using UnityEngine;

namespace BFG.Templates {
//  Namespace Properties ------------------------------

//  Class Attributes ----------------------------------

/// <summary>
///     Replace with comments...
/// </summary>
[CreateAssetMenu(
    fileName = "TemplateScriptableObject",
    menuName = "[Avocado]/TemplateScriptableObject",
    order = 0)]
public class TemplateScriptableObject : ScriptableObject {
    //  Fields ----------------------------------------
    [SerializeField]
    string _samplePublicText;

    //  Properties ------------------------------------
    public string SamplePublicText {
        get => _samplePublicText;
        set => _samplePublicText = value;
    }

    //  Unity Methods ---------------------------------
    protected void OnEnable() {
    }
}
}
