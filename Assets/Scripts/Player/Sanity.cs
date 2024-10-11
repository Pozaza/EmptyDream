using System;
using UnityEngine;
using UnityEngine.Rendering;

public class Sanity : MonoBehaviour {
	public static Sanity Instance;

	[Range(0f, 1f)]
	public float sanityMeter = 0f, easing = 0.1f;
	public Volume volume;
	float currentValue;
	Rotater rotater;
	VHSEffect vhsEffect;
	CRTAperture crtAperture;

	void Awake() {
		Instance = this;
		currentValue = sanityMeter;
		rotater = CameraFollow.Instance.GetComponent<Rotater>();
		volume.profile.TryGet(out vhsEffect);
		volume.profile.TryGet(out crtAperture);
	}
	void Update() {
		currentValue = Mathf.SmoothStep(currentValue, sanityMeter, easing);
		vhsEffect.fade.Override(currentValue);
		crtAperture.fade.Override(currentValue);
		if (rotater != null)
			rotater.maxRotationAngle = 20f * sanityMeter;
	}
	public void ChangeSanity(float value) => sanityMeter = value;
	public void ChangeEasing(float value) => easing = value;
}
