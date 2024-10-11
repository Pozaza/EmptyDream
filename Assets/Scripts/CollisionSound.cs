using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class CollisionSound : MonoBehaviour {
	[MinMaxSlider(0.5f, 1.5f)]
	public Vector2 randomPitch = new(.9f, 1.1f);
	public List<CollisionEffect> objectsToCollide;
	AudioSource audioSource;

	void Awake() => audioSource = GetComponent<AudioSource>();
	void OnCollisionEnter2D(Collision2D other) {
		if (objectsToCollide.Count == 0)
			return;

		CollisionEffect coll = objectsToCollide.Find(coll => coll.obj == other.gameObject);

		if (coll.obj != null) {
			AudioSource source = coll.source == null ? audioSource : coll.source;
			source.pitch = Random.Range(randomPitch.x, randomPitch.y);
			source.PlayOneShot(coll.sound == null ? source.clip : coll.sound);

			if (coll.particle != null)
				for (int i = 0; i < other.contacts.Length; i++) {
					coll.particle.transform.position = other.contacts[i].point;
					coll.particle.Emit((int)coll.particle.emission.rateOverTime.constantMax);
				}
		}
	}
}

[System.Serializable]
public struct CollisionEffect {
	public GameObject obj;
	public AudioClip sound;
	public ParticleSystem particle;
	public AudioSource source;
}