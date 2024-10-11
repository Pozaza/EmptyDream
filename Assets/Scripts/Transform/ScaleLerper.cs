using System.Collections;
using DG.Tweening;
using UnityEngine;

public class ScaleLerper : MonoBehaviour {
	public Vector3 maxScale;
	public float speed = .2f;
	public Ease easing = Ease.OutCubic;
	Vector3 _startScale;
	public bool resetValueOnDisable;
	public bool disableOnDisable;
	public float startCooldown;

	void Awake() => _startScale = transform.localScale;
	IEnumerator First() {
		yield return new WaitForSeconds(startCooldown);
		transform.DOScale(maxScale, speed).SetEase(easing).SetAutoKill(true);
	}
	void OnDisable() {
		if (resetValueOnDisable)
			transform.localScale = _startScale;
		if (disableOnDisable)
			enabled = false;
		transform.DOKill();
	}
	void OnEnable() => StartCoroutine(First());
	public void ToOriginalSize() => transform.DOScale(_startScale, speed).SetEase(easing).SetAutoKill(true);
	public void ToSize() => transform.DOScale(maxScale, speed).SetEase(easing).SetAutoKill(true);
	public void MultiplyScale(float multiply) {
		maxScale *= multiply;
		ToSize();
	}
	public void DivideScale(float divide) {
		maxScale /= divide;
		ToSize();
	}
}
