using UnityEngine;
using UnityEngine.Assertions;

/*
 * Amazing Enemy AI that can Idle or Patrol.
 * This class is used to demonstrate the use of Attributes to change how variables are displayed in the Unity3D editor.
 */
public class EnemyController : MonoBehaviour
{
    public enum State { Idle, Patrol };

    [SerializeField]
    private float IdleWaitTime = 2;
    public float IdleHoverDistance = 0.02f;

    public float PatrolWalkTime = 4;
    public float PatrolDistance = 6;
    public float PatrolSpeed = 5f;

    private enum PatrolDirection { Left, Right };
    private PatrolDirection patrolDirection;

    [ReadOnly]
    public State currentState;

    private float stateStartTime;

    private delegate void UpdateBehaviour();
    private UpdateBehaviour[] behaviours;

	void Start () {
        behaviours = new UpdateBehaviour[]
        {
            UpdateIdle,
            UpdatePatrol
        };

        ChangeState(State.Idle);
	}
	
	void Update () {
        int currentStateIndex = (int)currentState;
        Assert.IsTrue(currentStateIndex < behaviours.Length);

        behaviours[currentStateIndex]();
	}

    void UpdateIdle()
    {
        // Move up and down on the spot
        float timeWraped = Time.realtimeSinceStartup % 2; // The time clamped to the range 0-2
        float timePingPong = Mathf.Abs(timeWraped - 1) - 0.5f; // Change the time so it moves between -1 to 1
        float hover = timePingPong * IdleHoverDistance; // Calculate the hover distance
        transform.Translate(0, hover, 0);

        // Change state if we've been Idling for long enough
        if(stateStartTime + IdleWaitTime < Time.realtimeSinceStartup )
        {
            // Ground the enemy
            Vector3 position = transform.position;
            position.y = 0;
            transform.position = position;

            // Start patrolling
            ChangeState(State.Patrol);
        }
    }

    void UpdatePatrol()
    {
        // Reverse direction if we've moved far enough from the origin
        if (transform.position.magnitude > PatrolDistance)
        {
            patrolDirection = patrolDirection == PatrolDirection.Left ? PatrolDirection.Right : PatrolDirection.Left;
        }

        // Calculate how far we'll move in this frame
        float moveDistance = PatrolSpeed * Time.deltaTime;

        // Move the direction we're patrolling
        if ( patrolDirection==PatrolDirection.Left)
        {
            transform.Translate(-moveDistance, 0, 0);
        }
        else if(patrolDirection == PatrolDirection.Right)
        {
            transform.Translate(moveDistance, 0, 0);
        }

        // Change the state if we've bee patrolling for long enough
        if (stateStartTime + PatrolWalkTime < Time.realtimeSinceStartup)
        {
            ChangeState(State.Idle);
        }
    }

    private void ChangeState( State newState )
    {
        stateStartTime = Time.realtimeSinceStartup;
        currentState = newState;
    }


}
