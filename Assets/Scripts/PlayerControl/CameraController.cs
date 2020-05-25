using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{

	public float OrbitSensitivity = 5;
	public float ZoomSensitivity = 20;
	public float ZoomAnimationSpeed = 0.2f;
	public float OrbitDistance = 25;
	public float MaxOrbitDistance = 60;
	public float MinOrbitDistance = 10;
	public float ScopeOrbitDistance = -5;
	public float ScopeSensitivity = 1;
	public float CameraCollisionDistance = 5;

	[Range(0.0f, 90f)]
	public float CameraMaxXAngle = 90;
	[Range(-90f, 0.0f)]
	public float CameraMinXAngle = -30;

	[Range(0.0f, 179f)]
	public float DefaultFov = 60;
	[Range(0.0f, 179f)]
	public float ScopeFov = 20;

	public Transform Target;

	private Vector3 _desiredCameraPosition;

	private bool _lockCursor = true;
	private bool _isScoped = false;

	private float _xRot = 0;
	private float _yRot = 0;

	// Used for making smooth camera zoom animation.
	private float _targetOrbitDistance = 0f;

	void Awake()
	{
		Cursor.lockState = CursorLockMode.Locked;
		_targetOrbitDistance = OrbitDistance;
	}

	void Update()
	{
		if (_lockCursor)
		{
			AvoidCollision();
			RotateCamera();
			AdjustZoom();

			transform.position = _desiredCameraPosition;
		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			_lockCursor = !_lockCursor;
		}
		Cursor.lockState = _lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
	}

	public void RotateCamera()
	{
		var sensitivity = OrbitSensitivity;
		if (_isScoped)
		{
			sensitivity = ScopeSensitivity;
		}
		_xRot += -1 * Input.GetAxis("Mouse Y") * sensitivity;
		_xRot = Mathf.Clamp(_xRot, CameraMinXAngle, CameraMaxXAngle);
		_yRot += Input.GetAxis("Mouse X") * sensitivity;
		transform.rotation = Quaternion.Euler(0, _yRot, 0) * Quaternion.Euler(_xRot, 0, 0);
		_desiredCameraPosition = Target.position - transform.forward * OrbitDistance;
	}

	public void AdjustZoom()
	{

		if (_isScoped)
		{
			OrbitDistance = ScopeOrbitDistance;
		}
		else
		{
			var distanceToTargetZoom = Mathf.Abs(OrbitDistance - _targetOrbitDistance);
			var zoomSpeed = ZoomAnimationSpeed * Time.deltaTime * distanceToTargetZoom;
			var resultOrbitDistance = Mathf.MoveTowards(OrbitDistance, _targetOrbitDistance, zoomSpeed);
			resultOrbitDistance = Mathf.Clamp(resultOrbitDistance, MinOrbitDistance, MaxOrbitDistance);
			OrbitDistance = resultOrbitDistance;
		}

		var mouseWheelValue = -Input.GetAxis("Mouse ScrollWheel");
		if (mouseWheelValue == 0)
		{
			return;
		}

		_targetOrbitDistance = OrbitDistance + mouseWheelValue * ZoomSensitivity;

		if (_isScoped && mouseWheelValue > 0)
		{
			_isScoped = false;
			Camera.main.fieldOfView = DefaultFov;
		}
		else if (_targetOrbitDistance < MinOrbitDistance)
		{
			_isScoped = true;
			Camera.main.fieldOfView = ScopeFov;
		}
	}

	public void AvoidCollision()
	{
		if (_isScoped)
		{
			return;
		}

		var clipPoints = new Vector3[6];
		clipPoints[0] = new Vector3( //Bottom
			transform.position.x,
			transform.position.y - CameraCollisionDistance,
			transform.position.z);
		clipPoints[1] = new Vector3( //Top
			transform.position.x,
			transform.position.y + CameraCollisionDistance,
			transform.position.z);
		clipPoints[2] = new Vector3( //Left
			transform.position.x - CameraCollisionDistance,
			transform.position.y,
			transform.position.z);
		clipPoints[3] = new Vector3( //Right
			transform.position.x + CameraCollisionDistance,
			transform.position.y,
			transform.position.z);
		clipPoints[4] = new Vector3( //Front
			transform.position.x,
			transform.position.y,
			transform.position.z + CameraCollisionDistance);
		clipPoints[5] = new Vector3( //Back
			transform.position.x,
			transform.position.y,
			transform.position.z - CameraCollisionDistance);

		foreach (var clipPoint in clipPoints)
		{
			if (Physics.Linecast(transform.position, clipPoint, out var hit))
			{
				var resultDistance = Vector3.Distance(Target.position, hit.point);
				OrbitDistance = Mathf.Clamp(resultDistance * 0.95f, MinOrbitDistance, MaxOrbitDistance);
				_targetOrbitDistance = OrbitDistance;
			}
		}
	}
}
