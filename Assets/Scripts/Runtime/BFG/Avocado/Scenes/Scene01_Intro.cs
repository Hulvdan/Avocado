using UnityEngine;

namespace BFG.Avocado.Scenes {
//  Namespace Properties ------------------------------

//  Class Attributes ----------------------------------

/// <summary>
///     Replace with comments...
/// </summary>
public class Scene01_Intro : MonoBehaviour {
    //  Fields ----------------------------------------
    [SerializeField]
    string _samplePublicText;
    //  Events ----------------------------------------

    //  Properties ------------------------------------
    public string SamplePublicText {
        get => _samplePublicText;
        set => _samplePublicText = value;
    }

    //  Unity Methods ---------------------------------
    protected void Start() {
        Debug.Log($"{GetType().Name}.Start()");
    }

    protected void Update() {
    }

    //  Methods ---------------------------------------
    public string SamplePublicMethod(string message) {
        return message;
    }

    //  Event Handlers --------------------------------
    public void Target_OnCompleted(string message) {
    }
}
}
