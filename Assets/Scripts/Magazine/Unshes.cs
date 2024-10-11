using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class Unshes : MonoBehaviour {
	public static Unshes Instance;

	[Header("Магазин")]
	public bool hasMagazine = true;
	[ShowIf("hasMagazine")] public Text magazineUnshes, traderDialog;
	[ShowIf("hasMagazine")] public Animator tradorAnimator;

	[Header("Пули")]
	[ShowIf("hasMagazine")] public Text bulletsValueText;
	[EnableIf("hasMagazine")] int _bulletsValue = 1;

	public void LoadTrader() {
		Instance = this;

		if (SkinMagazine.Instance.unshes != null) {
			if (hasMagazine)
				magazineUnshes.text = SkinMagazine.Instance.unshesCount.ToString();
			else
				SaveUnshes();
		}
	}
	public void SaveUnshes() {
		SkinMagazine.Instance.SaveUnshes();

		if (hasMagazine)
			magazineUnshes.text = SkinMagazine.Instance.unshesCount.ToString();
	}
	public void BuyHealth(int type) {
		switch (type) {
			case 0:
				if (SkinMagazine.Instance.unshesCount >= 7) {
					SkinMagazine.Instance.DecraseUnshes(7);
					StopAllCoroutines();
					StartCoroutine(SuccessBuyHealth());
					PlayerHealth.Instance?.Heal(1);
				} else {
					int coff = 7 - SkinMagazine.Instance.unshesCount;
					StopAllCoroutines();
					StartCoroutine(NotEnoughUnshes(coff));
				}
				break;
			case 1:
				if (SkinMagazine.Instance.unshesCount >= 14) {
					SkinMagazine.Instance.DecraseUnshes(14);
					StopAllCoroutines();
					StartCoroutine(SuccessBuyHealth());
					PlayerHealth.Instance?.Heal(2);
				} else {
					int coff = 14 - SkinMagazine.Instance.unshesCount;
					StopAllCoroutines();
					StartCoroutine(NotEnoughUnshes(coff));
				}
				break;
			case 2:
				if (SkinMagazine.Instance.unshesCount >= 21) {
					SkinMagazine.Instance.DecraseUnshes(21);
					StopAllCoroutines();
					StartCoroutine(SuccessBuyHealth());
					PlayerHealth.Instance?.Heal(3);
				} else {
					int coff = 21 - SkinMagazine.Instance.unshesCount;
					StopAllCoroutines();
					StartCoroutine(NotEnoughUnshes(coff));
				}
				break;
		}
		SaveUnshes();
	}
	public void BuyFullHealth() {
		StopAllCoroutines();
		if (SkinMagazine.Instance.unshesCount >= 30) {
			SkinMagazine.Instance.DecraseUnshes(30);
			StartCoroutine(SuccessBuy());
			PlayerHealth.Instance?.HealFull();
		} else if (PlayerHealth.Instance?.health >= 5)
			StartCoroutine(SomethingWrong(0));
		else {
			int coff = 30 - SkinMagazine.Instance.unshesCount;
			StartCoroutine(NotEnoughUnshes(coff));
		}
		SaveUnshes();
	}
	public void ChangeBulletsValue(float value) {
		_bulletsValue = (int)value;
		bulletsValueText.text = value + " шт. * 1 = " + value + " унш.";
	}
	public void BuyBullets() {
		StopAllCoroutines();
		if (SkinMagazine.Instance.unshesCount >= _bulletsValue && PrefabWeapon.Instance.ammo + _bulletsValue <= 100) {
			SkinMagazine.Instance.DecraseUnshes(_bulletsValue);
			StartCoroutine(SuccessBuy());
			PrefabWeapon.Instance?.SetAmmo(_bulletsValue);
		} else if (PrefabWeapon.Instance?.ammo == 100)
			StartCoroutine(SomethingWrong(1));
		else {
			int coff = _bulletsValue - SkinMagazine.Instance.unshesCount;
			StartCoroutine(NotEnoughUnshes(coff));
		}
		SaveUnshes();
	}
	IEnumerator NotEnoughUnshes(int notEnough) {
		tradorAnimator.SetTrigger("failedBuy");
		traderDialog.text = "Извини, но тебе не хватает уншей: " + notEnough + "..\nПриходи, когда достанешь их!";
		yield return new WaitForSeconds(7);
		traderDialog.text = "";
	}
	IEnumerator SomethingWrong(int type) {
		tradorAnimator.SetTrigger("errorBuy");
		switch (type) {
			case 0:
				traderDialog.text = "У тебя уже полное здоровье.";
				break;
			case 1:
				traderDialog.text = "Ты не можешь нести больше патронов! (максимум: 100)";
				break;
			case 2:
				traderDialog.text = "Это улучшение уже на максимуме.";
				break;
			case 3:
				traderDialog.text = "Этот предмет уже был продан!";
				break;
		}
		yield return new WaitForSeconds(6);
		traderDialog.text = "";
	}
	IEnumerator SuccessBuy() {
		tradorAnimator.SetTrigger("successBuy");
		traderDialog.text = "Спасибо за покупку!\nПриходи ещё!";
		yield return new WaitForSeconds(6);
		traderDialog.text = "";
	}
	IEnumerator SuccessBuyHealth() {
		tradorAnimator.SetTrigger("successBuy");
		traderDialog.text = "Постарайся меньше раняться..\nПриходи ещё!";
		yield return new WaitForSeconds(5.5f);
		traderDialog.text = "";
	}
}
