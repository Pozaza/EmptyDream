using NaughtyAttributes;
using UnityEngine;

public class SoundArea : MonoBehaviour {
    [Range(0f, 1f)]
    public float maxVolume;
    [MinMaxSlider(0f, 50f)]
    public Vector2 distance = new(1f, 5f);
    public AudioSource audioSource;
    void Awake() {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }
    void Update() {
        audioSource.volume = (1 - ((Vector2.Distance(transform.position, PlayerScript.Instance.transform.position) - distance.x) / (distance.y - distance.x))) * maxVolume;
    }
}
