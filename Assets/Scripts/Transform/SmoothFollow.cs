using DG.Tweening;
using UnityEngine;

public class SmoothFollow : MonoBehaviour {
	public float m_DampTime = 10f;
	public Transform m_Target;
	public float m_XOffset, m_YOffset;
	public bool useRotation, useLookTo;

	void Update() {
		if (m_Target) {
			transform.DOMove(new Vector2(m_Target.position.x + m_XOffset, m_Target.position.y + m_YOffset), m_DampTime).SetEase(Ease.OutCubic).SetAutoKill(true);

			Quaternion rotation = m_Target.rotation;

			Quaternion rotateTo = Quaternion.FromToRotation(-transform.localPosition, m_Target.position);

			if (useRotation)
				transform.DORotateQuaternion(rotation, m_DampTime).SetAutoKill(true);
			if (useLookTo) {
				if (PlayerScript.Instance.facingRight)
					rotateTo.z *= -1;
				transform.DORotateQuaternion(rotateTo, m_DampTime).SetAutoKill(true);
			}
		}
	}
	void OnDisable() => transform.DOKill();
}