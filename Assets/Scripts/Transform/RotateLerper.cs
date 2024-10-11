using System.Collections;
using DG.Tweening;
using UnityEngine;

public class RotateLerper : MonoBehaviour {
	public float rotateTo;
	public float duration = .2f;
	public Ease easing = Ease.OutCubic;
	Quaternion _startRotation;
	public bool resetValueOnDisable;
	public float startCooldown;

	void Awake() => _startRotation = transform.rotation;
	void OnDisable() {
		if (resetValueOnDisable)
			transform.rotation = _startRotation;
		transform.DOKill();
	}
	public void AddRotate(float toAdd) {
		rotateTo += toAdd;
		transform.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, rotateTo), duration).SetEase(easing).SetAutoKill(true);
	}
	void OnEnable() => StartCoroutine(First());
	IEnumerator First() {
		yield return new WaitForSeconds(startCooldown);
		transform.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, rotateTo), duration).SetEase(easing).SetAutoKill(true);
	}
}
