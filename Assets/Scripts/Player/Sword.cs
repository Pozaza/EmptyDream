using UnityEngine;

public class Sword : MonoBehaviour {
	public static Sword Instance;

	void Awake() => Instance = this;
}
