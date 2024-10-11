using UnityEngine;
using UnityEngine.UI;

public class LevelInfo : MonoBehaviour {
	public Text levelName, levelInfo;

	public void SetData(LevelObject level) {
		levelName.text = level.name;
		levelInfo.text = level.info;
	}
}