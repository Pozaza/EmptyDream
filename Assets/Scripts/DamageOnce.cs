using UnityEngine;

public class DamageOnce : MonoBehaviour {
	public int damage;

	void OnTriggerEnter2D(Collider2D collision) {
		if (collision.CompareTag("Player"))
			PlayerHealth.Instance.TakeDamage(damage);
	}
}
