using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RandomHint : MonoBehaviour {
	[System.Serializable]
	public class Hint {
		public float timeToNext = 3.5f;
		public string hintText;
	}
	public Hint[] hints;
	bool _canChange = true;
	int _choose;

	void Update() {
		if (_canChange) {
			_canChange = false;
			int choose = Random.Range(0, hints.Length);
			if (choose == _choose)
				_canChange = true;
			else {
				_choose = choose;
				StartCoroutine(ChangeHint(hints[_choose].timeToNext, hints[_choose].hintText));
			}
		}
	}
	IEnumerator ChangeHint(float time, string text) {
		GetComponent<Text>().text = $"Совет: {text}";
		yield return new WaitForSeconds(time);
		_canChange = true;
	}
}
