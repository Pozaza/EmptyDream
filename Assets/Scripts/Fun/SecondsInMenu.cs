using UnityEngine;
using UnityEngine.UI;

public class SecondsInMenu : MonoBehaviour {
	public Text text;
	float _time;

	void Start() {
		if (SaveManager.Load("seconds", out TimeInMenu data)) {
			_time = (int)data.time;
			text.text = $"Лаэр насчитал:\n<color=yellow><size=150>{(int)data.time}</size></color> сек.";
		} else
			SaveTime();
	}
	void Update() {
		_time += Time.deltaTime;
		text.text = $"Лаэр насчитал:\n<color=yellow><size=150>{(int)_time}</size></color> сек.";
	}
	void OnDisable() => SaveTime();
	public void SaveTime() {
		SaveManager.Save(
			"seconds",
			new TimeInMenu() {
				time = _time
			}
		);
	}
}
