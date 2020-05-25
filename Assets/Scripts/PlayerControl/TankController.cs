using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour
{

	public List<AxleInfo> AxleInfos; // the information about each individual axle
	public float MaxMotorTorque; // maximum torque the motor can apply to wheel
	public float MaxRpm;
	public float BreakTorque;
	public float TurnBreakTorque;

	void Start()
	{
		if (AxleInfos != null && AxleInfos.Count > 0)
		{
			AxleInfos[0].LeftWheel.ConfigureVehicleSubsteps(5f, 12, 15);
		}
	}

	public void FixedUpdate()
	{
		float motor = MaxMotorTorque * Input.GetAxis("Vertical");
		float steeringMotor = MaxMotorTorque * Input.GetAxis("Horizontal");
		bool isBreaking = Input.GetKey(KeyCode.Space);

		foreach (var axleInfo in AxleInfos)
		{
			//print($"left rpm: {axleInfo.LeftWheel.rpm}, right rpm: {axleInfo.RightWheel.rpm}");

			var rightMotor = motor;
			var leftMotor = motor;
			var rightSteerMotor = steeringMotor * (motor >= 0 ? 1 : -1);
			var leftSteerMotor = steeringMotor * (motor >= 0 ? 1 : -1);

			if (Mathf.Abs(axleInfo.LeftWheel.rpm) > MaxRpm)
			{
				leftMotor = 0;
				leftSteerMotor = 0;
			}
			if (Mathf.Abs(axleInfo.RightWheel.rpm) > MaxRpm)
			{
				rightMotor = 0;
				rightSteerMotor = 0;
			}

			if (isBreaking)
			{
				axleInfo.LeftWheel.motorTorque = 0;
				axleInfo.RightWheel.motorTorque = 0;
				axleInfo.LeftWheel.brakeTorque = BreakTorque;
				axleInfo.RightWheel.brakeTorque = BreakTorque;
			}
			else if (steeringMotor > 0)
			{
				axleInfo.LeftWheel.motorTorque = leftSteerMotor;
				axleInfo.LeftWheel.brakeTorque = 0;
				axleInfo.RightWheel.motorTorque = 0;
				axleInfo.RightWheel.brakeTorque = TurnBreakTorque;
			}
			else if (steeringMotor < 0)
			{
				axleInfo.RightWheel.motorTorque = -rightSteerMotor;
				axleInfo.RightWheel.brakeTorque = 0;
				axleInfo.LeftWheel.motorTorque = 0;
				axleInfo.LeftWheel.brakeTorque = TurnBreakTorque;
			}
			else
			{
				axleInfo.LeftWheel.motorTorque = leftMotor;
				axleInfo.RightWheel.motorTorque = rightMotor;
				axleInfo.LeftWheel.brakeTorque = 0;
				axleInfo.RightWheel.brakeTorque = 0;
			}

			ApplyLocalPositionToVisuals(axleInfo.LeftWheel);
			ApplyLocalPositionToVisuals(axleInfo.RightWheel);
		}
	}

	// finds the corresponding visual wheel
	// correctly applies the transform
	private void ApplyLocalPositionToVisuals(WheelCollider collider)
	{
		if (collider.transform.childCount == 0)
		{
			return;
		}

		Transform visualWheel = collider.transform.GetChild(0);

		Vector3 position;
		Quaternion rotation;
		collider.GetWorldPose(out position, out rotation);

		visualWheel.transform.position = position;
		visualWheel.transform.rotation = rotation;
	}
}

[System.Serializable]
public class AxleInfo
{
	public WheelCollider LeftWheel;
	public WheelCollider RightWheel;
	public bool Motor; // is this wheel attached to motor?
	public bool Steering; // does this wheel apply steer angle?
}
