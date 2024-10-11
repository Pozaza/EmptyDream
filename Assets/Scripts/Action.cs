using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class Action : MonoBehaviour {
	public UnityEvent actions, actions2;
	public bool executeOnStart, executeAfterSeconds, executeOnPlayer, executeOnCollisionEnter, executeOnCollisionExit;
	[EnableIf("executeAfterSeconds")] public float seconds;
	[EnableIf("executeOnPlayer")] public bool once;

	IEnumerator Start() {
		if (executeOnStart)
			actions.Invoke();
		if (executeAfterSeconds) {
			yield return new WaitForSeconds(seconds);
			actions.Invoke();
		}
	}
	public void StartActions() => actions.Invoke();
	void OnTriggerEnter2D(Collider2D collider) {
		if (executeOnPlayer && collider.CompareTag("Player")) {
			actions.Invoke();
			if (once)
				Destroy(this);
		}
	}
	void OnCollisionEnter2D(Collision2D _) {
		if (executeOnCollisionEnter)
			actions.Invoke();
	}
	void OnCollisionExit2D(Collision2D _) {
		if (executeOnCollisionExit)
			actions2.Invoke();
	}
}
