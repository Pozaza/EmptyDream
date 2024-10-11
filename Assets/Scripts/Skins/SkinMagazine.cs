using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SkinMagazine : MonoBehaviour {
	public static SkinMagazine Instance;
	public Transform skinList;
	public List<SkinObject> skins = new();
	public Text unshes, unshesShadow;
	public Text notEnough;
	public Image notEnoughBg;
	public Sprite[] sprites;
	public GameObject skinCardPrefab;
	[HideInInspector] public int unshesCount = 0;
	List<GameObject> skinsGameObjects = new();
	List<SkinObject> openedSkins = new();
	int _notEnoughTimes;
	SkinObject _selectedSkin;
	bool loaded;

	void Awake() => Instance = this;
	public void LoadSkins() {
		if (skinsGameObjects.Count > 0 || loaded)
			return;

		loaded = true;

		if (SaveManager.Load("skins", out SkinsOpened data)) {
			openedSkins = data.skin.ToList().ConvertAll(item => skins[item]);

			if (skinList == null) {
				Debug.LogWarning("Нету скинов или не назначен родитель.");
				return;
			}

			SaveManager.Load("skinApply", out SkinApplyed data2);

			for (int i = 0; i < skins.Count; i++) {
				GameObject obj = Instantiate(skinCardPrefab, Vector3.zero, Quaternion.identity, skinList);
				GameObject buy = obj.FindObj("buy");

				buy.FindObj("Валюта").FindObj<Text>("Цена").text = skins[i].price.ToString();
				obj.FindObj<Image>("image").sprite = skins[i].skin;
				buy.FindObj<Button>("buyButton").onClick.AddListener(() =>
					Instance.BuySkin(Instance.skins[obj.transform.GetSiblingIndex()])
				);
				obj.FindObj("buyed").FindObj("applyButton").FindObj<Button>("button").onClick.AddListener(() =>
					Instance.SelectSkin(Instance.skins[obj.transform.GetSiblingIndex()])
				);
				obj.GetComponent<Image>().color = Color.red;

				skinsGameObjects.Add(obj);

				UnlockSkin(obj, openedSkins.Contains(skins[i]), Achievements.Instance.achievements.Any(item => item.skin == skins[i]));
			}

			SelectSkin(skins[data2.skin]);
		} else
			SaveSkin(true);

		if (SaveManager.Load("unshes", out SavedUnshes data3)) {
			unshesCount = data3.unshes;
			unshes.text = unshesCount.ToString();
			if (unshesShadow != null)
				unshesShadow.text = unshesCount.ToString();
		} else
			SaveUnshes();

		Unshes.Instance?.LoadTrader();
	}
	public void SaveUnshes() {
		SaveManager.Save(
			"unshes",
			new SavedUnshes() {
				unshes = unshesCount
			}
		);

		if (unshes != null)
			unshes.text = unshesCount.ToString();
		if (unshesShadow != null)
			unshesShadow.text = unshesCount.ToString();
	}
	public void AddUnshes(int value) {
		unshesCount += value;
		SaveUnshes();
	}
	public bool DecraseUnshes(int value) {
		if (unshesCount - value >= 0) {
			unshesCount -= value;
			return true;
		} else
			return false;
	}
	public void GetSkin(SkinObject skin) {
		LoadSkins();

		if (skinList == null) {
			Debug.LogWarning("Нету листа скинов..");
			return;
		}

		if (openedSkins.Contains(skin)) {
			Debug.LogWarning("Скин уже открыт!");
			return;
		}

		openedSkins.Add(skin);
		SaveSkin(false);

		if (skins.Count > 0) {
			if (skinList != null)
				UnlockSkin(skinsGameObjects[skins.IndexOf(skin)], true, true);
			Notifications.Instance?.EnableNotification("magazine");
		} else
			Notifications.Instance?.ChangeNotifications(-1, 1);
	}
	public void BuySkin(SkinObject skin) {
		if (DecraseUnshes(skin.price)) {
			openedSkins.Add(skin);
			UnlockSkin(skinsGameObjects[skins.IndexOf(skin)], true, true);
			SaveSkin(false);
		} else {
			StopAllCoroutines();
			notEnough.GetComponent<ColorLerp>().enabled = false;
			notEnoughBg.GetComponent<ColorLerp>().enabled = false;
			notEnoughBg.GetComponent<Image>().raycastTarget = true;
			StartCoroutine(NotEnough(.5f));
		}
	}

	/// <param name="isFirstTime">В первый раз</param>
	public void SaveSkin(bool isFirstTime) {
		if (!FunManager.Instance)
			return;

		if (isFirstTime) {
			openedSkins = new List<SkinObject>() { skins[0] };

			if (skinList == null) {
				Debug.LogWarning("Нету листа скинов!");
				_selectedSkin = skins[0];
			} else {
				for (int i = 0; i < skins.Count; i++) {
					GameObject obj = Instantiate(skinCardPrefab, Vector3.zero, Quaternion.identity, skinList);
					obj.FindObj("buy").FindObj("Валюта").FindObj<Text>("Цена").text = skins[i].price.ToString();
					obj.FindObj<Image>("image").sprite = skins[i].skin;
					obj.FindObj("buy").FindObj<Button>("buyButton").onClick.AddListener(delegate {
						BuySkin(skins[obj.transform.GetSiblingIndex()]);
					});
					obj.FindObj("buyed").FindObj("applyButton").FindObj<Button>("button").onClick.AddListener(() =>
						SkinMagazine.Instance.SelectSkin(SkinMagazine.Instance.skins[obj.transform.GetSiblingIndex()])
					);
					obj.GetComponent<Image>().color = Color.red;
					skinsGameObjects.Add(obj);

					UnlockSkin(obj, openedSkins.Contains(skins[i]), Achievements.Instance.achievements.Any(item => item.skin == skins[i]));

					SelectSkin(skins[0]);
				}
			}
		}

		SaveManager.Save(
			"skins",
			new SkinsOpened() {
				skin = openedSkins.ConvertAll(item => skins.IndexOf(item)).ToArray()
			}
		);
	}
	public void UnlockSkin(GameObject id, bool opened, bool isAchivement) {
		id.FindObj("lock").SetActive(!opened);
		id.FindObj("buy").SetActive(!opened && !isAchivement);

		if (opened) {
			GameObject buyed = id.FindObj("buyed");

			buyed.SetActive(true);

			if (_selectedSkin == null)
				_selectedSkin = skins[0];

			Image state = buyed.FindObj("Валюта").FindObj<Image>("Индикатор");

			state.sprite = skinsGameObjects[skins.IndexOf(_selectedSkin)] != id ? sprites[0] : sprites[1];
			state.color = skinsGameObjects[skins.IndexOf(_selectedSkin)] != id ? Color.yellow : Color.green;
			buyed.FindObj("applyButton").SetActive(skinsGameObjects[skins.IndexOf(_selectedSkin)] != id);
			id.GetComponent<Image>().color = skinsGameObjects[skins.IndexOf(_selectedSkin)] != id ? Color.yellow : Color.red;
		}
	}
	public void SelectSkin(SkinObject skin) {
		SkinObject previous = _selectedSkin;
		_selectedSkin = skin;

		SaveManager.Save(
			"skinApply",
			new SkinApplyed() {
				skin = skins.IndexOf(_selectedSkin)
			}
		);

		if (skinList == null) {
			Debug.LogWarning("Нету листа скинов, ничего не выбираю..");
			return;
		}

		if (previous != null) {
			Transform buy = skinsGameObjects[skins.IndexOf(previous)].transform.Find("buyed");
			buy.FindObj("applyButton").SetActive(true);
			buy.FindObjChild<Image>("Валюта").sprite = sprites[0];
			buy.FindObjChild<Image>("Валюта").color = Color.yellow;
			skinsGameObjects[skins.IndexOf(previous)].GetComponent<Image>().color = Color.yellow;
		}

		Transform buyed = skinsGameObjects[skins.IndexOf(_selectedSkin)].transform.Find("buyed");

		buyed.FindObj("applyButton").SetActive(false);
		buyed.FindObjChild<Image>("Валюта").sprite = sprites[1];
		buyed.FindObjChild<Image>("Валюта").color = Color.green;
		skinsGameObjects[skins.IndexOf(_selectedSkin)].GetComponent<Image>().color = Color.green;
	}
	IEnumerator NotEnough(float value) {
		notEnough.color = Color.red;
		notEnoughBg.color = new Color(0, 0, 0, .75f);
		if (PlayerPrefs.HasKey("Punishment") || PlayerPrefs.GetInt("Punishment") == 0)
			notEnough.text = "Недостаточно унш";
		else {
			_notEnoughTimes++;

			switch (_notEnoughTimes) {
				case 10:
					notEnough.text = "Ты не понял?";
					value = 1f;
					break;
				case 11:
					notEnough.text = "Я же тебе сказал что у тебя недостаточно унш!";
					value = 3f;
					break;
				case 12:
					notEnough.text = "Не раздражай меня";
					value = 2f;
					break;
				case 13:
					notEnough.text = "Ладно..";
					value = 1f;
					break;
				case 14:
					notEnough.text = "Если ты так хочешь";
					value = 1f;
					break;
				case 15:
					notEnough.text = "Я тебе это припомню..";
					value = 2f;
					PlayerPrefs.SetInt("Punishment", 0);
					break;
			}
		}
		yield return new WaitForSeconds(value);
		notEnoughBg.GetComponent<Image>().raycastTarget = false;
		notEnoughBg.GetComponent<ColorLerp>().enabled = true;
		notEnough.GetComponent<ColorLerp>().enabled = true;
	}
	public void AcceptPunishment() => PlayerPrefs.DeleteKey("Punishment");
	public void CheckChanges(SkinObject skin, AchievementScriptableObjects achievement) {
		SaveManager.Save(
			"getThing",
			new GetThing() {
				skin = skins.IndexOf(skin),
				achievement = Achievements.Instance.achievements.IndexOf(achievement)
			}
		);

		if (skin != null)
			GetSkin(skin);
	}
}
