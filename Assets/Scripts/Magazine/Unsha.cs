using System.Collections;
using UnityEngine;

public class Unsha : MonoBehaviour {
	[Range(1f, 50f)] public int value = 1;
	bool _can;
	Collider2D _collider;
	ParticleSystem _particles;
	SpriteRenderer _spriteRenderer;

	void Awake() {
		StartCoroutine(Wait());
		_collider = GetComponent<Collider2D>();
		_particles = GetComponent<ParticleSystem>();
		_spriteRenderer = GetComponent<SpriteRenderer>();
	}
	void OnTriggerEnter2D(Collider2D hitInfo) {
		if (_can) {
			if (hitInfo.CompareTag("Player")) {
				SkinMagazine.Instance.AddUnshes(value);
				StartCoroutine(Collected());
			} else if (hitInfo.CompareTag("Enemy"))
				Destroy(gameObject);
		}
	}
	IEnumerator Wait() {
		yield return new WaitForSeconds(.2f);
		_can = true;
	}
	IEnumerator Collected() {
		Destroy(_collider);
		Destroy(_spriteRenderer);
		_particles.Emit(8);
		yield return new WaitForSeconds(.35f);
		Destroy(gameObject);
	}
}
