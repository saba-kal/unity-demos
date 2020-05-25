using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Downforce : MonoBehaviour
{
	public Vector3 ForceVector;

	void Update()
	{
		GetComponent<Rigidbody>().AddForce(ForceVector);
	}
}
