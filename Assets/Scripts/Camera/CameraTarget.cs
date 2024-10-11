using NaughtyAttributes;
using UnityEngine;

public class CameraTarget : MonoBehaviour {
	public enum Type {
		Trigger,
		Zone,
		ZoneOutToPlayer
	}
	public Type targetType = Type.Zone;
	public Transform targetEnter;
	[ShowIf("targetType", Type.Zone)] public Transform targetExit;
	public Vector3 changeOffset = new(999, 999, 999);
	GameObject _targetPlayer;

	void Awake() {
		_targetPlayer = GameObject.FindWithTag("CameraTarget");
		if (targetEnter == null)
			targetEnter = transform;
	}
	void OnTriggerEnter2D(Collider2D coll) {
		if (coll.CompareTag("Player")) {
			CameraFollow.Instance.ChangeOffset(changeOffset, changeOffset == new Vector3(999, 999, 999));
			CameraFollow.Instance.target = targetEnter;
		}
	}
	void OnTriggerExit2D(Collider2D coll) {
		if (coll.CompareTag("Player")) {
			CameraFollow.Instance.ChangeOffset(Vector3.zero, true);
			CameraFollow.Instance.target = targetType == Type.Zone ? targetExit : targetType == Type.ZoneOutToPlayer ? _targetPlayer.transform : null;
		}
	}
}