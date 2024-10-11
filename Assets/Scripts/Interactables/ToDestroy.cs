using UnityEngine;

public class ToDestroy : MonoBehaviour {
	public DestroyObject destroyObjectScript;

	void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag("Sword"))
			destroyObjectScript.KillObject();
	}
}
