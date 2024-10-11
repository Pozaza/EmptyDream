using System.Collections;
using UnityEngine;

public class Abilities : MonoBehaviour {
	public static Abilities Instance;

	public bool lockAll;
	public AudioSource sounds;

	[Header("Искра")]
	public bool canSpark;
	public ParticleSystem sparkParticle;
	public AudioClip sparkSound;
	public Animation sparkCooldownAnimation;

	void Awake() {
		Instance = this;
	}

	void Update() {
		if (lockAll || Menu.menuDisplayed)
			return;

		if (Input.GetKeyDown(Settings.Key(KeyBind.Spark)) && canSpark) {
			StartCoroutine(Spark());
			sounds.PlayOneShot(sparkSound);
			if (PrefabWeapon.Instance.companyon)
				StartCoroutine(PrefabWeapon.Instance.Emote(8, 0, .75f));
			sparkCooldownAnimation.Play("spark_cooldown");
		}
	}
	IEnumerator Spark() {
		canSpark = false;
		sparkParticle.Emit(10);
		PlayerScript.Instance.rb.velocity = new(PlayerScript.Instance.rb.velocity.x + (PlayerScript.Instance.facingRight ? 50 : -50), PlayerScript.Instance.rb.velocity.y);
		yield return new WaitForSeconds(2.75f);
		canSpark = true;
	}
}