using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Message : MonoBehaviour {
	public static Message Instance;
	public Text txt;
	public SpriteRenderer img;
	public float zadershka;
	public AudioSource aaudio;
	public Color[] colors;

	void Awake() => Instance = this;
	void Start() {
		txt.color = colors[0];
		img.color = colors[1];
	}
	public void MessageShow(string text) {
		txt.text = text;
		GlitchScript.Instance.Glitch(.1f);
		aaudio.Play();
		txt.color = colors[2];
		img.color = colors[3];
		StartCoroutine(Fadee());
	}

	IEnumerator Fadee() {
		yield return new WaitForSeconds(zadershka);
		txt.color = colors[0];
		img.color = colors[1];
	}
}
