using UnityEngine;
using UnityEngine.UI;

public class ColorManager : MonoBehaviour {
	public Color[] colors;
	Image _img;
	SpriteRenderer _renderer;

	void Awake() {
		TryGetComponent(out Image img);
		if (img)
			_img = img;
		else
			_renderer = GetComponent<SpriteRenderer>();
	}

	public void SetColor(int id) {
		if (_img)
			_img.color = colors[id];
		else
			_renderer.color = colors[id];
	}
}
