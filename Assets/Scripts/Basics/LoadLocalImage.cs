using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LoadLocalImage : MonoBehaviour {
	public string nameOfTheFile;
	Texture2D _texture;
	Sprite _sprite;
	void Start() {
		_texture = LoadPNG(Application.dataPath + "/Sprites/" + nameOfTheFile);
		if (_texture.height == 0 || _texture.width == 0)
			return;
		_sprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), new Vector2(.5f, .5f));
		if (TryGetComponent(out Image img)) {
			GetComponent<RectTransform>().sizeDelta = new Vector2(_texture.width, _texture.height);
			img.sprite = _sprite;
		} else if (TryGetComponent(out SpriteRenderer sr))
			sr.sprite = _sprite;
	}
	public static Texture2D LoadPNG(string filePath) {
		Texture2D textureInput = new(1, 1, TextureFormat.RGBA32, true);
		if (File.Exists(filePath)) {
			textureInput.LoadImage(File.ReadAllBytes(filePath));
			textureInput.filterMode = FilterMode.Point;
			textureInput.anisoLevel = 0;
			textureInput.Apply();
		} else
			return new Texture2D(0, 0);
		return textureInput;
	}
}
