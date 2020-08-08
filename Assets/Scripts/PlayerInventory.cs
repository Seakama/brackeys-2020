using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Basic Inventory System
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    // Wheter the player has a tape in hands
    public bool HasTapeInHands { get; private set; }
    // Reference to the monster linked to the tape that we have in hands
    public MonsterAI MonsterLinkedToTape { get; private set; }

    private PlayerAudio playerAudio;

    /// <summary>
    /// Reference to the text of having picked up a tape
    /// </summary>
    [SerializeField]
    private GameObject pickedUpMessage = default;

    /// <summary>
    /// Reference to the objective text
    /// </summary>
    [SerializeField]
    private GameObject objectiveMessage = default;

    private void Start()
    {
        playerAudio = GetComponent<PlayerAudio>();
    }

    /// <summary>
    /// Sets HasTapeInHands to true and stores the reference to the monster
    /// </summary>
    /// <param name="monster">The monster linked to the tape</param>
    public void PickupTape(MonsterAI monster)
    {
        HasTapeInHands = true;
        MonsterLinkedToTape = monster;
        
        // play sound
        playerAudio.Pickup.Play();

        // Tape message displays
        pickedUpMessage.SetActive(true);
        StartCoroutine(DisabledIn(pickedUpMessage, 10f));

        // Objective object
        objectiveMessage.GetComponent<Text>().text = "Objective: Find a VHS reader";
    }

    /// <summary>
    /// Coroutine that disables an object after some time
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    private IEnumerator DisabledIn(GameObject gameObject, float duration)
    {
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);
    }
}
