using NaughtyAttributes;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour {
	public TextAsset inkJSON;
	public DialogColorAndFont dialogColorAndFont;

	[Header("Диалог")]
	public bool triggerOnlyOnce, mustClick = true;

	[Header("Настройки диалога")]
	public bool canInteract = true, lockMove;
	[ShowIf("canInteract")] public bool autoNext = true;

	void OnTriggerStay2D(Collider2D collider) {
		if (!collider.CompareTag("Player"))
			return;

		Activate();
	}
	public void Activate() {
		if (mustClick) {
			if (Input.GetKeyDown(Settings.Key(KeyBind.Dialog)) && !DialogueManager.Instance.DialogueIsPlaying) {
				autoNext = !canInteract ? true : autoNext;
				DialogueManager.Instance.EnterDialogueMode(inkJSON, dialogColorAndFont, canInteract, autoNext, lockMove);
				TryGetComponent(out BoxCollider2D coll);
				if (triggerOnlyOnce && coll != null)
					Destroy(coll);
			}
		} else if (!DialogueManager.Instance.DialogueIsPlaying) {
			DialogueManager.Instance.EnterDialogueMode(inkJSON, dialogColorAndFont, canInteract, autoNext, lockMove);
			if (triggerOnlyOnce)
				Destroy(GetComponent<BoxCollider2D>());
		}
	}
}
