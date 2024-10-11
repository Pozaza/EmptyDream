using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Settings : MonoBehaviour {
	public static Settings Instance;
	[Header("Основное")]
	public GameObject buttonPrefab;
	public Text fpsText;
	public Slider[] Music_Sound_FPS;
	public Toggle[] Toggles;
	public ToggleGroup toggleQuality;
	public InputField resolutionX, resolutionY;
	Toggle[] tQ;
	float music = .35f, sounds = .55f, UI = .55f;
	bool _skipStartMovie;

	bool _isMSAA = true, _HDR = true, _postProcessing = true, _isFullscreen = true, _changing;
	[HideInInspector] public Quality _quality = Quality.High;
	int _resolution_x, _resolution_y, _fps = 60, _sync = 0;

	[Header("Аудио миксер")]
	public AudioMixer audioMixer;

	[Header("Клавиши управления")]
	public List<Key> keys;
	public static KeyCode Key(KeyBind keybind) => Instance.keys[(int)keybind].keyBind;
	public GameObject keybindsParent;
	KeyBind bindingKey = KeyBind.None;

	void Awake() {
		_resolution_x = Screen.currentResolution.width;

		ValidateResolution();

		if (Instance == null)
			Instance = this;

		tQ = toggleQuality.GetComponentsInChildren<Toggle>();

		if (!SaveManager.Has("settings"))
			SaveSettings();
	}
	void Update() {
		if (bindingKey != KeyBind.None)
			foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
				if (Input.GetKey(kcode)) {
					keys[(int)bindingKey] = new(keys[(int)bindingKey].name, kcode);

					Text t = keybindsParent.transform.GetChild((int)bindingKey).GetComponentsInChildren<Text>().Last();
					t.text = kcode.ToString();
					t.color = Color.white;

					bindingKey = KeyBind.None;
				}
	}

	public void SaveSettings() {
		SaveManager.Save(
			"settings",
			new SaveData(),
			data => {
				data.Fullscreen = _isFullscreen;
				data.VSync = _sync;
				data.skipStartMovie = _skipStartMovie;

				data.Music = music;
				data.Sounds = sounds;
				data.UI = UI;

				data.Resolution = new(_resolution_x, _resolution_y);
				data.Quality = _quality;
				data.FPS = _fps;

				data.isMSAA = _isMSAA;
				data.HDR = _HDR;
				data.PostProcessing = _postProcessing;

				data.keys = new List<Key>(keys);
			}
		);

		LoadSettings();
	}

	public void LoadSettings() {
		for (int i = 0; keybindsParent.transform.childCount < keys.Count; i++) {
			GameObject button = Instantiate(buttonPrefab, keybindsParent.transform, false);

			button.GetComponentsInChildren<Text>().First().text = keys[i].name;
			button.GetComponentsInChildren<Text>().Last().text = keys[i].keyBind.ToString();
			button.GetComponentInChildren<Button>().onClick.AddListener(delegate {
				Instance.BindKey((KeyBind)button.transform.GetSiblingIndex());
			});
		}

		if (!SaveManager.Load("settings", out SaveData data))
			return;

		ChangeVsync(data.VSync == 1);
		ChangeFullscreenMode(data.Fullscreen);
		ChangeShowMovie(data.skipStartMovie);

		ChangeMusic(data.Music);
		ChangeSound(data.Sounds);
		ChangeUI(data.UI);

		ChangeQuality((int)data.Quality);
		ChangeResolution(data.Resolution.Item1, data.Resolution.Item2);

		PostProcessing(data.PostProcessing);
		HDR(data.HDR);
		MSAA(data.isMSAA);

		ChangeFPS(data.FPS);

		for (int i = 0; i < keys.Count; i++) {
			keys[i] = data.keys[i];
			keybindsParent.transform.GetChild(i).GetComponentsInChildren<Text>().Last().text = data.keys[i].keyBind.ToString();
		}
	}

	public void Glitch() => FindObjectOfType<GlitchScript>()?.Glitch(.1f);

	public void ChangeMusic(float value) {
		audioMixer.SetFloat("MusicVolume", value);
		Music_Sound_FPS[0].value = value;
		music = value;
	}
	public void ChangeSound(float value) {
		audioMixer.SetFloat("SoundsVolume", value);
		Music_Sound_FPS[1].value = value;
		sounds = value;
	}
	public void ChangeUI(float value) {
		audioMixer.SetFloat("UIVolume", value);
		Music_Sound_FPS[2].value = value;
		UI = value;
	}
	public void ChangeFPS(float value) {
		Application.targetFrameRate = (int)value;
		Music_Sound_FPS[3].value = value;
		_fps = (int)value;
		fpsText.text = _fps.ToString();
	}
	public void MSAA(bool value) {
		Toggles[2].isOn = value;
		foreach (Camera cam in Camera.allCameras)
			cam.allowMSAA = value;
		_isMSAA = value;
	}
	public void HDR(bool value) {
		Toggles[3].isOn = value;
		foreach (Camera cam in Camera.allCameras)
			cam.allowHDR = value;
		_HDR = value;
	}
	public void PostProcessing(bool value) {
		Toggles[5].isOn = value;
		foreach (Camera cam in Camera.allCameras)
			foreach (Volume v in cam.GetComponents<Volume>())
				v.enabled = value;
		_postProcessing = value;
	}
	public void ChangeFullscreenMode(bool value) {
		Toggles[1].isOn = value;
		Screen.fullScreen = value;
		_isFullscreen = value;
	}
	public void ChangeVsync(bool value) {
		_sync = value ? 1 : 0;
		Toggles[0].isOn = value;
		QualitySettings.vSyncCount = _sync;
	}
	public void ChangeShowMovie(bool value) {
		Toggles[4].isOn = value;
		_skipStartMovie = value;
	}
	public void ChangeQuality(int value) {
		Quality quality = (Quality)Enum.ToObject(typeof(Quality), value);

		if (_quality == quality && tQ[2 - value].isOn || _changing)
			return;

		_changing = true;

		QualitySettings.SetQualityLevel(value);
		_quality = quality;
		tQ[2 - value].isOn = true;

		_changing = false;
	}
	public void ChangeResolutionX(string value) { // использует InputField для записи в переменную
		if (!int.TryParse(value, out int x))
			return;

		_resolution_x = x;
	}
	public void ValidateResolution() { // выполняется при окончании редактирования X
		_resolution_x = Math.Clamp(_resolution_x, 480, 3840);
		_resolution_y = _resolution_x * 9 / 16;

		resolutionX.text = _resolution_x.ToString();
		resolutionY.text = _resolution_y.ToString();
	}
	public void ChangeResolution(int x, int y) { // применение разрешения
		Screen.SetResolution(
			x,
			y,
			_isFullscreen
		);

		_resolution_x = x;
		_resolution_y = y;

		resolutionX.text = _resolution_x.ToString();
		resolutionY.text = _resolution_y.ToString();
	}
	public void QuitGame() => Application.Quit();
	public void GoToSceneNow(string sceneName) {
		if (SceneManager.GetActiveScene().name == "Обучение")
			GameManager.Instance.CompleteGuide();

		Menu.Instance?.TriggerMenu();
		SceneManager.LoadScene(sceneName);
	}
	public void RestartLvl() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	public void BindKey(KeyBind key) {
		keybindsParent.transform.GetChild((int)key).GetComponentsInChildren<Text>().Last().color = Color.yellow;
		bindingKey = key;
	}
}

public enum KeyBind {
	Left, Right,
	Jump, Crouch, Lay,
	Spark, Interaction,
	Ability1, Ability2,
	Dialog, Menu,
	None = -1
}

[Serializable]
public enum Quality {
	Low, Medium, High
}

[Serializable]
public class Key {
	public string name;
	public KeyCode keyBind;

	public Key(string name, KeyCode keyBind) {
		this.name = name;
		this.keyBind = keyBind;
	}
}