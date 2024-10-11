using UnityEngine;
using UnityEngine.Events;

public class HideIfHasPref : MonoBehaviour {
	public string nameOfPref;
	public bool perFrame;
	public UnityEvent ifHas;
	public UnityEvent ifHasNot;

	void Awake() {
		if (PlayerPrefs.HasKey(nameOfPref))
			ifHas.Invoke();
		else
			ifHasNot.Invoke();
		if (!perFrame)
			Destroy(this);
	}
	void Update() {
		if (PlayerPrefs.HasKey(nameOfPref))
			ifHas.Invoke();
		else
			ifHasNot.Invoke();
	}
}