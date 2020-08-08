using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;
using System;

public class MonsterAI : MonoBehaviour
{
    // Reference to the NavMeshAgent component
    [SerializeField] private NavMeshAgent navMeshAgent = default;
    // Patrolling path of the monster
    [SerializeField] private MonsterPath path = default;
    // Current status of the monster ----- Visible in inspector just for debugging, after debugging we can hide it in inspector and set its default value to patrolling
    [SerializeField] private MonsterStatus status = default;
    // Minimum distance before it changes node destination
    [SerializeField] private float minChangeNodeDistance = default;

    // Maximum distance to attack the target
    [SerializeField] private float maxAttackDistance = default;
    // Target to pursue, when visible
    [SerializeField] private GameObject target = default;
    // Maximum vision angle of the monster (if the target is inside the angle, then pursue it, else keep patrolling)
    [SerializeField] private float maxVisionAngle = default;
    // Determines if the monster has a valid destination
    private bool hasDestination;

    /// <summary>
    /// Reference to the animator
    /// </summary>
    [SerializeField]
    private Animator animator = default;

    /// <summary>
    /// Reference to the global audio
    /// </summary>
    [SerializeField]
    private GlobalAudio globalAudio = default;

    private MonsterStatus Status { get => status;
        set {
            
            // pursue -> patrol
            if (status == MonsterStatus.Pursuing && value == MonsterStatus.Patrolling)
                globalAudio.MonsterStopsChasing();

            // patrol -> pursue
            if (status == MonsterStatus.Patrolling && value == MonsterStatus.Pursuing)
                globalAudio.CueMonsterChase();

            status = value; 
        }
    }

    // Called whenever we interact with the linked tape
    public void OnTapeInteract()
    {
        // Just for Debug, here we need the dying animation
        Destroy(gameObject);
    }

    private void Update()
    {
        if (Status != MonsterStatus.Attacking)
        {
            CheckTargetVisibility();

            if (Status == MonsterStatus.Patrolling)
            {
                Patrol();
            }
            else if (Status == MonsterStatus.Pursuing)
            {
                Pursue();
            }
        }
        else
        {
        }
    }

    /// <summary>
    /// Checks if the target is inside the view cone
    /// </summary>
    private void CheckTargetVisibility()
    {
        // If there is a target
        if (target)
        {
            // Calculate the direction between the monster and the target
            Vector3 direction = target.transform.position - transform.position;

            // If the angle between the target and the monster is within the maxVisionAngle, it means that the target is visible by the monster
            if (Vector3.Angle(transform.forward, direction) < maxVisionAngle)
            {
                // MAYBE: Add timer here if we want to "delay" the pursuing process to give the player some time to hide before being pursued

                Status = MonsterStatus.Pursuing;
            }
            else
            {
                // MAYBE: Add timer here if we want to "delay" the transition between pursuing to patrolling state so that the monster tries to look around to see if there is the player (or some other mechanic)

                Status = MonsterStatus.Patrolling;
            }
        }
        else
        {
            // If there is no target, keep patrolling
            Status = MonsterStatus.Patrolling;
        }
    }

    /// <summary>
    /// Manages the patrol state
    /// </summary>
    private void Patrol()
    {
        // If the monster hasn't got a desination, then find the nearest node and set it as the monster's destination
        if (!hasDestination)
        {

            int index = FindNearestPathNodeIndex();

            // If the index is withing the array bounds
            if (index < path.PathNodes.Length)
            {
                SetDestinationNodeIndex(index);
                hasDestination = true;
            }
        }
        // Else, if the distance between the destination is smaller than minChangeNodeDistance, then go to the next node
        else if (navMeshAgent.remainingDistance < minChangeNodeDistance)
        {
            SetDestinationNodeIndex(path.NextPathNode);
        }
    }

    /// <summary>
    /// Manages the pursue state
    /// </summary>
    private void Pursue()
    {
        // If the monster has a target
        if (target)
        {
            // Update the destination to the position of the target
            navMeshAgent.SetDestination(target.transform.position);

            // If the target is close enough to attack it, change the state to attacking
            if (navMeshAgent.remainingDistance <= maxAttackDistance)
            {
                Attack();
            }
        }
        // Else keep patrolling (we could only reach this part of code if the target got destroyed while pursuing)
        else
        {
            Status = MonsterStatus.Patrolling;
            hasDestination = false;
        }
    }

    /// <summary>
    /// Attacks the target and plays the attack animation
    /// </summary>
    private void Attack()
    {
        // Stop the monster
        navMeshAgent.isStopped = true;

        // Set the status to Attacking
        Status = MonsterStatus.Attacking;

        // TODO: Add attack mechanic and animation (at the end of the animation, set the Status back to Patrolling or Pursuing)
        animator.SetTrigger("Attack");

        // TODO: Just for debug (Call the method the animation is completed)
        //StartCoroutine(OnAttackCompletedDebug());
        //OnAttackCompleted();
    }

    private IEnumerator OnAttackCompletedDebug()
    {
        yield return new WaitForSeconds(1f);

        // Remove the stop state
        navMeshAgent.isStopped = false;
        // Set the status to patrolling (pursue would also be ok)
        Status = MonsterStatus.Patrolling;
        // Set the destination to null, so that the monster searches for the close node in the path
        hasDestination = false;
    }

    /// <summary>
    /// Method called whenever an attack animation has been completed
    /// </summary>
    public void OnAttackCompleted()
    {
        // Remove the stop state
        navMeshAgent.isStopped = false;
        // Set the status to patrolling (pursue would also be ok)
        Status = MonsterStatus.Patrolling;
        // Set the destination to null, so that the monster searches for the close node in the path
        hasDestination = false;

        // player takes (fatal) damage
        target.GetComponent<PlayerHealth>().TakeDamage();
    }

    /// <summary>
    /// Sets the destination of the monster to the position of the node at the given index
    /// </summary>
    /// <param name="index">The index of the node in the path</param>
    private void SetDestinationNodeIndex(int index)
    {
        navMeshAgent.SetDestination(path.PathNodes[index].Position.position);
        // Increase by one the NextPathNode and if it exceeds the array bounds, go back to 0 (modulo operation)
        path.NextPathNode = (index + 1) % path.PathNodes.Length;
    }

    /// <summary>
    /// Finds the nearest node to the monster and returns its index
    /// </summary>
    /// <returns></returns>
    private int FindNearestPathNodeIndex()
    {
        // Cache the minDistance to speed up the process
        float minDistance = int.MaxValue;
        int nearestIndex = 0;

        for (int i = 1; i < path.PathNodes.Length; ++i)
        {
            // Calculate the sqr (faster) distance between the node and the monster's position and see if it is smaller than the minDistance
            if ((transform.position - path.PathNodes[i].Position.position).sqrMagnitude < minDistance)
            {
                nearestIndex = i;
                minDistance = (transform.position - path.PathNodes[nearestIndex].Position.position).sqrMagnitude;
            }
        }

        return nearestIndex;
    }

    public void RewindDeath()
    {
        Destroy(this.gameObject);
    }
}

public enum MonsterStatus
{
    Idle,
    Patrolling,
    Pursuing,
    Attacking,
}

[System.Serializable]
public class MonsterPath
{
    // The nodes in the path
    public PathNode[] PathNodes;
    // Points to the next node to go
    public int NextPathNode;
}

[System.Serializable]
public class PathNode
{
    // Stores the transform of node GameObject
    public Transform Position;
}
