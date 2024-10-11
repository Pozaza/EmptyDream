using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class InteractiveCutscene : MonoBehaviour { // TODO: доделать
    public PlayableDirector director;
    public TimelineAsset movie;
    public GameObject buttons;
    public ColorLerp colorLerp;
    public bool canShoot, companyon, canMove, cameraFollow, canInterface, noGravity;
    BoxCollider2D _collider;
    bool startInterface;
    KeyBind currentKey;

    void Awake() => _collider = GetComponent<BoxCollider2D>();
    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player"))
            StartCutscene();
    }
    void Update() {
        if (Input.GetKeyDown(Settings.Key(currentKey))) {
            director.Resume();
        }
    }
    public void NextKeyEvent(KeyBind key) {
        director.Pause();
        currentKey = key;
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
        startInterface = Interface.Instance.GetState();
        if (!canInterface)
            Interface.Instance?.HideInterface();
        if (noGravity)
            PlayerScript.Instance.rb.simulated = false;
        if (!cameraFollow)
            CameraFollow.Instance.active = false;
        GameManager.interactiveCutscene = true;
    }

    public void EndCutscene() {
        if (noGravity)
            PlayerScript.Instance.rb.simulated = true;
        PrefabWeapon.Instance?.CanShoot(true);
        if (startInterface)
            Interface.Instance?.ShowInterface();
        else
            Interface.Instance?.HideInterface();
        PlayerScript.Instance?.CanMove();
        if (!cameraFollow)
            CameraFollow.Instance.active = true;
        GameManager.interactiveCutscene = false;
    }
}
