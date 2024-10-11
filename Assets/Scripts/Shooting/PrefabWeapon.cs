using System.Collections;
using DG.Tweening;
using UnityEngine;

public class PrefabWeapon : MonoBehaviour {
	public static PrefabWeapon Instance;
	public SpriteRenderer face;
	public GameObject bulletPrefab;
	public bool companyon, canShoot, inMenu;

	[Header("Эмоции:")]
	public Sprite[] emote;

	[Header("Движение")]
	public float speed = .5f;

	[Header("Другое")]
	public int ammo = 50;
	public Vector3 weaponOffset;
	public Color normal, notPatrons;
	public static bool inShootMode;
	public bool canShootWhileCutscene;

	[Header("Звуки")]
	public AudioClip[] attack, noPatrons;

	bool _isChangedEmote, _isCursorChanged, _timerOut = true;

	void Awake() => Instance = this;
	void Update() {
		if (companyon && canShoot && Input.GetButton("Fire1") && !inMenu && (canShootWhileCutscene && GameManager.cutscene || !GameManager.cutscene) && (canShootWhileCutscene && GameManager.interactiveCutscene || !GameManager.interactiveCutscene)) {
			_isCursorChanged = false;
			Cursour.Instance?.ChangeCursor(Cursour.Instance?.attack, true);
		} else if (!canShoot && Input.GetButton("Fire1") && !_isCursorChanged) {
			_isCursorChanged = true;
			Cursour.Instance?.ChangeCursor(Cursour.Instance?.normal, false);
		}
		TryGetComponent(out ParticleSystem ps);
		TryGetComponent(out SpriteRenderer sr);
		if (companyon) {
			transform.DOMove(PlayerScript.Instance.transform.position + weaponOffset, speed);
			sr.enabled = true;
			face.enabled = true;
			if (canShoot && _timerOut) {
				if (ammo > 0) {
					sr.material.color = normal;
					if (!ps.isPlaying)
						ps.Play();
					_isChangedEmote = false;
				} else {
					if (!_isChangedEmote) {
						_isChangedEmote = true;
						StartCoroutine(Emote(2, 3, .4f));
					}
					sr.material.color = notPatrons;
					if (ps.isPlaying)
						ps.Stop();
				}
				if (ammo > 0 && Input.GetButton("Fire1") && !inMenu && (canShootWhileCutscene && GameManager.cutscene || !GameManager.cutscene) && (canShootWhileCutscene && GameManager.interactiveCutscene || !GameManager.interactiveCutscene)) {
					_timerOut = false;
					StartCoroutine(TimerOff());
					inShootMode = true;
					Shoot(true);
					ammo -= 1;
					GetComponent<AudioSource>().PlayOneShot(attack.Random());
				} else if (ammo <= 0 && Input.GetButtonDown("Fire1") && !inMenu && (canShootWhileCutscene && GameManager.cutscene || !GameManager.cutscene) && (canShootWhileCutscene && GameManager.interactiveCutscene || !GameManager.interactiveCutscene)) {
					GetComponent<AudioSource>().PlayOneShot(noPatrons.Random());
					StartCoroutine(Emote(2, 3, .4f));
				}
			}
			Vector3 diff = CameraFollow.Instance.cameraComponent.ScreenToWorldPoint(Input.mousePosition) - transform.position;
			diff.Normalize();

			float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
			Quaternion rotation = Quaternion.Euler(0, 0, rot_z);
			transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 10 * Time.deltaTime);
		} else {
			transform.DOKill(true);
			ps.Stop();
			sr.enabled = false;
			face.enabled = false;
		}
	}
	IEnumerator TimerOff() {
		yield return new WaitForSeconds(.3f);
		_timerOut = true;
	}
	public IEnumerator Emote(int first, int second, float time) {
		face.sprite = emote[first];
		yield return new WaitForSeconds(time);
		face.sprite = emote[second];
	}
	public void Shoot(bool isNormal) {
		Instantiate(bulletPrefab, transform.position, transform.rotation);
		if (isNormal && companyon)
			StartCoroutine(Emote(9, 0, .4f));
		StartCoroutine(MouseSprite());
	}
	IEnumerator MouseSprite() {
		yield return new WaitForSeconds(1.5f);
		inShootMode = false;
	}
	public void CanShoot(bool can) => canShoot = can;
	public void Companyon(bool val) => companyon = val;
	public void SetAmmo(int val) => ammo += val;
}
