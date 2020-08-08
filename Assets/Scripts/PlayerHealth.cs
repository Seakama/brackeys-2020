using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the health of the player
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    /// <summary>
    /// Reference to the game manager
    /// </summary>
    [SerializeField]
    private GameManager gameManager = default;

    /// <summary>
    /// Damage is fatal
    /// </summary>
    public void TakeDamage()
    {
        // death animation
        // defeat screen
        gameManager.Defeat();
    }
}
