using UnityEngine;

public class Notifications : MonoBehaviour {
	public static Notifications Instance;
	public GameObject achievements, magazine;
	void Awake() {
		Instance = this;
		if (SaveManager.Load("notifications", out NotificationEnabled data) && achievements != null) {
			achievements.SetActive(data.achievement);
			magazine.SetActive(data.magazine);
		} else if (achievements != null)
			SaveNotifications();
	}
	public void EnableNotification(string type) {
		if (type == "achievement")
			achievements.SetActive(true);
		else if (type == "magazine")
			magazine.SetActive(true);
		SaveNotifications();
	}
	public void ChangeNotifications(int achievements = -1, int magazine = -1) {
		SaveManager.Load("notifications", out NotificationEnabled data);

		bool achievements_bool = achievements == -1 ? data.achievement : achievements != 0;
		bool magazine_bool = magazine == -1 ? data.magazine : magazine != 0;

		SaveManager.Save(
			"notifications",
			new NotificationEnabled() {
				achievement = achievements_bool,
				magazine = magazine_bool
			}
		);
	}
	public void DisableNotification(string type) {
		if (type == "achievement")
			achievements.SetActive(false);
		else if (type == "magazine")
			magazine.SetActive(false);
		SaveNotifications();
	}
	void SaveNotifications() {
		SaveManager.Save(
			"notifications",
			new NotificationEnabled() {
				achievement = achievements.activeSelf,
				magazine = magazine.activeSelf
			}
		);
	}
}
