using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenLink : MonoBehaviour {
	public void Open(string link) => Application.OpenURL(link);
	public void QuitGame() => Application.Quit();
	public void GoToScene(string SceneName) => StartCoroutine(Scene(SceneName));
	public void GoToSceneNow(string SceneName) => SceneManager.LoadScene(SceneName);
	IEnumerator Scene(string SceneName) {
		yield return new WaitForSeconds(Random.Range(.5f, 2));
		SceneManager.LoadScene(SceneName);
	}
}
