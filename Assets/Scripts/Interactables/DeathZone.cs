using UnityEngine;

public class DeathZone : MonoBehaviour {
	void OnTriggerEnter2D(Collider2D coll) {
		if (coll.CompareTag("Player"))
			PlayerHealth.Instance?.Die();
	}
}