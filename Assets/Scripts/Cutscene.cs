using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[RequireComponent(typeof(SignalReceiver))]
public class Cutscene : MonoBehaviour {
	public PlayableDirector director;
	public TimelineAsset movie;
	BoxCollider2D _collider;

	public bool canShoot, companyon, canMove, cameraFollow, canMenu, canInterface, noGravity;
	bool startInterface;

	void Awake() => _collider = GetComponent<BoxCollider2D>();
	void OnTriggerEnter2D(Collider2D collision) {
		if (collision.CompareTag("Player"))
			StartCutscene();
	}
	public void StartCutscene() {
		director.stopped += delegate {
			EndCutscene();
		};

		DialogueManager.Instance?.ExitDialogueMode();
		Destroy(_collider);
		director.Play(movie);
		PrefabWeapon.Instance.canShootWhileCutscene = canShoot;
		if (companyon)
			PrefabWeapon.Instance.companyon = false;
		if (!canMove)
			PlayerScript.Instance?.CantMove();
		if (!canMenu)
			Menu.Instance?.Cutscene(false);
		startInterface = Interface.Instance.GetState();
		if (!canInterface)
			Interface.Instance?.HideInterface();
		if (noGravity)
			PlayerScript.Instance.rb.simulated = false;
		if (!cameraFollow)
			CameraFollow.Instance.active = false;
		GameManager.cutscene = true;

	}
	public void EndCutscene() {
		if (noGravity)
			PlayerScript.Instance.rb.simulated = true;
		PrefabWeapon.Instance?.CanShoot(true);
		Menu.Instance?.Cutscene(true);
		if (startInterface)
			Interface.Instance?.ShowInterface();
		else
			Interface.Instance?.HideInterface();
		PlayerScript.Instance?.CanMove();
		GameManager.cutscene = false;
		if (!cameraFollow)
			CameraFollow.Instance.active = true;
	}
}
