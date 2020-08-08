using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Decides what happens when buttons are clicked on the main menu
/// </summary>
public class MainMenu : MonoBehaviour
{

    /// <summary>
    /// String of the name of the scene containing the scene.
    /// that scene must be in the build settings.
    /// </summary>
    [Tooltip("Name of the scene of the level")]
    [SerializeField]
    private string sceneName = "LevelTest";

    /// <summary>
    /// Empty containing the main menu buttons
    /// </summary>
    [SerializeField]
    private GameObject mainButtons = default;

    /// <summary>
    /// Empty containing the credits
    /// </summary>
    [SerializeField]
    private GameObject credits = default;

    /// <summary>
    /// Play button object
    /// </summary>
    [SerializeField]
    private GameObject playButton = default;
    
    /// <summary>
    /// Scene transition manager reference
    /// </summary>
    [SerializeField]
    private SceneTransitionManager sceneTransitionManager = default;

    /// <summary>
    /// Main camera (for bg music)
    /// </summary>
    [SerializeField]
    private GameObject mainCamera = default;

    public void GoToLevel()
    {
        // play a sound for pressing the start button
        playButton.GetComponent<FMODUnity.StudioEventEmitter>().Play();
        
        // send a stop message to the bg music
        mainCamera.GetComponent<FMODUnity.StudioEventEmitter>().Stop();

        sceneTransitionManager.FadeToScene(sceneName);
    }

    public void ShowCredits()
    {
        credits.SetActive(true);
        mainButtons.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void BackToMain()
    {
        credits.SetActive(false);
        mainButtons.SetActive(true);
    }
}
