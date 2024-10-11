using System.Collections;
using DG.Tweening;
using UnityEngine;

public class TransformLerper : MonoBehaviour {
	public Vector3 endPos;
	public float duration = .35f;
	public Ease easing = Ease.OutCubic;
	Vector3 _startPos, _target;
	public bool resetValueOnDisable;
	public bool disableOnDisable;
	public float startCooldown;

	void Awake() {
		_startPos = transform.localPosition;
		_target = endPos;
	}
	void OnDisable() {
		if (resetValueOnDisable)
			transform.localPosition = _startPos;
		if (disableOnDisable)
			enabled = false;
		transform.DOKill();
	}
	public void ToOriginalPos() {
		_target = _startPos;
		transform.DOLocalMove(_target, duration).SetEase(easing).SetAutoKill(true);
	}
	public void ToPos() {
		_target = endPos;
		transform.DOLocalMove(_target, duration).SetEase(easing).SetAutoKill(true);
	}
	void OnEnable() => StartCoroutine(First());
	IEnumerator First() {
		yield return new WaitForSeconds(startCooldown);
		transform.DOLocalMove(_target, duration).SetEase(easing).SetAutoKill(true);
	}
}
