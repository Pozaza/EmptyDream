using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using DG.Tweening;

public class Rotater : MonoBehaviour {
	public enum ROTATIONAXIS {
		xAxis,
		yAxis,
		zAxis
	}
	public float maxRotationAngle;
	public float period = 1;
	public bool flip;
	public ROTATIONAXIS rotationAxis;
	[MinMaxSlider(0.5f, 10f)]
	public Vector2 randomStopTime = Vector2.zero;
	[MinMaxSlider(3f, 60f)]
	public Vector2 randomStopFreq = Vector2.zero;

	float myTime;
	bool canRotate = true;


	void Start() {
		if (randomStopTime == Vector2.zero || randomStopFreq == Vector2.zero)
			return;

		StartCoroutine(RandomStop());
	}
	void Update() {
		if (!canRotate)
			return;

		float phase = Mathf.Sin(myTime / period) * (flip ? -1 : 1);
		myTime += Time.deltaTime;

		switch (rotationAxis) {
			case ROTATIONAXIS.xAxis:
				transform.DOLocalRotateQuaternion(Quaternion.Euler(phase * maxRotationAngle, 0, 0), 1);
				break;
			case ROTATIONAXIS.yAxis:
				transform.DOLocalRotateQuaternion(Quaternion.Euler(0, phase * maxRotationAngle, 0), 1);
				break;
			case ROTATIONAXIS.zAxis:
				transform.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, phase * maxRotationAngle), 1);
				break;
		}
	}
	IEnumerator RandomStop() {
		yield return new WaitForSeconds(Random.Range(randomStopFreq.x, randomStopFreq.y));
		canRotate = false;
		yield return new WaitForSeconds(Random.Range(randomStopTime.x, randomStopTime.y));
		canRotate = true;
		StartCoroutine(RandomStop());
	}
}
