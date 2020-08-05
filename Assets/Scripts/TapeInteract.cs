﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/// <summary>
/// This script should be added on the PlayerControler Object or the game manager.
/// Uses raycasts to see what's in front of the player, to be interacted with.
/// Shows tooltip if Player can interact with object
/// 
/// Requires: The tape object tag to be set to "Tape"
/// </summary>


public class TapeInteract : MonoBehaviour
{
    [Tooltip("Add the playerObject here")]
    [SerializeField]
    private GameObject playerObject = default;

    [Tooltip("Set the distance of the raycast for interacting with tapes(Should be arms lenght)")]
    [SerializeField]
    private float distanceToInteract = default;

    [SerializeField]
    private GameObject pressEToolTip = default;

    [SerializeField] private PlayerInventory playerInventory = default;

    private float playerReach = 1;

    private RaycastHit hit;
    private Vector3 playerForwardDirection;

    /// <summary>
    /// Whether an interactable object is in range
    /// </summary>
    private bool isInteractionInRange;

    // Start is called before the first frame update
    void Start()
    {
        if (pressEToolTip == null)
        {
            //Look up Canvas, and then the pressE Text/Game Object 
            pressEToolTip = GameObject.Find("Canvas").transform.Find("PressE").gameObject;

        }

        pressEToolTip.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        ShootRaycast();
    }

    void ShootRaycast()
    {
        //get player forward direction
        playerForwardDirection = playerObject.transform.forward;

        //Cast a box ray in front of the player too look for tapes to interact
        bool found = Physics.BoxCast(playerObject.transform.position, new Vector3(playerReach, playerReach), playerForwardDirection, out hit, Quaternion.identity, distanceToInteract);
        isInteractionInRange = found;

        // object it by raycast is stored in the field "hit"

        if (found)
        {
            if (pressEToolTip.activeSelf == false)
                pressEToolTip.SetActive(true);

        }
        else
        {
            if (pressEToolTip.activeSelf == true)
                pressEToolTip.SetActive(false);
        }
    }

    private void InteractVHSPlayer()
    {
        Debug.Log("Interact with VHS player");

        VHSPlayerManager vhsPlayerManager = hit.collider.gameObject.GetComponent<VHSPlayerManager>();

        // If the player has a tape in hands
        if (playerInventory.HasTapeInHands)
        {
            vhsPlayerManager.Interact(playerInventory.MonsterLinkedToTape);
        }
    }

    private void InteractTape()
    {
        Debug.Log("Interact with tape");
        // Get the monster linked to the tape
        MonsterAI linkedMonster = hit.collider.gameObject.GetComponent<TapeManager>().monsterAI;

        // If the player hasn't a tape in hands yet
        if (!playerInventory.HasTapeInHands)
        {
            // TODO: Here we have to destroy the tape, so that we cannot interact with it anymore
            playerInventory.PickupTape(linkedMonster);
        }
    }

    /// <summary>
    /// This can be disabled, used to just show how the player finds objects and
    /// where is the player looking in a given time.
    /// </summary>
    void OnDrawGizmos()
    {

        Gizmos.DrawRay(playerObject.transform.position, playerForwardDirection * hit.distance);

        Gizmos.DrawWireCube(playerObject.transform.position + playerForwardDirection * hit.distance, new Vector3(playerReach, playerReach));
    }

    /// <summary>
    /// Listener to the Interaction action in input system
    /// </summary>
    /// <param name="ctx"></param>
    public void Interaction(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && isInteractionInRange)
        {
            if (hit.collider.gameObject.CompareTag("Tape"))
            {
                InteractTape();
            }
            else if (hit.collider.gameObject.CompareTag("VHSPlayer"))
            {
                InteractVHSPlayer();
            }
        }
    }
}
