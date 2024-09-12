using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIWaypointNode : MonoBehaviour
{
    [Header("Speed set once we reach the waypoint")]
    public float aiMaxSpeed = 0;

    [Header("This is the waypoint we are going towards, not yet reached")]
    public float minDistanceToReachWaypoint = 5;

    public AIWaypointNode[] nextNode;
}
