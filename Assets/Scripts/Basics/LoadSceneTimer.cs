using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneTimer : MonoBehaviour {
	public float timeToChangeScene;

	void Awake() {
		if (SaveManager.Load("settings", out SaveData data) && data.skipStartMovie)
			SceneManager.LoadScene("Menu");
		else
			StartCoroutine(Interval());
	}
	IEnumerator Interval() {
		yield return new WaitForSeconds(timeToChangeScene);

		if (!SaveManager.Has("settings"))
			SceneManager.LoadScene("FirstTime");
		else
			SceneManager.LoadScene("Menu");
	}
}
