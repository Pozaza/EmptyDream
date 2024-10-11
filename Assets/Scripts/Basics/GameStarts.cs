using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStarts : MonoBehaviour {
	public static GameStarts Instance;

	[Header("Настройки сцены")]
	public bool isSoundSwitch, companyonkaStart, interfaceStart, loadPlayerPos;

	[Header("Обучение")]
	public Text[] guide;

	[Header("Другое")]
	public GameObject[] save;
	readonly List<GameObj> _saveObjects = new();

	void Awake() => Instance = this;
	void Start() {
		if (guide.Length > 0) {
			guide[0].text = $"[<color=yellow>{Settings.Key(KeyBind.Jump)}</color>] - прыжок";
			guide[1].text = $"[<color=yellow>{Settings.Key(KeyBind.Left)}</color>] - ←\n[<color=yellow>{Settings.Key(KeyBind.Right)}</color>] - →";
			guide[2].text = $"Кнопка [<color=yellow>{Settings.Key(KeyBind.Interaction)}</color>]\nэто взаимодействие";
			guide[3].text = $"Зажмите [<color=yellow>{Settings.Key(KeyBind.Crouch)}</color>]\nчтобы пролезть";
		}
	}
	public void StartGame() {
		if (isSoundSwitch)
			SoundFadeInOut.Instance.PlayFade();
		PlayerScript.Instance.CanMove();
		if (companyonkaStart) {
			PrefabWeapon.Instance.Companyon(true);
			PrefabWeapon.Instance.CanShoot(true);
		}
		if (interfaceStart)
			Interface.Instance.ShowInterface();
		Menu.canDisplay = true;
		if (loadPlayerPos)
			Load();
	}
	public void Save() {
		_saveObjects.Clear();
		for (int i = 0; i < save.Length; i++)
			_saveObjects.Add(new GameObj(save[i]));

		PLS newPls = new() {
			health = PlayerHealth.Instance.health,
			playerPosX = PlayerHealth.Instance.transform.position.x,
			playerPosY = PlayerHealth.Instance.transform.position.y,
			objectsSaves = _saveObjects.ToArray(),
			sceneId = SceneManager.GetActiveScene().buildIndex,
			showInterface = Interface.Instance.GetComponent<Animator>().GetBool("showed")
		};

		SaveManager.Save(
			"playerLocationSaves",
			new PlayerLocationSaves(),
			data => {
				if (data.saves.Any(p => p.sceneId == SceneManager.GetActiveScene().buildIndex))
					data.saves.Remove(data.saves.Find(p => p.sceneId == SceneManager.GetActiveScene().buildIndex));
				data.saves.Add(newPls);
			}
		);
	}
	public void Load() {
		if (!SaveManager.Load(
			"playerLocationSaves",
			out PlayerLocationSaves data
		))
			return;

		if (data.saves.Any(pls => pls.sceneId == SceneManager.GetActiveScene().buildIndex)) {
			PLS pls = data.saves.Find(pls => pls.sceneId == SceneManager.GetActiveScene().buildIndex);

			PlayerHealth.Instance?.SetHealth(pls.health);
			PlayerHealth.Instance.respawnPlayer = new(pls.playerPosX, pls.playerPosY);
			PlayerScript.Instance.transform.position = new(pls.playerPosX, pls.playerPosY);

			if (pls.showInterface)
				Interface.Instance?.ShowInterface();

			for (int i = 0; i < save.Length; i++)
				pls.objectsSaves[i].SetProps(ref save[i]);
		}
	}
}