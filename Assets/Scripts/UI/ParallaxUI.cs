using UnityEngine;

public class ParallaxUI : MonoBehaviour {

	Vector3 startPos;
	public float modifier;
	void Start() => startPos = transform.position;
	void Update() {
		Vector3 pz = CameraFollow.Instance.cameraComponent.ScreenToViewportPoint(Input.mousePosition);
		pz.z = 0;
		gameObject.transform.position = pz;
		transform.position = new Vector3(startPos.x + (pz.x * modifier), startPos.y + (pz.y * modifier), 0);
	}
}