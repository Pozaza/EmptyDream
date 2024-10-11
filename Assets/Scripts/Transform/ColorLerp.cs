using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ColorLerp : MonoBehaviour {
	public Color end;
	public float time = .35f;
	public Ease easing = Ease.OutCubic;
	bool _isUI;
	public bool isText;
	public float timer;
	public bool resetValueOnDisable;
	public bool disableOnEnd;
	Color _startColor;

	void Awake() {
		TryGetComponent(out Image i);
		_isUI = i;
		_startColor = i ? i.color : TryGetComponent(out SpriteRenderer i2) ? i2.color : GetComponent<Text>().color;
	}
	void OnDisable() {
		if (_isUI && !isText) {
			TryGetComponent(out Image image);
			image.DOKill();
			image.color = _startColor;
		} else if (!_isUI && !isText) {
			TryGetComponent(out SpriteRenderer img);
			img.DOKill();
			img.color = _startColor;
		}
		if (isText && TryGetComponent(out Text t)) {
			t.DOKill();
			t.color = _startColor;
		}
	}
	void OnEnable() => StartCoroutine(Play());
	IEnumerator Play() {
		yield return new WaitForSeconds(timer);
		if (_isUI && !isText)
			GetComponent<Image>().DOColor(end, time).SetEase(easing).SetAutoKill(true).onComplete = CheckForDisabling;
		else if (!_isUI && !isText)
			GetComponent<SpriteRenderer>().DOColor(end, time).SetEase(easing).SetAutoKill(true).onComplete = CheckForDisabling;
		else if (TryGetComponent(out Text t))
			t.DOColor(end, time).SetEase(easing).SetAutoKill(true).onComplete = CheckForDisabling;
	}
	void CheckForDisabling() {
		if (disableOnEnd)
			enabled = false;
	}
}
