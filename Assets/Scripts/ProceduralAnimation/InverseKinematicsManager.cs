using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <see cref="https://www.alanzucconi.com/2017/04/06/implementing-forward-kinematics/"/>
public class InverseKinematicsManager : MonoBehaviour
{
	public List<RobotJoint> Joints;
	public Transform TargetObject;

	public float SamplingDistance;
	public float LearningRate;
	public float DistanceThreshold;

	private Vector3 _endEffectorPos;

	void FixedUpdate()
	{
		var angles = new List<float>();
		foreach (var joint in Joints)
		{
			angles.Add(joint.GetAngle());
		}
		InverseKinematics(TargetObject.position, angles.ToArray());
	}

	public void InverseKinematics(Vector3 target, float[] angles)
	{
		var distance = Vector3.Distance(Joints[Joints.Count - 1].transform.position, target);
		//print("Calculated distance: " + DistanceFromTarget(target, angles) + ", actual distance: " + distance);
		if (Vector3.Distance(Joints[Joints.Count - 1].transform.position, target) < DistanceThreshold)
		{
			return;
		}

		var gradients = new List<float>();

		for (int i = 0; i < Joints.Count; i++)
		{
			// Gradient descent
			// Update : Solution -= LearningRate * Gradient
			float gradient = PartialGradient(target, angles, i);
			gradients.Add(gradient);
			//print("Gradient: " + gradient);
			angles[i] -= LearningRate * gradient;
			Joints[i].SetAngle(angles[i]);

			if (Vector3.Distance(Joints[Joints.Count - 1].transform.position, target) < DistanceThreshold)
			{
				return;
			}
		}

		//print(string.Join(",", gradients));
	}

	public float PartialGradient(Vector3 target, float[] angles, int i)
	{
		// Saves the angle,
		// it will be restored later
		float angle = angles[i];

		// Gradient : [F(x+SamplingDistance) - F(x)] / h
		float f_x = DistanceFromTarget(target, angles);

		angles[i] += SamplingDistance;
		float f_x_plus_d = DistanceFromTarget(target, angles);

		angles[i] -= SamplingDistance;
		float f_x_minus_d = DistanceFromTarget(target, angles);

		//print(f_x_plus_d + "," + f_x);

		float gradient1 = (f_x_plus_d - f_x) / SamplingDistance;
		float gradient2 = (f_x_minus_d - f_x) / SamplingDistance;

		// Restores
		angles[i] = angle;

		//if (gradient1 < gradient2)
		//{
		//	return gradient1;
		//}
		return gradient1;
	}

	public float DistanceFromTarget(Vector3 target, float[] angles)
	{
		Vector3 point = ForwardKinematics(angles);
		var distance = Vector3.Distance(Joints[Joints.Count - 1].transform.position, target);
		return Vector3.Distance(point, target);
	}

	public Vector3 ForwardKinematics(float[] angles)
	{
		Vector3 prevPoint = Joints[0].transform.position;
		Quaternion rotation = transform.parent.gameObject.transform.rotation;
		for (int i = 1; i < Joints.Count; i++)
		{
			// Rotates around a new axis
			rotation *= Quaternion.AngleAxis(angles[i - 1], Joints[i - 1].Axis);
			Vector3 nextPoint = prevPoint + rotation * Joints[i].StartOffset;
			Debug.DrawLine(prevPoint, nextPoint, Color.red);
			prevPoint = nextPoint;
		}

		_endEffectorPos = prevPoint;
		//print("end effector position: " + _endEffectorPos);
		return prevPoint;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(_endEffectorPos, 1);
	}
}
