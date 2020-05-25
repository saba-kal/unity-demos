using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
	public float MaxHealth;
	public float CurrentHealth;

	public Image HealthBarImage;

	public void TakeDamage(float damage)
	{
		CurrentHealth -= damage;
		HealthBarImage.fillAmount = CurrentHealth / MaxHealth;
	}
}
