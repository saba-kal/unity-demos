using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class RobotLeg
{

	public Transform ObjectToFollow;

	public bool IsMoving;

	//Used for moving the follow point.
	public Vector3 InitialWorldPosition;
	public Vector3 InitialLocalPosition;
	public Vector3 MoveToTargetPosition;

	//Used for animating arc movement.
	public float AnimationStartTime;
	public Vector3 AnimationArcCenter;
	public Vector3 AnimationStartPoint;

	//Used for drawing gizmos
	public Vector3 RaycastFromPosition;
	public Vector3 Direction;
}
