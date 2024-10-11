using UnityEngine;
using UnityEngine.UI;

public class Levels : MonoBehaviour {
	public GameObject slidersRoot;
	int[] _levels = new int[8];
	Slider[] _sliders;
	public static int avaibleLevels;
	public Text avaibleLevelsText;

	void Awake() => _sliders = slidersRoot.GetComponentsInChildren<Slider>();

	void Start() {
		if (SaveManager.Load("levels", out LevelsCharacters data)) {
			_levels = data.levels;
			avaibleLevels = data.available;

			for (int i = 0; i < 8; i++) {
				_sliders[i].value = _levels[i];
				_sliders[i].transform.parent.parent.GetComponentInChildren<Button>().interactable = _sliders[i].value < 5 && avaibleLevels > 0;
			}
		} else
			SaveLevels();
		ChangeText();
	}
	public void SaveLevels() {
		SaveManager.Save(
			"levels",
			new LevelsCharacters() {
				levels = _levels,
				available = avaibleLevels
			}
		);
	}

	void ChangeText() => avaibleLevelsText.text = avaibleLevels > 0 ? $"Доступно очков навыка: {avaibleLevels}" : "Нету очков навыка..";

	public void AddLevel(int value) {
		_levels[value]++;
		_sliders[value].value++;
		avaibleLevels--;
		for (int i = 0; i < 8; i++)
			slidersRoot.GetComponentsInChildren<Button>()[i].interactable = _sliders[i].value < 5 && avaibleLevels > 0;
		SaveLevels();
		ChangeText();
	}
}
