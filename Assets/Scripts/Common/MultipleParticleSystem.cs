using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleParticleSystem : MonoBehaviour
{
	public List<ParticleSystem> ParticlePrefabs;

	public void Play()
	{
		if (ParticlePrefabs == null)
		{
			return;
		}

		foreach (var particle in ParticlePrefabs)
		{
			var particleObject = Instantiate(particle, transform.position, Quaternion.identity);
			particleObject.Play();
			Destroy(particleObject.gameObject, particleObject.main.duration);
		}
	}
}
