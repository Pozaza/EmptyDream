using System.Collections;
using UnityEngine;

public class EnemyBullet : MonoBehaviour {
	public float speed = 20f;
	public int damage;
	public Rigidbody2D rb;
	public bool isPenetration;

	void Start() {
		rb.velocity = transform.right * speed;
		StartCoroutine(BulletD());
	}
	void OnTriggerEnter2D(Collider2D hitInfo) {
		if (hitInfo.CompareTag("Player")) {
			PlayerHealth.Instance.TakeDamage(damage);
			if (!isPenetration)
				Destroy(gameObject);
		} else if (hitInfo.gameObject.name == "Коллизия")
			Destroy(gameObject);
	}
	IEnumerator BulletD() {
		yield return new WaitForSeconds(3f);
		Destroy(gameObject);
	}
}
