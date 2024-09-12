using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CarAIHandler : MonoBehaviour
{
    public enum AIMode
    {
        followPlayer,
        followWaypoints
    };

    [Header("AI Settings")]
    public AIMode aIMode;
    public float aiMaxSpeed = 16;
    public bool aiIsAvoidingCars = true;
    public int playerNumber;

    [Range(0.0f, 1.0f)]
    public float skillLevel = 1.0f;

    Vector3 targetPosition = Vector3.zero;
    Transform targetTransform = null;
    float aiOriginalMaxSpeed = 0;

    bool isRunningStuckCheck = false;
    bool isFirstTemporaryWaypoint = false;
    int stuckCheckCounter = 0;
    List<Vector2> temporaryWaypoints = new List<Vector2>();
    float angleToTarget = 0;

    Vector2 avoidanceVectorLerped = Vector3.zero;

    [SerializeField]
    AIWaypointNode curWaypoint = null;
    [SerializeField]
    AIWaypointNode prevWaypoint = null;
    [SerializeField]
    AIWaypointNode[] allWaypoints;

    PolygonCollider2D polygonCollider;

    PlayerCarController carController;
    PathFinding pathFinding;

    void Awake()
    {
        carController = GetComponent<PlayerCarController>();     
        allWaypoints = FindObjectsOfType<AIWaypointNode>();

        pathFinding = GetComponent<PathFinding>();

        polygonCollider = GetComponentInChildren<PolygonCollider2D>();

        aiOriginalMaxSpeed = aiMaxSpeed;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetMaxSpeedBasedOnSkillLevel(aiMaxSpeed);
    }

    void FixedUpdate()
    {
        if (GameManager.instance.GetGameState() == GameStates.countdown)
            return;

        Vector2 inputVector = Vector2.zero;

        switch (aIMode)
        {
            case AIMode.followPlayer:
                FollowPlayer();
                break;
            case AIMode.followWaypoints:
                if (temporaryWaypoints.Count == 0)
                    FollowWaypoints();
                else FollowTemporaryWaypoints();
                break;
        }

        inputVector.x = TurnTowardTarget();
        inputVector.y = UseThrottleOrBrake(inputVector.x);

        if (carController.GetVelocityMagnitude() < 0.5f && Mathf.Abs(inputVector.y) > 0.01f && !isRunningStuckCheck)
            StartCoroutine(StuckCheckCo());

        if (stuckCheckCounter >= 4 && !isRunningStuckCheck)
            StartCoroutine(StuckCheckCo());

        carController.SetInputVector(inputVector);
    }

    void FollowPlayer()
    {
        if (targetTransform == null)
            targetTransform = GameObject.FindGameObjectWithTag("Player").transform;

        if (targetTransform != null)
            targetPosition = targetTransform.position;
    }

    void FollowWaypoints()
    {
        if (curWaypoint == null)
        {
            curWaypoint = FindClosestWaypoint();
            prevWaypoint = curWaypoint;       
        }

        if (curWaypoint != null)
        {
            targetPosition = curWaypoint.transform.position;

            float distanceToWaypoint = (targetPosition - transform.position).magnitude;

            if (distanceToWaypoint > 20)
            {
                Vector3 nearestPointOnTheWaypointLine = FindNearestPointOnLine(prevWaypoint.transform.position, curWaypoint.transform.position, transform.position);

                float segments = distanceToWaypoint / 20.0f;

                targetPosition = (targetPosition + nearestPointOnTheWaypointLine * segments) / (segments + 1);

                Debug.DrawLine(transform.position, targetPosition, Color.cyan);

            }

            if (distanceToWaypoint <= curWaypoint.minDistanceToReachWaypoint)
            {
                if (curWaypoint.aiMaxSpeed > 0)
                    SetMaxSpeedBasedOnSkillLevel(curWaypoint.aiMaxSpeed);
                else
                    SetMaxSpeedBasedOnSkillLevel(1000);

                prevWaypoint = curWaypoint;

                curWaypoint = curWaypoint.nextNode[Random.Range(0, curWaypoint.nextNode.Length)];
            }
        }
    }

    void FollowTemporaryWaypoints()
    {
        targetPosition = temporaryWaypoints[0];

        float distanceToWaypoint = (targetPosition - transform.position).magnitude;

        SetMaxSpeedBasedOnSkillLevel(5);

        float minDistanceToReachWaypoint = 1.5f;

        if (!isFirstTemporaryWaypoint)
            minDistanceToReachWaypoint = 3.0f;

        if (distanceToWaypoint <= minDistanceToReachWaypoint)
        {
            temporaryWaypoints.RemoveAt(0);
            isFirstTemporaryWaypoint = false;
        }
    }

    AIWaypointNode FindClosestWaypoint()
    {
        return allWaypoints
            .OrderBy(t => Vector3.Distance(transform.position, t.transform.position))
            .FirstOrDefault();
    }

    float TurnTowardTarget()
    {
        Vector2 vectorToTarget = targetPosition - transform.position;
        vectorToTarget.Normalize();

        if (aiIsAvoidingCars)
            AvoidCars(vectorToTarget, out vectorToTarget);

        angleToTarget = Vector2.SignedAngle(transform.up, vectorToTarget);
        angleToTarget *= -1;

        float steerAmount = angleToTarget / 45.0f;

        steerAmount = Mathf.Clamp(steerAmount, -1.0f, 1.0f);

        return steerAmount;
    }

    float UseThrottleOrBrake(float inputX)
    {
        if (carController.GetVelocityMagnitude() > aiMaxSpeed)
            return 0;

        float reduceSpeedDueToCorner = Mathf.Abs(inputX) / 1.0f;

        float throttle = 1.05f - reduceSpeedDueToCorner * skillLevel;

        if (temporaryWaypoints.Count() != 0)
        {
            if (angleToTarget > 70)
                throttle *= -1;
            else if (angleToTarget < -70)
                throttle *= -1;
            else if (stuckCheckCounter > 3)
                throttle *= -1;
        }

        return throttle;
    }

    void SetMaxSpeedBasedOnSkillLevel(float newSpeed)
    {
        aiMaxSpeed = Mathf.Clamp(newSpeed, 0, aiOriginalMaxSpeed);

        float aiSkillBasedMaximumSpeed = Mathf.Clamp(skillLevel, 0.3f, 1.0f);
        aiMaxSpeed *= aiSkillBasedMaximumSpeed;
    }

    Vector2 FindNearestPointOnLine(Vector2 lineStart, Vector2 lineEnd, Vector2 point)
    {
        Vector2 lineHeadingVector = (lineEnd - lineStart);

        float maxDistance = lineHeadingVector.magnitude;
        lineHeadingVector.Normalize();

        Vector2 lineVectorStartToPoint = point - lineStart;
        float dotProd = Vector2.Dot(lineVectorStartToPoint, lineHeadingVector);

        dotProd = Mathf.Clamp(dotProd, 0f, maxDistance);

        return lineStart + lineHeadingVector * dotProd;
    }

    bool IsCarInFront(out Vector3 position, out Vector3 otherCarRightVector)
    {
        if (carController.isJumping == true)
            polygonCollider.enabled = false;

        RaycastHit2D hit = Physics2D.CircleCast(transform.position + transform.up * 0.5f, 1, transform.up, 12, 1 << LayerMask.NameToLayer("Car"));

        if (carController.isJumping == false)
            polygonCollider.enabled = true;

        if (hit.collider != null)
        {
            Debug.DrawRay(transform.position, transform.up * 12, Color.red);

            position = hit.collider.transform.position;
            otherCarRightVector = hit.collider.transform.right;

            return true;
        }
        else
        {
            Debug.DrawRay(transform.position, transform.up * 12, Color.black);
        }

        position = Vector3.zero;
        otherCarRightVector = Vector3.zero;

        return false;
    }

    void AvoidCars(Vector2 vectorToTarget, out Vector2 newVectorToTarget)
    {
        if (IsCarInFront(out Vector3 otherCarPosition, out Vector3 otherCarRightVector))
        {
            Vector2 avoidanceVector = Vector2.zero;

            avoidanceVector = Vector2.Reflect((otherCarPosition - transform.position).normalized, otherCarRightVector);

            float distanceToTarget = (targetPosition - transform.position).magnitude;

            float driveToTargetInfluence = 6.0f / distanceToTarget;

            driveToTargetInfluence = Mathf.Clamp(driveToTargetInfluence, 0.30f, 1.0f);

            float avoidanceInfluence = 1.0f - driveToTargetInfluence;

            avoidanceVectorLerped = Vector2.Lerp(avoidanceVectorLerped, avoidanceVector, Time.fixedDeltaTime * 4);

            newVectorToTarget = vectorToTarget * driveToTargetInfluence + avoidanceVectorLerped * avoidanceInfluence;
            newVectorToTarget.Normalize();

            Debug.DrawRay(transform.position, avoidanceVector * 10, Color.green);

            Debug.DrawRay(transform.position, newVectorToTarget * 10, Color.yellow);

            return;
        }

        newVectorToTarget = vectorToTarget;
    }

    IEnumerator StuckCheckCo()
    {
        Vector3 initialStuckPosition = transform.position;

        isRunningStuckCheck = true;

        yield return new WaitForSeconds(0.7f);

        if ((transform.position - initialStuckPosition).sqrMagnitude < 3)
        {
            temporaryWaypoints = pathFinding.FindPath(curWaypoint.transform.position);

            if (temporaryWaypoints == null)
                temporaryWaypoints = new List<Vector2>();

            stuckCheckCounter++;

            isFirstTemporaryWaypoint = true;
        }
        else
            stuckCheckCounter = 0;

        isRunningStuckCheck = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("OffTrack"))
        {
            this.transform.position = prevWaypoint.transform.position;
        }
    }
}
