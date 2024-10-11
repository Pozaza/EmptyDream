using UnityEngine;

class RandomMusic : MonoBehaviour {
	static AudioClip[] clips;
	public AudioSource Music;
	public string FolderName;

	void Awake() => clips = Resources.LoadAll<AudioClip>(FolderName);
	void Start() => RandomM();
	public void RandomM() {
		Music.clip = clips[Random.Range(0, clips.Length)];
		Music.Play();
	}
}