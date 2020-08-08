using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This Script keeps track of level progress. 
/// Checks if the player has won.
/// </summary>
public class GameManager : MonoBehaviour
{

    [Tooltip("Set this equal to how many enemies are gonna spawn in the level")]
    [SerializeField]
    int maxEnemies = default;

    [Tooltip("A text object that displays when winning the level")]
    [SerializeField]
    private GameObject victoryScreen = default;

    [Tooltip("An UI object that displays when defeated")]
    [SerializeField]
    private GameObject defeatScreen = default;

    [Tooltip("An reference to the SceneTransitionManager")]
    [SerializeField]
    private SceneTransitionManager sceneTransitionManager = default;

    /// <summary>
    /// When the player dies
    /// </summary>
    public void Defeat()
    {
        defeatScreen.SetActive(true);
    }

    int enemiesLeft;

    private void Start() 
    {
        enemiesLeft = maxEnemies;    
    }

    public int GetMaxEnemies() {
        return maxEnemies;
    }
    public int GetEnemiesLeft() {
        return enemiesLeft;
    }

    public void IncrementEnemyCounter() {
        enemiesLeft++;
        if (enemiesLeft > maxEnemies) enemiesLeft = maxEnemies;



    }

    public void DecrementEnemyCounter() {
        enemiesLeft--;
        if (enemiesLeft < 0) enemiesLeft = 0;

        if(enemiesLeft == 0) 
        {
            //Win condition
            //1.Stop players movement.
            //2.Victory screen UI
            //3.Play Victory music
            victoryScreen.SetActive(true);
        }
    }

    /// <summary>
    /// Back button event listener
    /// </summary>
    public void BackButton()
    {
        //SceneManager.LoadScene("Main Menu");
        sceneTransitionManager.FadeToScene("Main Menu");
    }

    /// <summary>
    /// Replay button listener
    /// </summary>
    public void ReplayButton()
    {
        //SceneManager.LoadScene("LevelTest");
        sceneTransitionManager.FadeToScene("LevelTest");
    }
}
