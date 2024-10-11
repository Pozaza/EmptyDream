using NaughtyAttributes;
using UnityEngine;

public class CameraZoom : MonoBehaviour {
	public enum Type {
		Trigger,
		Zone,
		ZoneOutToPlayer
	}
	float _startValue;
	public Type zoomType = Type.Zone;
	public float toZoom = 4;
	[ShowIf("zoomType", Type.Zone)] public float exitZoneValue = 4;
	public float time = .1f;
	public bool instant;

	void Awake() => _startValue = CameraFollow.Instance.cameraComponent.orthographicSize;

	void OnTriggerEnter2D(Collider2D coll) {
		if (coll.CompareTag("Player"))
			CameraZoomManager.Instance.Zoom(toZoom, instant ? 0 : time);
	}

	void OnTriggerExit2D(Collider2D coll) {
		if (coll.CompareTag("Player")) {
			switch (zoomType) {
				case Type.Zone:
					CameraZoomManager.Instance.Zoom(exitZoneValue, instant ? 0 : time);
					break;
				case Type.ZoneOutToPlayer:
					CameraZoomManager.Instance.Zoom(_startValue, instant ? 0 : time);
					break;
			}
		}
	}
	void OnDrawGizmos() {
		Vector3 pos = transform.childCount == 0 ? transform.position : transform.GetChild(0).position;

		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(pos + FindObjectOfType<CameraFollow>().offset, new Vector3(toZoom / .28125f, toZoom / 1.775f / .28125f, 1));
	}
}