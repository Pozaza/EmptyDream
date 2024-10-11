using System.Collections.Generic;
using UnityEngine;

public class ResetWhenDied : MonoBehaviour {
	public GameObject[] objects;
	readonly List<GameObj> saved = new();

	void Start() {
		for (int i = 0; i < objects.Length; i++)
			saved.Add(new GameObj(objects[i]));

		PlayerHealth.Instance.RespawnEvent += ResetObjects;
	}
	void ResetObjects() {
		for (int i = 0; i < saved.Count; i++)
			saved[i].SetProps(ref objects[i]);
	}
}
