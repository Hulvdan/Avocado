using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace BFG.Templates {
//  Namespace Properties ------------------------------

//  Class Attributes ----------------------------------

/// <summary>
///     Replace with comments...
/// </summary>
[Category("BFG.Avocado")]
public class TemplateComponentPlayModeTest {
    //  Fields ----------------------------------------
    GameObject _parentGameObject;

    //  Initialization --------------------------------

    /// <summary>
    ///     Optional. Called before every [Test] method
    /// </summary>
    [SetUp]
    public void Setup() {
        // Add something as needed...
    }

    /// <summary>
    ///     Optional. Called after every [Test] method
    /// </summary>
    [TearDown]
    public void TearDown() {
        if (_parentGameObject != null) {
            if (Application.isPlaying) {
                // Unity prefers this while playing
                Object.Destroy(_parentGameObject);
            }
            else {
                // Unity prefers this while NOT playing
                Object.DestroyImmediate(_parentGameObject, false);
            }

            _parentGameObject = null;
        }
    }

    //  Methods ---------------------------------------

    /// <summary>
    ///     A [Test] behaves as an ordinary method
    /// </summary>
    [Test]
    public void SamplePublicText_GetValueIsExpected_WhenSet() {
        // Arrange
        _parentGameObject = new GameObject();
        var templateComponent = _parentGameObject.AddComponent<TemplateComponent>();
        var text = "MyText";

        // Act
        templateComponent.SamplePublicText = text;

        //  -----------------------------------------------
        // NOTE: This is a silly demo. It offers low value
        //       to unit test a set/get like this.
        //  -----------------------------------------------

        // Assert
        Assert.AreEqual(templateComponent.SamplePublicText, text);
    }

    /// <summary>
    /// A [UnityTest] behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    /// </summary>
    /// <returns></returns>
    [UnityTest]
    public IEnumerator SamplePublicText_GetValueIsExpected_1FrameAfterWhenSet() {
        // Arrange
        _parentGameObject = new GameObject();
        var templateComponent = _parentGameObject.AddComponent<TemplateComponent>();
        var text = "MyText";

        // Act
        templateComponent.SamplePublicText = text;

        //  -----------------------------------------------
        // NOTE: This is a silly demo. There is no reason
        //		 to skip a frame here.
        //  -----------------------------------------------

        // Use yield to skip a frame.
        yield return null;

        // Assert
        Assert.AreEqual(templateComponent.SamplePublicText, text);
    }
}
}
