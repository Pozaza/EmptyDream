using System.Collections;
using NaughtyAttributes;
using UnityEngine;

public class Enemy : MonoBehaviour {
	Vector3 _scale;

	[Header("Индикаторы")]
	public GameObject indicatorShow;
	public float radius;
	public GameObject polosa;
	float _cof;
	bool _per = true;

	[Header("Дамаг")]
	public AudioClip damageSound;
	public GameObject deathParticles;
	public bool once;
	public float HP;
	[HideIf("once")] public float interval;
	float _timeLeft;
	public Color HurtColor;
	Color _normalColor;
	public int damage;
	bool _ishurt, _istimeroff = true;

	[Header("Стрельба")]
	public bool canShoot;
	[ShowIf("canShoot")] public float cooldown;
	[ShowIf("canShoot")] public GameObject prefab, firePoint;
	[EnableIf("canShoot")] GameObject _player;
	[EnableIf("canShoot")] bool _timeroff = true;

	[Header("Поворот")]
	public bool canRotate;
	public bool hasEyes;
	bool _facingRight = true;
	[ShowIf("hasEyes")] public Transform eye;
	[ShowIf("hasEyes")] public float eyeRadius;

	[Header("Унши")]
	public bool dropUnshes = true;
	[ShowIf("dropUnshes")] public int unshesToDrop = 5;
	[ShowIf("dropUnshes")] public GameObject unshiPrefab;

	[Header("Движение")]
	public bool isJumping;
	[ShowIf("isJumping")] public bool intervalJump;
	[ShowIf("isJumping")] public float strenght = 5;
	[ShowIf("intervalJump")] public float intervalJumpS;
	[ShowIf("intervalJump")] public bool randomInterval;
	[ShowIf("randomInterval")][MinMaxSlider(.0f, 10.0f)] public Vector2 random;
	bool _canJump = true;
	Rigidbody2D _rb;

	void Awake() {
		_player = this.FindTag("Player");
		_scale = transform.localScale;
		_normalColor = GetComponent<SpriteRenderer>().color;
		_rb = gameObject.transform.GetComponentInParent<Rigidbody2D>();
	}
	void Update() {
		if (Vector2.Distance(_player.transform.position, transform.position) < radius) {
			if (canShoot && _timeroff) {
				_timeroff = false;
				StartCoroutine(Shoot());
			}
			//Стрельба (поворот)
			Vector3 diff = _player.transform.position - transform.position;
			if (_player.transform.position.x < transform.position.x) {
				if (canRotate)
					transform.localScale = new Vector3(_scale.x, -_scale.y, _scale.z);
				else {
					transform.localScale = new Vector3(-_scale.x, _scale.y, _scale.z);
					_facingRight = false;
				}
			} else {
				transform.localScale = new Vector3(_scale.x, _scale.y, _scale.z);
				_facingRight = true;
			}
			diff.Normalize();
			float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
			if (canShoot)
				firePoint.transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
			if (canRotate)
				transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
			if (indicatorShow != null)
				indicatorShow.SetActive(true);
			if (hasEyes) {
				eye.parent.transform.localScale = _facingRight ? new Vector3(-1, 1, 1) : Vector3.one;
				eye.localPosition = eyeRadius * (_player.transform.position - eye.position).normalized;
			}
		} else {
			if (indicatorShow != null)
				indicatorShow.SetActive(false);
			if (canRotate) {
				transform.localScale = new Vector3(_scale.x, _scale.y, _scale.z);
				transform.rotation = Quaternion.Euler(0f, 0f, 0f);
			}
		}
		if (_ishurt && _timeLeft > 0) {
			_timeLeft -= Time.deltaTime;
			GetComponent<SpriteRenderer>().color = Color.Lerp(HurtColor, _normalColor, Time.deltaTime / _timeLeft);
		} else {
			_ishurt = false;
			_timeLeft = .5f;
			GetComponent<SpriteRenderer>().color = _normalColor;
		}
		if (isJumping && _canJump)
			StartCoroutine(Jump());
	}
	void OnTriggerEnter2D(Collider2D collision) {
		if (collision.CompareTag("Player") && once) {
			StartCoroutine(DealDamage());
			Destroy(this);
		}
	}
	void OnTriggerStay2D(Collider2D collision) {
		if (collision.CompareTag("Player") && !once && _istimeroff) {
			StartCoroutine(DealDamage());
			_istimeroff = false;
		}
	}
	public void Hit(float damager) {
		if (HP - damager > 0) {
			_ishurt = true;
			GetComponent<AudioSource>().PlayOneShot(damageSound, .75f);
			GameObject p = Instantiate(deathParticles, transform.position, Quaternion.identity);
			p.GetComponent<ParticleSystem>().Emit(15);
			HP -= damager;
			Vector3 _scaleCofficent;
			_scaleCofficent = polosa.transform.localScale;
			if (_per) {
				_cof = damager / HP;
				_scaleCofficent.x -= _cof;
				polosa.transform.localScale = _scaleCofficent;
				_per = false;
			} else {
				_scaleCofficent.x -= _cof;
				polosa.transform.localScale = _scaleCofficent;
			}
		} else
			Death();
	}
	IEnumerator DealDamage() {
		PlayerHealth.Instance.DiedEvent += CameraToMe;
		PlayerHealth.Instance.TakeDamage(damage);
		PlayerHealth.Instance.DiedEvent -= CameraToMe;
		yield return new WaitForSeconds(interval);
		_istimeroff = true;
	}
	void CameraToMe() {
		FindObjectOfType<CameraZoomManager>().Zoom(1.5f, 1f);
		CameraFollow.Instance?.StartFollow();
		CameraFollow.Instance.offset = Vector3.zero;
		CameraFollow.Instance.target = transform;
	}
	IEnumerator Shoot() {
		Instantiate(prefab, firePoint.transform.position, firePoint.transform.rotation);
		yield return new WaitForSeconds(cooldown);
		_timeroff = true;
	}
	IEnumerator Jump() {
		_canJump = false;
		_rb.AddForce(Vector2.up * strenght, ForceMode2D.Impulse);
		if (!intervalJump)
			yield return new WaitForSeconds(1.5f);
		else
			yield return new WaitForSeconds(!randomInterval ? intervalJumpS : Random.Range(random.x, random.y));
		_canJump = true;
	}
	public void Death() {
		GameObject p = Instantiate(deathParticles, transform.position, Quaternion.identity);
		p.GetComponent<ParticleSystem>().Emit(15);
		for (int i = 0; i < unshesToDrop; i++) {
			Instantiate(unshiPrefab, transform.position + new Vector3(Random.Range(-.25f, .25f), Random.Range(0, .25f), 0), Quaternion.identity);
		}
		Destroy(gameObject);
	}
}
