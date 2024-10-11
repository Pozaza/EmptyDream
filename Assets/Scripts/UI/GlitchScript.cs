using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GlitchScript : MonoBehaviour {
	public static GlitchScript Instance;
	Image img;
	AudioSource src;

	void Awake() {
		Instance = this;

		img = GetComponentInChildren<Image>();
		src = GetComponentInChildren<AudioSource>();
	}
	public void Glitch(float time) {
		StopAllCoroutines();
		img.GetComponent<Image>().enabled = false;

		StartCoroutine(StartGlitch(time));
		img.GetComponent<Image>().enabled = true;
		src.PlayOneShot(src.clip, .55f);
	}
	IEnumerator StartGlitch(float time) {
		yield return new WaitForSeconds(time);
		img.GetComponent<Image>().enabled = false;
	}
}
