using DG.Tweening;
using UnityEngine;

public class ParallaxBG : MonoBehaviour {
	public float parallax = .5f;
	Vector2 startPos, playerStartPos;

	void Awake() {
		playerStartPos = CameraFollow.Instance.transform.position;
		startPos = transform.position;
	}
	void Update() {
		Vector2 dist = new(
			playerStartPos.x - CameraFollow.Instance.transform.position.x,
			playerStartPos.y - CameraFollow.Instance.transform.position.y
		);

		transform.DOMove(new Vector3(startPos.x + dist.x * parallax * -1, startPos.y + dist.y * parallax * -1, transform.position.z), .01f);
	}
}
