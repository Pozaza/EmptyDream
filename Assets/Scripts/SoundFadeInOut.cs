using System.Collections;
using UnityEngine;

public class SoundFadeInOut : MonoBehaviour {
	public static SoundFadeInOut Instance;
	public AudioSource firstAudio, secondAudio;
	public AudioClip[] musics;
	public float fadeInSec;

	AudioSource _currentMainAudio;
	int _currentClip;
	float firstAudioMax, secondAudioMax;

	void Awake() => Instance = this;
	void Start() {
		_currentMainAudio = firstAudio;
		firstAudioMax = firstAudio.volume;
		secondAudioMax = secondAudio.volume;
	}
	public void PlayFade() {
		if (++_currentClip >= musics.Length)
			_currentClip = 0;

		StartCoroutine(PlayFadeCoroutine());
	}
	IEnumerator PlayFadeCoroutine() {
		AudioSource newMainAudio = _currentMainAudio == firstAudio ? secondAudio : firstAudio;
		newMainAudio.clip = musics[_currentClip];
		newMainAudio.volume = 0;
		newMainAudio.Play();

		float volume = 1f / fadeInSec * Time.deltaTime;
		while (_currentMainAudio.volume > 0) {
			_currentMainAudio.volume -= volume;
			if (newMainAudio.volume < (newMainAudio == firstAudio ? firstAudioMax : secondAudioMax))
				newMainAudio.volume += volume;
			yield return null;
		}

		_currentMainAudio = newMainAudio;
	}
}