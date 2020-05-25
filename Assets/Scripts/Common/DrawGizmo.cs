using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGizmo : MonoBehaviour
{
	public Color GizmoColor = Color.yellow;

	private void OnDrawGizmos()
	{
		Gizmos.color = GizmoColor;
		Gizmos.DrawSphere(transform.position, 1);
	}
}
