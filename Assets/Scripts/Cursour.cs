using DG.Tweening;
using UnityEngine;

public class Cursour : MonoBehaviour {
	public static Cursour Instance;
	public Texture2D normal, attack, clicked;
	public Sprite attackOrb, normalOrb;
	public Color normalColor, attackColor;
	public GameObject cursour;

	ParticleSystem _particleSystem;
	ParticleSystem.MainModule _mainModule;

	Vector3 firstMousePos = Vector3.zero;

	void Awake() {
		if (Instance == null)
			Instance = this;

		if (cursour != null && cursour.transform.childCount > 0) {
			_particleSystem = cursour.GetChildObj(0).GetComponent<ParticleSystem>();
			_mainModule = _particleSystem.main;
		}
	}
	void Update() {
		if (firstMousePos != Input.mousePosition && _particleSystem != null) {
			_mainModule.loop = true;
			_particleSystem.Play();
		} else
			_mainModule.loop = false;

		firstMousePos = Input.mousePosition;

		Vector3 mousePos = CameraFollow.Instance.cameraComponent.ScreenToWorldPoint(Input.mousePosition);
		mousePos.z = 1;
		cursour.transform.DOMove(mousePos, .5f).SetEase(Ease.OutCubic);

		if (Input.GetMouseButtonDown(0)) {
			cursour.TryGetComponent(out ScaleLerper sl);

			if (sl.enabled)
				sl.ToSize();
			else
				sl.enabled = true;
		} else if (Input.GetMouseButtonUp(0))
			cursour.GetComponent<ScaleLerper>().ToOriginalSize();

		if (PrefabWeapon.Instance) {
			if (Input.GetMouseButtonUp(0) && !PrefabWeapon.inShootMode)
				ChangeCursor(normal, false);
			else if (Input.GetMouseButtonUp(0) && !PrefabWeapon.Instance)
				ChangeCursor(normal, false);

			if (Input.GetMouseButtonDown(0) && !PrefabWeapon.inShootMode)
				ChangeCursor(clicked, false);
			else if (Input.GetMouseButtonDown(0) && !PrefabWeapon.Instance)
				ChangeCursor(clicked, false);
		}
	}
	public void ChangeCursor(Texture2D texture, bool attackMode) {
		if (cursour.TryGetComponent(out SpriteRenderer sr)) {
			sr.color = attackMode ? attackColor : normalColor;
			sr.sprite = attackMode ? attackOrb : normalOrb;
		}
		Cursor.SetCursor(texture, new Vector2(48, 48), CursorMode.Auto);
	}
}
