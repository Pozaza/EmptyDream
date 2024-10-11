using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {
	public static PlayerScript Instance;

	[Header("Другое")]
	public SpriteRenderer head;
	public Sprite[] eyes;
	public bool IsSurprised { get; set; }
	public ParticleSystem playerParticles;
	Animation _animation;

	[Header("Движение")]
	[HideInInspector] public Rigidbody2D rb;
	public float horizontalSpeed = 6f, jumpSpeed = 6f;
	public bool facingRight = true, canJump = true, canCrouch = true, canLay = true, isDamaged, canBlink = true;
	float _timeInAir, _movement;

	[Header("Столкновения")]
	public LayerMask collisionLayer;
	public float groundLength = 1.5f;
	public PhysicsMaterial2D physicsMaterial;
	public BoxCollider2D crouchCheck, layCheck;
	BoxCollider2D _collider;
	bool _onGround = true, _onGroundBefore, canMove, isCrouching, isLaying;

	void Awake() {
		Instance = this;
		rb = GetComponent<Rigidbody2D>();
		_animation = GetComponent<Animation>();
		_collider = GetComponent<BoxCollider2D>();
	}
	void Start() => StartCoroutine(EyesBlink());
	void Update() {
		_onGroundBefore = _onGround;

		_onGround = Physics2D.Raycast(transform.position + new Vector3(.05f, 0), Vector2.down, groundLength, collisionLayer) || Physics2D.Raycast(transform.position - new Vector3(.05f, 0), Vector2.down, groundLength, collisionLayer) || Physics2D.Raycast(transform.position, Vector2.down, groundLength, collisionLayer);

		_collider.sharedMaterial = _onGround ? null : physicsMaterial;

		if (canMove) {
			_movement = Input.GetKey(Settings.Key(KeyBind.Left)) ? -1 : Input.GetKey(Settings.Key(KeyBind.Right)) ? 1 : 0;
			Move(_movement * (_onGround ? 1.25f : 1.1f));

			if (Input.GetKeyDown(Settings.Key(KeyBind.Jump)) && _onGround && canJump && !isDamaged)
				Jump();

			if (Input.GetKey(Settings.Key(KeyBind.Crouch)) && CanCrouch())
				Crouch(true);
			else if ((!Input.GetKey(Settings.Key(KeyBind.Crouch)) || !_onGround || !canCrouch || isDamaged) && isCrouching)
				Crouch(false);

			if (Input.GetKey(Settings.Key(KeyBind.Lay)) && CanLay())
				Lay(true);
			else if ((!Input.GetKey(Settings.Key(KeyBind.Lay)) || !_onGround || !canLay || isDamaged) && isLaying)
				Lay(false);

			// if (Input.GetButtonDown("Fire2"))
			// _animation.CrossFade("PlayerAttack", .05f);
		} else
			_movement = 0;

		PlayerState state = GetPlayerState();

		if (state == PlayerState.Jump)
			_timeInAir += Time.deltaTime * 4;

		_animation.CrossFade(GetAnimationStateName(state), .1f);

		if (_movement != 0)
			_animation[GetAnimationStateName(state)].speed = Mathf.InverseLerp(0, 3f, Mathf.Abs(rb.velocity.x)) + Mathf.InverseLerp(3f, 6f, Mathf.Abs(rb.velocity.x));

		if (_onGround && !_onGroundBefore) {
			PlayerSounds.Instance.Jump();
			CameraFollow.Instance.Shake(.05f * _timeInAir, .05f * _timeInAir);
			_timeInAir = 0;
			_onGroundBefore = true;
		}
	}
	public void Move(float horizontalMovement) {
		Vector2 targetVelocity = new(horizontalMovement * horizontalSpeed, rb.velocity.y);
		Vector2 _v = new();
		rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref _v, .025f);

		if ((horizontalMovement > 0 && !facingRight) || (horizontalMovement < 0 && facingRight))
			Flip();
	}
	void Jump() {
		_onGround = false;
		rb.velocity = new Vector2(rb.velocity.x, jumpSpeed / 10);
	}
	void Crouch(bool value, bool force = false) {
		if (value) {
			horizontalSpeed /= 2.5f;
			transform.localPosition -= new Vector3(0f, .26f, 0f);
			_collider.size = new(_collider.size.x, _collider.size.y - .11f);
			isCrouching = true;
		} else {
			ContactFilter2D filter = new();
			filter.SetLayerMask(collisionLayer);
			if (crouchCheck.Cast(Vector2.zero, filter, new List<RaycastHit2D>(), 1f) > 0 && !force)
				return;
			horizontalSpeed *= 2.5f;
			transform.localPosition += new Vector3(0f, .26f, 0f);
			_collider.size = new(_collider.size.x, _collider.size.y + .11f);
			isCrouching = false;
		}
	}
	void Lay(bool value) {
		if (value) {
			if (isCrouching)
				Crouch(false, true);

			horizontalSpeed /= 4;
			transform.localPosition -= new Vector3(0f, .35f, 0f);
			_collider.size = new(_collider.size.x + .2f, _collider.size.y - .2f);
			isLaying = true;
		} else {
			horizontalSpeed *= 4;
			transform.localPosition += new Vector3(0f, .35f, 0f);
			_collider.size = new(_collider.size.x - .2f, _collider.size.y + .2f);
			isLaying = false;

			if (CanCrouch())
				Crouch(true);
		}
	}
	bool CanStay() {
		ContactFilter2D filter = new();
		filter.SetLayerMask(collisionLayer);
		return crouchCheck.Cast(Vector2.zero, filter, new List<RaycastHit2D>(), 1f) <= 0;
	}
	bool CanCrouch() {
		ContactFilter2D filter = new();
		filter.SetLayerMask(collisionLayer);
		return _onGround && !isCrouching && canCrouch && !isDamaged && layCheck.Cast(Vector2.zero, filter, new List<RaycastHit2D>(), 1f) <= 0;
	}
	bool CanLay() {
		return _onGround && !isLaying && canLay && !isDamaged;
	}
	PlayerState GetPlayerState() {
		if (!_onGround)
			return PlayerState.Jump;
		if (_animation.IsPlaying("PlayerAttack"))
			return PlayerState.Attack;
		if (_movement == 0)
			return isCrouching ? PlayerState.CrouchIdle : isDamaged ? PlayerState.DamagedIdle : isLaying ? PlayerState.LayIdle : PlayerState.Idle;
		else
			return isCrouching ? PlayerState.CrouchWalk : isDamaged ? PlayerState.DamagedWalk : isLaying ? PlayerState.LayWalk : PlayerState.Walk;
	}
	string GetAnimationStateName(PlayerState state) => state == PlayerState.Jump ? "PlayerJump" : state == PlayerState.Idle ? "PlayerIdle" : state == PlayerState.Walk ? "PlayerWalk" : state == PlayerState.DamagedIdle ? "PlayerIdleDamaged" : state == PlayerState.DamagedWalk ? "PlayerWalkDamaged" : state == PlayerState.CrouchIdle ? "PlayerCrouchIdle" : state == PlayerState.CrouchWalk ? "PlayerCrouchWalk" : "PlayerAttack";
	void Flip() {
		facingRight = !facingRight;
		transform.rotation = Quaternion.Euler(0, facingRight ? 0 : 180, 0);
	}
	public void Stop() {
		Input.ResetInputAxes();
		_movement = 0;
	}
	public void ShowInterface() {
		Interface.Instance?.ShowInterface();
		Menu.canDisplay = true;
	}
	public void CantMove() {
		canMove = false;
		Stop();
	}
	public void CanMove() => canMove = true;
	public void ChangeJump(bool value) => canJump = value;
	public void ChangeCrouch(bool value) => canCrouch = value;
	public void ChangeDamaged(bool value) => isDamaged = value;
	public void ChangeHorizontalSpeed(float value) => horizontalSpeed = value;
	public void CantBlink() {
		canBlink = false;
		StopAllCoroutines();
	}
	public void CanBlink() {
		canBlink = true;
		StartCoroutine(EyesBlink());
	}
	IEnumerator EyesBlink() {
		if (!canBlink)
			yield return null;

		yield return new WaitForSeconds(Random.Range(.5f, 6f));
		head.sprite = eyes[0];
		yield return new WaitForSeconds(Random.Range(.025f, .2f));
		head.sprite = eyes[isDamaged ? 2 : IsSurprised ? 3 : 1];
		StartCoroutine(EyesBlink());
	}
}
public enum PlayerState {
	Idle, Walk, Jump, CrouchIdle, CrouchWalk, LayIdle, LayWalk, DamagedIdle, DamagedWalk, Attack
}