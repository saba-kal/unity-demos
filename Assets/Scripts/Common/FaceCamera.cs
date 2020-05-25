using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
	public Camera TargetCamera;

	void Update()
	{
		transform.rotation = TargetCamera.transform.rotation;
	}
}
