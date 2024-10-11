using UnityEngine;
using UnityEngine.UI;

public class SkinLoader : MonoBehaviour {
	void Start() {
		SaveManager.Load("skinApply", out SkinApplyed data);

		string path = "Robots/" + data.skin;

		if (TryGetComponent(out SpriteRenderer s)) {
			s.color = new Color(.4f, .4f, .4f, 1);
			s.sprite = Resources.Load<Sprite>(path);
		} else
			GetComponent<Image>().sprite = Resources.Load<Sprite>(path);
	}
}
