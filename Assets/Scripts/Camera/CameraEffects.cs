using UnityEngine;

public class CameraEffects : MonoBehaviour {
	public void StartEffect(string name) => GetComponent<Animator>().SetTrigger(name);
}
