using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using RaSt;
using UnityEngine;

public class SaveManager : MonoBehaviour {
	public static bool Load<T>(string fileName, out T outData) {
		if (!Has(fileName)) {
			Debug.LogWarning("Файл сохранения не существует!");
			outData = default;
			return false;
		}

		FileStream file = File.OpenRead(fileName.Path());

		if (file.Length == 0) {
			Debug.LogWarning("Файл сохранения пустой!");
			file.Close();
			outData = default;
			return false;
		}

		outData = (T)new BinaryFormatter().Deserialize(file);
		file.Close();

		return true;
	}
	public static void Save<T>(string fileName, T data, Action<T> func = null, bool notification = false) {
		if (notification)
			Interface.Instance?.ShowSaveNotification();

		FileStream file;

		if (!Has(fileName))
			file = File.Create(fileName.Path());
		else
			file = File.OpenWrite(fileName.Path());

		if (file.Length > 0 && func != null && file.CanRead)
			Load(fileName, out data);

		func?.Invoke(data);

		new BinaryFormatter().Serialize(file, data);
		file.Close();
	}
	public static bool Has(string fileName) => File.Exists(fileName.Path());
}

namespace RaSt {
	public static class Extensions {
		public static string Path(this string path) => Application.persistentDataPath + $"/{path}.dat";
	}
}

[Serializable]
class SaveData {
	public bool isMSAA = true;
	public bool Fullscreen = true;
	public bool HDR = true;
	public bool skipStartMovie = false;
	public bool PostProcessing = true;
	public int VSync = 1;
	public Tuple<int, int> Resolution;
	public Quality Quality = Quality.High;
	public float Music = .35f;
	public float Sounds = .55f;
	public float UI = .55f;
	public int FPS = 60;
	public List<Key> keys = new();
}

[Serializable]
class SavedUnshes {
	public int unshes;
}

[Serializable]
class TimeInMenu {
	public float time;
}

[Serializable]
class AchievementsOpened {
	public int[] achievement;
}

[Serializable]
class SkinsOpened {
	public int[] skin;
}

[Serializable]
class SkinApplyed {
	public int skin = 0;
}

[Serializable]
class NotificationEnabled {
	public bool achievement, magazine;
}

[Serializable]
class GetThing { // Уведомления
	public int skin = -1, achievement = -1;
}

[Serializable]
class LevelsCharacters {
	public int[] levels = new int[8];
	public int available;
}

[Serializable]
class CharacterStats {
	public int maxHealth;
}

[Serializable]
class PlayerLocationSaves {
	public List<PLS> saves = new();
}

[Serializable]
class PLS { // Сохранение игрока на сцене
	public float playerPosX, playerPosY;
	public int health;
	public GameObj[] objectsSaves;
	public int sceneId;
	public bool showInterface;
}

[Serializable]
class GameObj {
	float posX, posY, posZ,
		rotX, rotY, rotZ, rotW,
		scaleX, scaleY, scaleZ,
		width, height;
	public bool active, colliderEnabled, rigidbodySleep;

	public GameObj(GameObject obj) {
		posX = obj.transform.localPosition.x;
		posY = obj.transform.localPosition.y;
		posZ = obj.transform.localPosition.z;

		rotX = obj.transform.localRotation.x;
		rotY = obj.transform.localRotation.y;
		rotZ = obj.transform.localRotation.z;
		rotW = obj.transform.localRotation.w;

		scaleX = obj.transform.localScale.x;
		scaleY = obj.transform.localScale.y;
		scaleZ = obj.transform.localScale.z;

		active = obj.activeSelf;

		if (obj.TryGetComponent(out RectTransform rect))
			(width, height) = (rect.rect.width, rect.rect.height);

		if (obj.TryGetComponent(out Collider2D collider))
			colliderEnabled = collider.enabled;

		if (obj.TryGetComponent(out Rigidbody2D rigidbody))
			rigidbodySleep = rigidbody.IsSleeping();
	}

	public void SetProps(ref GameObject obj) {
		obj.SetActive(false);

		if (obj.TryGetComponent(out Collider2D collider2D))
			collider2D.enabled = colliderEnabled;

		if (obj.TryGetComponent(out RectTransform rectTransform))
			rectTransform.sizeDelta = GetSize();

		if (obj.TryGetComponent(out Rigidbody2D rigidbody2D)) {
			rigidbody2D.velocity = Vector2.zero;

			if (rigidbodySleep)
				rigidbody2D.Sleep();
			else
				rigidbody2D.WakeUp();
		}

		obj.transform.localPosition = GetPosition();
		obj.transform.localRotation = GetRotation();
		obj.transform.localScale = GetScale();

		obj.SetActive(active);
	}

	Vector3 GetPosition() => new(posX, posY, posZ);
	Quaternion GetRotation() => new(rotX, rotY, rotZ, rotW);
	Vector3 GetScale() => new(scaleX, scaleY, scaleZ);
	Vector2 GetSize() => new(width, height);
}