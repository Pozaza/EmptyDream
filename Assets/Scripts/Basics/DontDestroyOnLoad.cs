using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour {
	public GameObject[] objects;
	void Start() {
		foreach (GameObject element in objects)
			DontDestroyOnLoad(element);
	}
	public void RemoveCopies() {
		foreach (GameObject element in objects)
			Destroy(element);
	}
}