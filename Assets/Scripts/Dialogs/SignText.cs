using UnityEngine;

public class SignText : MonoBehaviour {
	public GameObject labelObject;
	public AudioClip sound;

	void Awake() {
		if (!labelObject)
			labelObject = gameObject.transform.GetChildObj(1);
	}
	void OnTriggerEnter2D(Collider2D collision) {
		if (collision.CompareTag("Player")) {
			labelObject.SetActive(true);
			GameManager.Instance?.sounds?.PlayOneShot(sound);
		}
	}
	void OnTriggerStay2D(Collider2D collision) {
		if (collision.CompareTag("Player")) {
			if (Input.GetKeyDown(Settings.Key(KeyBind.Dialog)))
				labelObject.SetActive(false);
		}
	}
	void OnTriggerExit2D(Collider2D collision) {
		if (collision.CompareTag("Player")) {
			labelObject.SetActive(false);
			GameManager.Instance?.sounds?.PlayOneShot(sound);
		}
	}
}
