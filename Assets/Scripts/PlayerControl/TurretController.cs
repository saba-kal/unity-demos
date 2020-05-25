using System;
using UnityEngine;

public class TurretController : MonoBehaviour
{
	public Camera PlayerCamera;
	public GameObject GunBarrel;

	public float TurretRotationSpeed = 1;
	public float GunRotationSpeed = 1;
	public float HitDistance = 10;
	public float GunMaxDepressionAngle = 8;
	public float GunMaxAscensionAngle = 15;

	private Vector3 _hitPoint = new Vector3(0, 0, 0);

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKey(KeyCode.Mouse1))
		{
			return;
		}

		// Bit shift the index of the layer (8) to get a bit mask
		int layerMask = 1 << 8;
		// This would cast rays only against colliders in layer 8, so we just inverse the mask.
		layerMask = ~layerMask;

		Ray ray = PlayerCamera.ScreenPointToRay(new Vector3(PlayerCamera.pixelWidth / 2, PlayerCamera.pixelHeight / 2, 0));

		//Raycast hit an object.
		if (Physics.Raycast(ray, out var hit, HitDistance, layerMask))
		{
			_hitPoint = hit.point;
		}
		else
		{
			_hitPoint = ray.origin + ray.direction * HitDistance;
		}

		RotateTurretToPoint(_hitPoint);
		RotateGunToPoint(_hitPoint);
	}

	private void RotateTurretToPoint(Vector3 targetPoint)
	{
		Vector3 turretDirection = (targetPoint - transform.position).normalized;
		Quaternion turretLookRotation = Quaternion.LookRotation(turretDirection, transform.up);

		transform.rotation = Quaternion.RotateTowards(transform.rotation, turretLookRotation, TurretRotationSpeed * Time.deltaTime);
		transform.localEulerAngles = new Vector3(0f, transform.localEulerAngles.y, 0f);
	}

	private void RotateGunToPoint(Vector3 targetPoint)
	{
		Vector3 gunDirection = (targetPoint - GunBarrel.transform.position).normalized;
		Quaternion gunLookRotation = Quaternion.LookRotation(gunDirection, GunBarrel.transform.right);
		Quaternion gunLocalLookRotation = Quaternion.Inverse(GunBarrel.transform.rotation) * gunLookRotation;

		var desiredGunXAngle = gunLocalLookRotation.eulerAngles.x;

		if (desiredGunXAngle > 180)
		{
			desiredGunXAngle = Mathf.Clamp(desiredGunXAngle, 360 - GunMaxAscensionAngle, 359);
		}
		else
		{
			desiredGunXAngle = Mathf.Clamp(desiredGunXAngle, 0, GunMaxDepressionAngle);
		}

		var desiredGunAngle = transform.rotation * Quaternion.Euler(desiredGunXAngle, 0, 0);

		gunLookRotation = Quaternion.Euler(
			desiredGunAngle.eulerAngles.x,
			GunBarrel.transform.rotation.eulerAngles.y,
			GunBarrel.transform.rotation.eulerAngles.z);

		GunBarrel.transform.rotation = Quaternion.RotateTowards(GunBarrel.transform.rotation, gunLookRotation, GunRotationSpeed * Time.deltaTime);
		GunBarrel.transform.localEulerAngles = new Vector3(GunBarrel.transform.localEulerAngles.x, 0, 0);
	}

	private void OnDrawGizmos()
	{

		Gizmos.color = Color.red;
		Gizmos.DrawSphere(_hitPoint, 1);
	}
}
