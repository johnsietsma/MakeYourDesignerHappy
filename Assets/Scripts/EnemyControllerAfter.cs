﻿using UnityEngine;
using UnityEngine.Assertions;

/*
 * Amazing Enemy AI that can Idle or Patrol.
 * This class is used to demonstrate the use of Attributes to change how variables are displayed in the Unity3D editor.
 */
public class EnemyControllerAfter : MonoBehaviour
{
    public enum State { Idle, Patrol, Count };

    // ---- Public variables ----

    // Idle State Variables
    [Header("Idle State")]

    [Tooltip("The time to spend waiting in the Idle state")]
    public float idleWaitTime = 2;

    [Tooltip("The vertical distance travelled when hovering up and down")]
    [Range(0,0.1f)] public float idleHoverDistance = 0.02f;

    // Patrol State Variables
    [Header("Patrol State")]

    [Tooltip("How many seconds to walk in the Patrol state")]
    public float patrolWalkTime = 4;

    [Tooltip("The distance to travel in the direction of travel before changing directions")]
    public float patrolDistance = 6;

    [Tooltip("The speed at which to walk, in world units/second")]
    public float patrolSpeed = 5f;

    // State Machine Variables
    [Header("State Machine")]

    [Tooltip("The State the enemy should start in")]
    public State startState;

    [Tooltip("The current state, displayed for debugging purposes")]
    [Space(20)][ReadOnly] public State currentState;


    // ---- Private Variables ----

    // The direction of the patrol
    private enum PatrolDirection { Right, Up, Left, Down, Count };
    private PatrolDirection currentPatrolDirection = PatrolDirection.Right;

    // The time we entered the current state
    private float stateStartTime;

    // The state machine delegates
    private delegate void UpdateBehaviour();
    private UpdateBehaviour[] behaviours;

	void Start () {
        // Setup the delegates that control the state machine
        behaviours = new UpdateBehaviour[]
        {
            UpdateIdle,
            UpdatePatrol
        };

        // Change to the initial state
        ChangeState(startState);
	}
	
	void Update () {
        // Make sure that the delegate we need actually exists
        // This catches if a new state is added but no delegate is provided for it
        int currentStateIndex = (int)currentState;
        Assert.IsTrue(currentStateIndex < behaviours.Length);

        // Call the delegate for the current state
        behaviours[currentStateIndex]();
	}

    // The Update function for the Idle State
    void UpdateIdle()
    {
        // Move up and down on the spot
        float timeWraped = Time.realtimeSinceStartup % 2; // The time clamped to the range 0-2
        float timePingPong = Mathf.Abs(timeWraped - 1) - 0.5f; // Change the time so it moves between -1 to 1
        float hover = timePingPong * idleHoverDistance; // Calculate the hover distance
        transform.Translate(0, hover, 0);

        // Change state if we've been Idling for long enough
        if(stateStartTime + idleWaitTime < Time.realtimeSinceStartup )
        {
            // Ground the enemy
            Vector3 position = transform.position;
            position.y = 0;
            transform.position = position;

            // Start patrolling
            ChangeState(State.Patrol);
        }
    }

    // The Update function for the Patrol State
    void UpdatePatrol()
    {
        // The normalized direction of travel
        Vector3 patrolNormal = GetPatrolDirectionNormal(currentPatrolDirection);

        // The position we're heading for
        Vector3 goalPosition = patrolNormal * patrolDistance;

        // The vector that moves us to the goal position
        Vector3 goalVector = goalPosition - transform.position;

        // Returns positive if the goal vector is in the same direction as the patrol vector
        if (Vector3.Dot(goalVector, patrolNormal) < 0 )
        {
            // We're past the goal, change direction
            currentPatrolDirection = (PatrolDirection)(((int)currentPatrolDirection + 1) % (int)PatrolDirection.Count);
        }

        // Calculate how far we'll move in this frame
        float moveDistance = patrolSpeed * Time.deltaTime;

        // Move the direction we're patrolling
        transform.Translate(patrolNormal * moveDistance);

        // Change the state if we've bee patrolling for long enough
        if (stateStartTime + patrolWalkTime < Time.realtimeSinceStartup)
        {
            ChangeState(State.Idle);
        }
    }

    // Return a normalized Vector3 which points in the direction of travel
    private Vector3 GetPatrolDirectionNormal( PatrolDirection patrolDirection )
    {
        Vector3 patrolDirectionNormal = Vector3.zero;
        switch(patrolDirection)
        {
            case PatrolDirection.Right: patrolDirectionNormal.x = 1; break;
            case PatrolDirection.Up: patrolDirectionNormal.z = 1; break;
            case PatrolDirection.Left: patrolDirectionNormal.x = -1; break;
            case PatrolDirection.Down: patrolDirectionNormal.z = -1; break;
        }
        return patrolDirectionNormal;
    }

    // Change the state we're in and reset the start time.
    private void ChangeState( State newState )
    {
        stateStartTime = Time.realtimeSinceStartup;
        currentState = newState;
    }


}
