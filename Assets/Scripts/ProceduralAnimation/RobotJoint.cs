using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <see cref="https://www.alanzucconi.com/2017/04/06/implementing-forward-kinematics/"/>
public class RobotJoint : MonoBehaviour
{
	public Vector3 Axis;
	public Vector3 StartOffset;

	public float RotationSpeed = 50;

	void Awake()
	{
		StartOffset = transform.localPosition;
	}

	public float GetAngle()
	{
		if (Axis.x >= 1)
		{
			return gameObject.transform.localEulerAngles.x;
		}
		else if (Axis.y >= 1)
		{
			return gameObject.transform.localEulerAngles.y;
		}
		else if (Axis.z >= 1)
		{
			return gameObject.transform.localEulerAngles.z;
		}
		return 0;
	}

	public void SetAngle(float angle)
	{
		Quaternion? desiredRotation = null;

		if (Axis.x >= 1)
		{
			desiredRotation = Quaternion.Euler(angle, 0, 0);
		}
		else if (Axis.y >= 1)
		{
			desiredRotation = Quaternion.Euler(0, angle, 0);
		}
		else if (Axis.z >= 1)
		{
			desiredRotation = Quaternion.Euler(0, 0, angle);
		}

		if (desiredRotation.HasValue)
		{
			var rotation = Quaternion.RotateTowards(gameObject.transform.localRotation, desiredRotation.Value, Time.deltaTime * RotationSpeed);
			gameObject.transform.localRotation = rotation;
		}
	}
}
