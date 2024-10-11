using DG.Tweening;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
	public static CameraFollow Instance;
	public Transform startTransform, target;
	public Vector3 offset;
	public float speed = .5f;
	public bool canMouseOffset, active;
	[HideInInspector] public Vector3 startOffset;
	[HideInInspector] public Camera cameraComponent;

	void Awake() {
		cameraComponent = GetComponent<Camera>();
		Instance = this;
		startOffset = offset;
		if (startTransform != null)
			target = startTransform;
		if (active)
			StartFollow();
	}
	void Update() {
		if (active) {
			Vector3 pos = new();
			if (canMouseOffset) {
				Vector2 mousePos = cameraComponent.ScreenToViewportPoint(Input.mousePosition);
				pos = (mousePos - Vector2.one / 2f) * .75f;
			}
			transform.DOMove(pos + target.position + offset, speed).SetEase(Ease.OutCubic);
		}
	}
	public void Shake(float power, float duration) => transform.DOShakePosition(duration, power, 10, 0);
	public void StartFollow() => active = true;
	public void StopFollow() => active = false;
	public void InstantFollow() => transform.position = target.position + offset;
	public void ChangeOffset(Vector3 newOffset, bool reset) => offset = reset ? startOffset : newOffset;
}