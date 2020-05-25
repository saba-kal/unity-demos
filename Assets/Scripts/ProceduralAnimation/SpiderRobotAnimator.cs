using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderRobotAnimator : MonoBehaviour
{

	public RobotLeg Leg1;
	public RobotLeg Leg2;
	public RobotLeg Leg3;
	public RobotLeg Leg4;

	public float MaxLegMoveDistance = 10;
	public float LegRayCastPositionOffset = 5;
	public float LegMoveSpeed = 3;
	public float LegAnimationArcHeight = 10;

	void Start()
	{
		InitiateLeg(Leg1, new Vector3(0, 0, 5));
		InitiateLeg(Leg2, new Vector3(0, 0, 0));
		InitiateLeg(Leg3, new Vector3(0, 0, 0));
		InitiateLeg(Leg4, new Vector3(0, 0, -5));
	}

	void Update()
	{
		ProcessLegMovement(Leg1);
		ProcessLegMovement(Leg2);
		ProcessLegMovement(Leg3);
		ProcessLegMovement(Leg4);
	}

	private void InitiateLeg(RobotLeg leg, Vector3 offset)
	{
		MoveLegToGround(leg);
		leg.InitialWorldPosition = leg.ObjectToFollow.position + offset;
		leg.InitialLocalPosition = leg.ObjectToFollow.localPosition;
	}

	private void ProcessLegMovement(RobotLeg leg)
	{
		if (leg.IsMoving && ContinueMovingLeg(leg))
		{
			return;
		}

		//Check if leg needs to move.
		var legStartingWorldPosition = transform.TransformPoint(leg.InitialLocalPosition);
		if (Vector3.Distance(leg.ObjectToFollow.position, legStartingWorldPosition) > MaxLegMoveDistance &&
			GetNumberOfLegsCurrentlyMoving() < 2)
		{
			InitiateLegMovement(leg);
		}
		else
		{
			leg.ObjectToFollow.position = leg.InitialWorldPosition;
		}

		MoveLegToGround(leg);
	}

	private void InitiateLegMovement(RobotLeg leg)
	{
		//Set up animation.
		leg.IsMoving = true;
		leg.AnimationStartTime = Time.time;

		//Get the target point leg needs top move to.
		var direction = -(leg.ObjectToFollow.localPosition - leg.InitialLocalPosition).normalized;
		leg.Direction = direction;
		var targetPoint = leg.InitialLocalPosition + direction * (MaxLegMoveDistance * 0.8f);

		//Adjust target position for elevation changes.
		var targetWorldPos = transform.TransformPoint(targetPoint);
		leg.RaycastFromPosition = targetWorldPos + new Vector3(0, LegRayCastPositionOffset, 0);
		if (Physics.Raycast(leg.RaycastFromPosition, -Vector3.up, out var hit))
		{
			targetPoint = transform.InverseTransformPoint(hit.point);
		}

		//Store new target position. Reset leg's initial position.
		leg.MoveToTargetPosition = targetPoint;
		leg.InitialWorldPosition = leg.ObjectToFollow.position;

		//Move leg.
		leg.AnimationArcCenter = (leg.ObjectToFollow.localPosition + targetPoint) * 0.5F;
		leg.AnimationArcCenter -= new Vector3(0, LegAnimationArcHeight, 0); // move the center a bit downwards to make the arc vertical
		leg.AnimationStartPoint = leg.ObjectToFollow.localPosition;
		MoveLegInArc(leg);
	}

	private bool ContinueMovingLeg(RobotLeg leg)
	{
		var distanceBetweenPosAndTarget = Vector3.Distance(leg.ObjectToFollow.localPosition, leg.MoveToTargetPosition);
		if (distanceBetweenPosAndTarget > 1f)
		{
			leg.InitialWorldPosition = leg.ObjectToFollow.position;
			MoveLegInArc(leg);
			return true;
		}
		else
		{
			leg.IsMoving = false;
			return false;
		}
	}

	private void MoveLegToGround(RobotLeg leg)
	{
		leg.RaycastFromPosition = leg.ObjectToFollow.position + new Vector3(0, LegRayCastPositionOffset, 0);

		if (Physics.Raycast(leg.RaycastFromPosition, -Vector3.up, out var hit))
		{
			leg.ObjectToFollow.position = hit.point;
			var hitLocalPoint = transform.InverseTransformPoint(hit.point);
			leg.InitialLocalPosition = new Vector3(leg.InitialLocalPosition.x, hitLocalPoint.y, leg.InitialLocalPosition.z);
		}
	}

	private void MoveLegInArc(RobotLeg leg)
	{
		// Interpolate over the arc relative to center
		var startRelativeToCenter = leg.AnimationStartPoint - leg.AnimationArcCenter;
		var endRelativeToCenter = leg.MoveToTargetPosition - leg.AnimationArcCenter;

		// The fraction of the animation that has happened so far is
		// equal to the elapsed time divided by the desired time for
		// the total journey.
		var fracComplete = (Time.time - leg.AnimationStartTime) * LegMoveSpeed;

		//leg.AnimationArcCenter = new Vector3(leg.AnimationArcCenter.x, leg.AnimationArcCenter.y, leg.AnimationArcCenter.z / 2.0f);
		var arcPosition = Vector3.Slerp(startRelativeToCenter, endRelativeToCenter, fracComplete) + leg.AnimationArcCenter;
		//print(leg.AnimationArcCenter);
		arcPosition = new Vector3(arcPosition.x, arcPosition.y, arcPosition.z);

		leg.ObjectToFollow.localPosition = arcPosition;
		//leg.ObjectToFollow.localPosition += leg.AnimationArcCenter;
	}

	private int GetNumberOfLegsCurrentlyMoving()
	{
		return Convert.ToInt32(Leg1.IsMoving) + Convert.ToInt32(Leg2.IsMoving) +
			Convert.ToInt32(Leg3.IsMoving) + Convert.ToInt32(Leg4.IsMoving);
	}

	private void OnDrawGizmos()
	{
		DrawLegGizmos(Leg1);
		DrawLegGizmos(Leg2);
		DrawLegGizmos(Leg3);
		DrawLegGizmos(Leg4);
	}

	private void DrawLegGizmos(RobotLeg leg)
	{
		Gizmos.color = Color.yellow;
		var legMoveToTargetPosition = transform.TransformPoint(leg.MoveToTargetPosition);
		Gizmos.DrawSphere(legMoveToTargetPosition, 0.5f);

		Gizmos.color = Color.red;
		Gizmos.DrawSphere(leg.RaycastFromPosition, 0.5f);
		Gizmos.DrawRay(leg.RaycastFromPosition, -Vector3.up * LegRayCastPositionOffset);

		var legStartingPosition = transform.TransformPoint(leg.InitialLocalPosition);
		UnityEditor.Handles.DrawWireDisc(legStartingPosition, Vector3.up, MaxLegMoveDistance);

		Gizmos.color = Color.black;
		Gizmos.DrawSphere(legStartingPosition, 0.5f);
		var targetPoint = leg.InitialLocalPosition + leg.Direction * MaxLegMoveDistance;
		Gizmos.DrawLine(leg.ObjectToFollow.position, transform.TransformPoint(targetPoint));
	}
}
