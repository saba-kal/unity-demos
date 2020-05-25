using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
	public Transform Target;
	public Transform HoverPoint;

	public float MoveSpeed;
	public float LookSpeed;
	public float StopDistance = 10;
	public float HoverHeight = 5;
	public float HoverAdjustSpeed = 1;

	// Update is called once per frame
	void Update()
	{
		RotateToFacePoint(Target.position);
		if (Vector3.Distance(transform.position, Target.position) > StopDistance)
		{
			MoveToTarget(Target.position);
		}
		HoverOverGround();
		TiltToMatchTerrainSlope();
	}

	private void RotateToFacePoint(Vector3 targetPoint)
	{
		var direction = (targetPoint - transform.position).normalized;
		var lookRotation = Quaternion.LookRotation(direction, transform.up);

		transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, LookSpeed * Time.deltaTime);
		transform.localEulerAngles = new Vector3(0f, transform.localEulerAngles.y, 0f);
	}

	private void MoveToTarget(Vector3 targetPoint)
	{
		var step = MoveSpeed * Time.deltaTime;
		var horizontalTarget = new Vector3(targetPoint.x, transform.position.y, targetPoint.z);
		transform.position = Vector3.MoveTowards(transform.position, horizontalTarget, step);
	}

	private void HoverOverGround()
	{
		if (Physics.Raycast(transform.position, -Vector3.up, out var hit))
		{
			var differenceToHoverHeight = Mathf.Abs(transform.position.y - hit.point.y - HoverHeight);
			var step = (differenceToHoverHeight * (HoverAdjustSpeed / 10f)) * Time.deltaTime;

			var desiredHeightPosition = new Vector3(transform.position.x, hit.point.y + HoverHeight, transform.position.z);
			transform.position = Vector3.Lerp(transform.position, desiredHeightPosition, step);
		}
	}

	private void TiltToMatchTerrainSlope()
	{
		//var frontRayCastPosition = transform.position + new Vector3(0, 10, 10);
		//var backRayCastPosition = transform.position + new Vector3(0, 10, -10);
		//if (Physics.Raycast(frontRayCastPosition, -Vector3.up, out var frontHit) &&
		//	Physics.Raycast(backRayCastPosition, -Vector3.up, out var backHit))
		//{
		//	var upright = Vector3.Cross(transform.right, -(frontHit.point - backHit.point).normalized);
		//	transform.rotation = Quaternion.LookRotation(Vector3.Cross(transform.right, upright));
		//}

		if (Physics.SphereCast(transform.position, 5f, -(transform.up), out var hit, HoverHeight))
		{
			var desiredRotation = Quaternion.LookRotation(Vector3.Cross(transform.right, hit.normal));
			transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, HoverAdjustSpeed * Time.deltaTime);
		}
	}
}
