using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class Interact : MonoBehaviour {
	public UnityEvent enter, exit, firstAction, secondAction;
	bool isUsed = false, entered = false;
	public bool useDistance = false;
	[EnableIf("useDistance")] public float actionDistance = 10f;

	public void Destroy(Collider2D collider) => Destroy(collider);

	void Update() {
		if (!useDistance)
			return;

		if (Vector2.Distance(PlayerScript.Instance.transform.position, transform.position) < actionDistance && !entered) {
			entered = true;
			enter.Invoke();
		} else if (Vector2.Distance(PlayerScript.Instance.transform.position, transform.position) > actionDistance && entered) {
			entered = false;
			exit.Invoke();
		}

		if (entered && Input.GetKeyDown(Settings.Key(KeyBind.Interaction))) {
			if (!isUsed)
				firstAction.Invoke();
			else
				secondAction.Invoke();
			isUsed = !isUsed;
		}
	}

	void OnTriggerEnter2D(Collider2D collision) {
		if (collision.CompareTag("Player") && !useDistance)
			enter.Invoke();
	}
	void OnTriggerExit2D(Collider2D collision) {
		if (collision.CompareTag("Player") && !useDistance)
			exit.Invoke();
	}
	void OnTriggerStay2D(Collider2D collision) {
		if (collision.CompareTag("Player") && Input.GetKeyDown(Settings.Key(KeyBind.Interaction)) && !useDistance) {
			if (!isUsed)
				firstAction.Invoke();
			else
				secondAction.Invoke();
			isUsed = !isUsed;
		}
	}
}