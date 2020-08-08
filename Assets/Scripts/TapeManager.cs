using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the tape mechanics and interacts with the linked monster
/// </summary>
public class TapeManager : MonoBehaviour
{
    
    // Reference to the Monster linked to this tape
    public MonsterAI monsterAI;

    /// <summary>
    /// Destroys the tape
    /// </summary>
    public void PickUp()
    {
        Destroy(this.gameObject);
    }
}
