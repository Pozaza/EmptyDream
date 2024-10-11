using DG.Tweening;
using UnityEngine;

public class CameraZoomManager : MonoBehaviour {
	public static CameraZoomManager Instance;
	float startValue;

	void Awake() {
		Instance = this;
		startValue = CameraFollow.Instance.cameraComponent.orthographicSize;
	}
	public void Zoom(float value, float t) {
		for (int i = 0; i < Camera.allCamerasCount; i++)
			Camera.allCameras[i].DOOrthoSize(value, t).SetEase(Ease.OutCubic);
	}
	public void ZoomInstant(float value) {
		for (int i = 0; i < Camera.allCamerasCount; i++)
			Camera.allCameras[i].DOOrthoSize(value, 0);
	}
	public void ZoomToOriginal() {
		for (int i = 0; i < Camera.allCamerasCount; i++)
			Camera.allCameras[i].DOOrthoSize(startValue, .1f).SetEase(Ease.OutCubic);
	}
}
