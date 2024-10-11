using UnityEngine;
public class DestroyObject : MonoBehaviour {
	public AudioClip clip;
	public Material outline;
	public GameObject objectToOutline;
	public GameObject objectToDestroy;
	Material _original;
	SpriteRenderer _renderer;

	void Awake() {
		_renderer = objectToOutline.GetComponent<SpriteRenderer>();
		_original = _renderer.material;
	}
	public void KillObject() {
		GameManager.Instance?.sounds?.PlayOneShot(clip, .75f);
		if (objectToDestroy.TryGetComponent(out Enemy enemy))
			enemy.Hit(3);
		else {
			Destroy(objectToDestroy);
			Destroy(this);
		}
	}
	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.CompareTag("Player"))
			_renderer.material = outline;
	}
	void OnTriggerExit2D(Collider2D collider) {
		if (collider.CompareTag("Player"))
			_renderer.material = _original;
	}
}