using UnityEngine;

public class CameraBlurFollow : MonoBehaviour {
	public static CameraBlurFollow Instance;
	Camera cameraComponent;

	void Awake() {
		cameraComponent = GetComponent<Camera>();
		Instance = this;
	}
	void Update() {
		cameraComponent.orthographicSize = CameraFollow.Instance.cameraComponent.orthographicSize;
	}
}