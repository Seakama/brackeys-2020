using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    [SerializeField] private Animator animator = default;

    private string sceneNameToLoad;

    public void FadeToScene(string name)
    {
        sceneNameToLoad = name;
        animator.SetTrigger("FadeOut");
    }

    public void OnFadeToSceneComplete()
    {
        SceneManager.LoadSceneAsync(sceneNameToLoad, LoadSceneMode.Single);
    }
}
