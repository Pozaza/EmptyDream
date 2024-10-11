using UnityEngine;

[CreateAssetMenu(fileName = "Achievement", menuName = "ED/New Achievement", order = 1)]
public class AchievementScriptableObjects : ScriptableObject {
	public Sprite picture;
	public string text;
	public SkinObject skin;
}
