using UnityEngine;

public class PlayerSounds : MonoBehaviour {
	public static PlayerSounds Instance;
	public AudioSource audioManager;
	public AudioClip jump;

	void Awake() => Instance = this;
	public void Jump() => audioManager.PlayOneShot(jump, 1);
}