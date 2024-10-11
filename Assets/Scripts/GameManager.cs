using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	public static GameManager Instance;
	[HideInInspector] public AudioSource sounds, uiSounds;
	public static bool cutscene = false, interactiveCutscene = false;

	void Awake() {
		cutscene = false;
		interactiveCutscene = false;

		FunManager.Instance?.SceneChanged?.Invoke();

		sounds = this.FindTag<AudioSource>("Sounds");
		uiSounds = this.FindTag<AudioSource>("UISounds");

		Instance = this;
	}
	public void CompleteGuide() => PlayerPrefs.SetInt("guide", 0);
	public void Glitch() => FindObjectOfType<GlitchScript>()?.Glitch(.1f);
	public void ToggleSettings() {
		Menu.Instance?.TriggerSettingsTab();
		Menu.Instance?.TriggerMenu();
	}
	public void GoToSceneNow(string sceneName) => SceneManager.LoadScene(sceneName);
	public void GoToScene(string sceneName) => StartCoroutine(Scene(sceneName));
	IEnumerator Scene(string sceneName) {
		yield return new WaitForSeconds(Random.Range(5f, 8f));
		SceneManager.LoadScene(sceneName);
	}
}