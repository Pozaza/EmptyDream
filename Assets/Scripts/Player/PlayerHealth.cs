using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {
	public static PlayerHealth Instance;
	[Header("Урон")]
	public AudioClip hurtSound, diedSound;
	public ParticleSystem particles;

	[Header("HP")]
	public Image section;
	public int health = 2;
	int _maxHealth = 2;
	public ParticleSystem healParticles;
	public Animation deathAnimation;

	[Header("Цвета")]
	public Color hurtColor, healColor;
	public ColorLerp colorLerpElement;

	[Header("Весёлости")]
	public AchievementScriptableObjects died10times;
	int diedCount;

	AudioSource playerAudioManager;
	Vector2 rectSize = new(1.4f, 1);
	[HideInInspector] public bool isDead;
	[HideInInspector] public Vector3 respawnPlayer;
	[HideInInspector] public event System.Action DiedEvent;
	[HideInInspector] public event System.Action RespawnEvent;

	void Awake() {
		Instance = this;
		health = _maxHealth;
		respawnPlayer = transform.position;
		playerAudioManager = GetComponent<AudioSource>();
		Load();
	}
	public void TakeDamage(int amount) {
		if (!isDead) {
			if (health > amount) {
				health -= amount;
				VignetteColor(hurtColor, .25f);
				rectSize.x -= .7f * amount;
				section.rectTransform.DOSizeDelta(rectSize, 1).SetEase(Ease.OutCubic);
				playerAudioManager.PlayOneShot(hurtSound, .5f);
				if (PrefabWeapon.Instance.companyon)
					StartCoroutine(PrefabWeapon.Instance?.Emote(3, 0, 1.5f));
				CameraFollow.Instance?.Shake(.6f, .075f);
				particles.Emit(25);
			} else
				Die();
		}
	}
	public void Heal(int amount) {
		int i = health + amount;
		if (i > _maxHealth) {
			health = _maxHealth;
			rectSize.x = .7f * _maxHealth;
			section.rectTransform.DOSizeDelta(rectSize, 1).SetEase(Ease.OutCubic);
		} else {
			health += amount;
			rectSize.x += +.7f * amount;
			section.rectTransform.DOSizeDelta(rectSize, 1).SetEase(Ease.OutCubic);
		}
		healParticles.Emit(10);
		VignetteColor(healColor, .5f);
	}
	public void HealFull() {
		health = _maxHealth;
		healParticles.Emit(10);
		rectSize.x = .7f * _maxHealth;
		section.rectTransform.DOSizeDelta(rectSize, 1).SetEase(Ease.OutCubic);
	}
	public void Die() {
		if (++diedCount == 10 && FunManager.Instance)
			FunManager.Instance?.GetAchievement(died10times);
		deathAnimation.Play();
		CameraFollow.Instance?.StopFollow();
		PlayerScript.Instance?.CantMove();
		GameManager.Instance?.sounds?.PlayOneShot(diedSound);
		DiedEvent?.Invoke();
	}
	public void SetHealth(int hp) {
		health = hp > _maxHealth ? _maxHealth : hp;
		rectSize.x = .7f * (hp > _maxHealth ? _maxHealth : hp);
		section.rectTransform.DOSizeDelta(rectSize, 1).SetEase(Ease.OutCubic);
	}
	public void RespawnTrigger() { // выполняется в анимации смерти
		health = _maxHealth;
		CameraFollow.Instance.target = CameraFollow.Instance.startTransform;
		CameraFollow.Instance?.StartFollow();
		CameraFollow.Instance?.InstantFollow();
		CameraZoomManager.Instance?.ZoomToOriginal();
		PlayerScript.Instance?.CanMove();
		CameraFollow.Instance.offset = CameraFollow.Instance.startOffset;
		PlayerScript.Instance.transform.position = respawnPlayer;
		rectSize.x = .7f * _maxHealth;
		section.rectTransform.DOSizeDelta(rectSize, 1).SetEase(Ease.OutCubic);
		Message.Instance?.MessageShow("Ты сможешь..");
		isDead = false;
		RespawnEvent?.Invoke();
	}
	void VignetteColor(Color col, float time) {
		colorLerpElement.time = time;
		colorLerpElement.end = new Color(col.r, col.g, col.b, 0);
		colorLerpElement.GetComponent<Image>().color = col;
		if (!colorLerpElement.isActiveAndEnabled)
			colorLerpElement.enabled = true;
	}
	void Load() {
		if (SaveManager.Load("characterStats", out CharacterStats data))
			_maxHealth = data.maxHealth;
		else
			SaveManager.Save(
				"characterStats",
				new CharacterStats() {
					maxHealth = _maxHealth
				}
			);
	}
}