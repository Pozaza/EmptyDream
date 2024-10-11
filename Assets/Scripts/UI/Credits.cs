using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour {
	void OnEnable() {
		if (Input.GetKeyDown(Settings.Key(KeyBind.Dialog)))
			ReloadGame();
	}
	void OnDisable() {
		if (Input.GetKeyDown(Settings.Key(KeyBind.Dialog)))
			ReloadGame();
	}
	public void ReloadGame() => SceneManager.LoadScene(1);
}