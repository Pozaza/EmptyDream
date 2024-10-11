using UnityEngine;

public class JumpPad : MonoBehaviour {
	public Vector2 strenght = new(0, 20);
	void OnTriggerEnter2D(Collider2D collider) {
		if (!collider.IsTag("Player", "Enemy") && collider.TryGetComponent(out Rigidbody2D rb))
			rb.velocity = strenght;
		if (collider.CompareTag("Player"))
			CameraFollow.Instance.Shake(.05f, .1f);
	}
}
