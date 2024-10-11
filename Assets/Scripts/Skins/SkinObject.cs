using UnityEngine;

[CreateAssetMenu(fileName = "Skin", menuName = "ED/New Skin", order = 3)]
public class SkinObject : ScriptableObject {
	public int price = 40;
	public Sprite skin;
	public bool isOpened;
}
