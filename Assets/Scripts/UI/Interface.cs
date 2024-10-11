using DG.Tweening;
using UnityEngine;

public class Interface : MonoBehaviour {
	public static Interface Instance;
	public GameObject saveNotification, magazine;
	Animator _animator;

	void Awake() {
		Instance = this;
		_animator = GetComponent<Animator>();
	}
	void Update() => transform.DOLocalMove(PlayerScript.Instance.rb.velocity * -2, 1).SetEase(Ease.OutCubic);
	void OnDisable() => transform.DOKill(true);

	public void ShowInterface() {
		if (_animator == null)
			return;
		_animator.SetBool("showed", true);
		_animator.SetTrigger("show");
	}
	public void HideInterface() {
		if (_animator == null)
			return;
		_animator.SetBool("showed", false);
		_animator.SetTrigger("hide");
	}
	public bool GetState() => _animator.GetBool("showed");
	public void ShowSaveNotification() => saveNotification.SetActive(true);
}
