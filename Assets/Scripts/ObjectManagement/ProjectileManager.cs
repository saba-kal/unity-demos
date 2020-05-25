using UnityEngine;

public class ProjectileManager : MonoBehaviour
{

	public float LifeTime = 5;
	public float Damage = 10;

	void Start()
	{
		Destroy(gameObject, LifeTime);
	}

	void OnCollisionEnter(Collision collision)
	{
		var healthBar = collision.gameObject.GetComponent<HealthBar>();
		if (healthBar != null)
		{
			healthBar.TakeDamage(Damage);
		}

		var particleSystem = gameObject.GetComponent<MultipleParticleSystem>();
		if (particleSystem != null)
		{
			particleSystem.Play();
		}
		Destroy(gameObject);
	}
}
