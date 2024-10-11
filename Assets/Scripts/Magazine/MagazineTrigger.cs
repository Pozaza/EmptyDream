using UnityEngine;

public class MagazineTrigger : MonoBehaviour {
	public SpriteRenderer[] toOutline;
	public Material outline;

	[Header("Звуки")]
	AudioSource _audioManager;
	public AudioClip changeSound;
	public AudioClip traderDisappear;
	public AudioClip traderShow;

	Material _originalMaterial;
	bool isOpened;

	void Awake() {
		_originalMaterial = toOutline[0].material;
		_audioManager = this.FindTag<AudioSource>("Sounds");
	}
	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.CompareTag("Player")) {
			foreach (SpriteRenderer r in toOutline)
				r.material = outline;
			Menu.Instance.Cutscene(false);
		}
	}
	void OnTriggerStay2D(Collider2D collider) {
		if (collider.CompareTag("Player") && Input.GetKey(Settings.Key(KeyBind.Menu)) && isOpened) {
			Interface.Instance.magazine.GetComponent<Animator>().SetTrigger("close");
			_audioManager.PlayOneShot(changeSound, 1f);
			isOpened = false;
			PlayerScript.Instance.CanMove();
			foreach (SpriteRenderer r in toOutline)
				r.material = outline;
		} else if (collider.CompareTag("Player") && Input.GetKey(Settings.Key(KeyBind.Interaction)) && !isOpened) {
			Interface.Instance.magazine.GetComponent<Animator>().SetTrigger("open");
			_audioManager.PlayOneShot(changeSound, 1f);
			isOpened = true;
			PlayerScript.Instance.CantMove();
			foreach (SpriteRenderer r in toOutline)
				r.material = _originalMaterial;
		}
	}
	void OnTriggerExit2D(Collider2D collider) {
		if (collider.CompareTag("Player") && isOpened) {
			Interface.Instance.magazine.GetComponent<Animator>().SetTrigger("close");
			_audioManager.PlayOneShot(changeSound, 1f);
			isOpened = false;
			PlayerScript.Instance.CanMove();
			Menu.Instance.Cutscene(true);
			foreach (SpriteRenderer r in toOutline)
				r.material = _originalMaterial;
		} else if (collider.CompareTag("Player") && !isOpened) {
			foreach (SpriteRenderer r in toOutline)
				r.material = _originalMaterial;
			Menu.Instance.Cutscene(true);
		}
	}
}
