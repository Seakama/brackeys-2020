﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the functionalities of the VHS player
/// </summary>
public class VHSPlayerManager : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager = default;

    /// <summary>
    /// Called whenever the player interacts with the VHSPlayer
    /// </summary>
    /// <param name="monster">The monster linked to the tape given to the VHSPlayer</param>
    public void Interact(MonsterAI monster)
    {
        if (monster)
        {
            // NOTE: Just for Debug. Change with a die animation
            gameManager.DecrementEnemyCounter();
            Destroy(monster.gameObject);
        }
    }
}