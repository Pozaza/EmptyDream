using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Achievements : MonoBehaviour {
	public static Achievements Instance;
	public Transform achievementsParent;
	public GameObject achievementCardPrefab;
	public List<AchievementScriptableObjects> achievements;
	public bool isSilent;
	List<GameObject> achievementsGameObjects = new();
	List<AchievementScriptableObjects> openedAchievements = new();

	void Awake() {
		Instance = this;

		if (SaveManager.Load("achievements", out AchievementsOpened data)) {
			if (achievementsGameObjects.Count > 0)
				return;

			if (data.achievement.Length > 0)
				openedAchievements = data.achievement.ToList().ConvertAll(item => achievements[item]);

			if (achievementsParent == null)
				return;

			for (int i = 0; i < achievements.Count; i++) {
				GameObject obj = Instantiate(achievementCardPrefab, Vector3.zero, Quaternion.identity, achievementsParent);
				obj.FindObj<Text>("text").text = achievements[i].text;
				obj.FindObj<Image>("image").sprite = achievements[i].picture;
				achievementsGameObjects.Add(obj);

				SetActiveAchievement(achievementsGameObjects[i], openedAchievements.Contains(achievements[i]));
			}
		} else
			SaveAchievements(true);

		if (SaveManager.Load("getThing", out GetThing data2) && achievements.Count > 0) {
			if (data2.achievement != -1) {
				isSilent = true;
				GetAchievement(achievements[data2.achievement]);
			}

			SkinMagazine.Instance.CheckChanges(data2.skin == -1 ? null : SkinMagazine.Instance.skins[data2.skin], data2.achievement == -1 ? null : achievements[data2.achievement]);
		}
	}
	public void GetAchievement(AchievementScriptableObjects achievement) {
		if (openedAchievements.Contains(achievement))
			return;

		if (!isSilent)
			StartCoroutine(GetAchievement(achievement.picture, achievement.text));
		else
			isSilent = false;

		openedAchievements.Add(achievement);

		SaveAchievements(false);

		if (achievements.Count > 0) {
			if (achievementsGameObjects.Count > 0)
				SetActiveAchievement(achievementsGameObjects[achievements.IndexOf(achievement)], true);
			Notifications.Instance.EnableNotification("achievement");
		} else
			Notifications.Instance.ChangeNotifications(1);
	}
	public void SaveAchievements(bool firstTime) {
		if (firstTime) {
			if (achievementsParent == null)
				return;

			for (int i = 0; i < achievements.Count; i++) {
				GameObject obj = Instantiate(achievementCardPrefab, Vector3.zero, Quaternion.identity, achievementsParent);
				obj.FindObj<Text>("text").text = achievements[i].text;
				obj.FindObj<Image>("image").sprite = achievements[i].picture;
				achievementsGameObjects.Add(obj);

				SetActiveAchievement(achievementsGameObjects[i], openedAchievements.Contains(achievements[i]));
			}
		}

		SaveManager.Save(
			"achievements",
			new AchievementsOpened() {
				achievement = openedAchievements.Count == 0 ? new int[0] : openedAchievements.ConvertAll(item => achievements.IndexOf(item)).ToArray()
			}
		);
	}
	public void SetActiveAchievement(GameObject id, bool state) {
		id.FindObj("lock").SetActive(!state);
		id.FindObj("salute").SetActive(state);
		id.GetComponent<ScaleLerper>().enabled = state;
		if (state) {
			id.GetComponent<Image>().color = Color.green;
			id.FindObj<Text>("text").color = Color.green;
		}
	}
	IEnumerator GetAchievement(Sprite picture, string text) { // уведомление сверху
		transform.FindObj<Image>("picture").sprite = picture;
		transform.FindObj<Text>("text").text = text;
		transform.TryGetComponent(out TransformLerper l);
		if (!l.enabled)
			l.enabled = true;
		else
			l.ToPos();
		yield return new WaitForSeconds(3.5f);
		l.ToOriginalPos();
	}
}