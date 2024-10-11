using System.Collections;
using DG.Tweening;
using UnityEngine;

public class SizeLerper : MonoBehaviour {
	public Vector2 maxScale;
	public float speed = .2f;
	public Ease easing = Ease.OutCubic;
	public bool resetValueOnDisable = false;
	public float startCooldown;
	RectTransform Rect => GetComponent<RectTransform>();
	Vector2 startScale;

	void Awake() => startScale = Rect.sizeDelta;
	IEnumerator First() {
		yield return new WaitForSeconds(startCooldown);
		ToSize();
	}
	void OnDisable() {
		if (resetValueOnDisable)
			Rect.sizeDelta = startScale;
		Rect.DOKill();
	}
	void OnEnable() => StartCoroutine(First());
	public void ToOriginalSize() => Rect.DOSizeDelta(startScale, speed).SetEase(easing).SetAutoKill(true);
	public void ToSize() => Rect.DOSizeDelta(maxScale, speed).SetEase(easing).SetAutoKill(true);
}
