using UnityEngine;

[CreateAssetMenu(fileName = "DialogColorAndFont", menuName = "Dialog color and font")]
public class DialogColorAndFont : ScriptableObject {
	public Color color;
	public AudioClip sound;
	public Color GetColor() => color;
	public AudioClip GetSound() => sound;
}
