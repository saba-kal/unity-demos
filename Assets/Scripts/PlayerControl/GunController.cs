using UnityEngine;
using System.Collections;

public class GunController : MonoBehaviour
{
	public Rigidbody Projectile;
	public Transform Spawnpoint;
	public float ProjectileVelocity = 100;

	void Update()
	{
		if (Input.GetButtonDown("Fire1"))
		{
			var projectileClone = Instantiate(Projectile, Spawnpoint.position, Projectile.rotation);
			projectileClone.velocity = Spawnpoint.TransformDirection(Vector3.forward * ProjectileVelocity);
		}
	}
}
