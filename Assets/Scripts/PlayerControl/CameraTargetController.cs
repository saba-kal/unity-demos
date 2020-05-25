using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTargetController : MonoBehaviour
{

	public Transform TargetObject;
	public float PositionYOffset;

	void Update()
	{
		var targetPosition = TargetObject.position;
		targetPosition.y += PositionYOffset;

		transform.position = targetPosition;
	}

	private void OnDrawGizmos()
	{

		Gizmos.color = Color.blue;
		Gizmos.DrawSphere(transform.position, 1);
	}
}
