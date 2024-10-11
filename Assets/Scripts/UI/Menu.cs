using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {
	public static Menu Instance;

	public static bool canDisplay;
	public static bool menuDisplayed = false;
	GameObject menu;

	void Awake() {
		menuDisplayed = false;
		menu = transform.GetChild(0).gameObject;
		menu.SetActive(false);

		if (Instance == null)
			Instance = this;
	}
	void Update() {
		if (canDisplay && Input.GetKeyDown(Settings.Key(KeyBind.Menu)) && SceneManager.GetActiveScene().name !=
		"Menu")
			TriggerMenu();
	}
	public void TriggerMenu() {
		menuDisplayed = !menuDisplayed;
		menu.SetActive(menuDisplayed);
		if (menuDisplayed)
			PlayerScript.Instance?.CantMove();
		else
			PlayerScript.Instance?.CanMove();
		PlayerScript.Instance?.Stop();
		if (PrefabWeapon.Instance != null)
			PrefabWeapon.Instance.inMenu = menuDisplayed;
	}
	public void TriggerSettingsTab() {
		if (menu.GetChildObj(0).gameObject.activeSelf && SceneManager.GetActiveScene().name ==
		"Menu") {
			TriggerMenu();
			GlitchScript.Instance?.Glitch(.1f);
		}

		menu.GetChildObj(0).gameObject.SetActive(!menu.GetChildObj(0).gameObject.activeSelf);
		menu.GetChildObj(1).gameObject.SetActive(!menu.GetChildObj(1).gameObject.activeSelf);
	}
	public void Cutscene(bool val) {
		if (menuDisplayed)
			TriggerMenu();
		canDisplay = val;
	}
}