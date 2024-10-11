using System.Collections;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class Slerper : MonoBehaviour {
	public bool use2positions;
	public Vector3 firstTransformOffset;
	Vector3 _pointToTransform;
	public float speed = .1f;
	[ShowIf("use2positions")] public Vector3 secondTransformOffset;
	public bool resetValueOnDisable;
	public bool repeat;
	[ShowIf("repeat")] public float timeBetweenTransforms = 2f;

	void Awake() {
		if (!use2positions)
			secondTransformOffset = transform.localPosition;
		_pointToTransform = firstTransformOffset;
	}
	void Start() {
		if (repeat)
			StartCoroutine(RepeatTransform());
	}
	public IEnumerator RepeatTransform() {
		while (repeat) {
			yield return new WaitForSeconds(timeBetweenTransforms);
			_pointToTransform = secondTransformOffset;
			yield return new WaitForSeconds(timeBetweenTransforms);
			_pointToTransform = firstTransformOffset;
		}
	}
	void OnDisable() {
		if (resetValueOnDisable)
			transform.localPosition = secondTransformOffset;
		transform.DOKill();
	}
	void OnEnable() {
		transform.DOLocalMove(_pointToTransform, speed).SetEase(Ease.OutCubic).SetAutoKill(true);
		if (repeat)
			StartCoroutine(RepeatTransform());
	}
}
