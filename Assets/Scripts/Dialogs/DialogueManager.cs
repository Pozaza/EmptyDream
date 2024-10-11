using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {
	public static DialogueManager Instance;
	public RectTransform dialogBox;
	public Image dialogImage;
	public TMP_Text dialogText;
	public bool DialogueIsPlaying { get; private set; }
	public AudioSource sounds;
	AudioClip sound;
	bool _autoNext, _canInteract, _lockMove, _skip;
	Story currentStory;
	Coroutine displayLineCoroutine;
	Button _continueButton;

	void Awake() {
		Instance = this;
		dialogText.autoSizeTextContainer = true;
		_continueButton = dialogText.transform.GetChild(0).GetComponent<Button>();
		_continueButton.gameObject.SetActive(false);
	}
	void Start() => DialogueIsPlaying = false;
	void Update() {
		if (!DialogueIsPlaying || !_canInteract)
			return;
		if (Input.GetKeyDown(Settings.Key(KeyBind.Dialog)))
			Skip();
	}
	void Sound() {
		sounds.pitch = Random.Range(.8f, 1.3f);
		sounds.PlayOneShot(sound);
	}
	public void EnterDialogueMode(TextAsset inkJSON, DialogColorAndFont dialogColorAndFont, bool canInteract, bool autoNext, bool lockMove) {
		dialogText.text = "";
		_canInteract = canInteract;
		_autoNext = autoNext;
		_lockMove = lockMove;
		dialogImage.color = dialogColorAndFont.GetColor();
		dialogText.color = dialogColorAndFont.GetColor();
		_continueButton.GetComponent<Image>().color = dialogColorAndFont.GetColor();
		sound = dialogColorAndFont.GetSound();

		currentStory = new Story(inkJSON.text);
		StopAllCoroutines();
		StartCoroutine(ChangeDialog(false));
		DialogueIsPlaying = true;
		StartCoroutine(ChangeDialog(true));
		_continueButton.gameObject.SetActive(canInteract);
		ContinueStory();
		if (_lockMove)
			PlayerScript.Instance.CantMove();
	}
	public void ExitDialogueMode() {
		DialogueIsPlaying = false;
		StopAllCoroutines();
		StartCoroutine(ChangeDialog(false));
	}
	public void ContinueStory() {
		_skip = false;
		if (currentStory.canContinue) {
			if (displayLineCoroutine != null)
				StopCoroutine(displayLineCoroutine);
			string nextLine = currentStory.Continue();
			displayLineCoroutine = StartCoroutine(DisplayLine(nextLine));
		} else
			ExitDialogueMode();
	}
	public void Skip() {
		if (_skip && _canInteract) {
			StopCoroutine("DisplayLine");
			ContinueStory();
		} else
			_skip = true;
	}
	IEnumerator DisplayLine(string line) {
		char[] chars = line.ToCharArray();
		List<int> indexes = new();
		List<int> times = new();
		for (int i = 0; i < chars.Length; i++) {
			if (chars[i] == '•' || chars[i] == '○') {
				int durable = int.Parse(chars[i + 1].ToString());
				indexes.Add(i);
				times.Add(chars[i] == '•' ? durable : durable * 10);
				chars = chars.ArrayToString().Remove(i, 2).ToCharArray();
			}
		}
		dialogText.maxVisibleCharacters = 0;
		dialogText.text = chars.ArrayToString();
		bool isAddingRichTextTag = false;

		for (int i = 0; i < chars.Length; i++) {
			if (_skip) {
				dialogText.maxVisibleCharacters = chars.Length;
				break;
			}
			bool wait = indexes.Any(i2 => i2 == i);
			if (wait)
				yield return new WaitForSeconds(times[indexes.IndexOf(i)] / 10f);
			if (chars[i] == '<' || isAddingRichTextTag)
				isAddingRichTextTag = chars[i] != '>';
			else {
				dialogText.maxVisibleCharacters++;
				if (chars[i] != ',') {
					Sound();
					if (!wait)
						yield return new WaitForSeconds(Random.Range(.015f, .06f));
				} else
					yield return new WaitForSeconds(.15f);
			}
		}

		if (_canInteract)
			_continueButton.gameObject.SetActive(true);

		if (!_canInteract || _autoNext) {
			yield return new WaitForSeconds(2f);
			ContinueStory();
		}
	}
	IEnumerator ChangeDialog(bool state) { // Открыть(true) или закрыть(false)
		if (_continueButton.gameObject.activeSelf && !state)
			_continueButton.gameObject.SetActive(false);
		if (_lockMove)
			PlayerScript.Instance.CanMove();
		float time = 0;
		while (time <= 1.25f) {
			time += Time.deltaTime / .4f;
			dialogBox.localScale = Vector3.Lerp(dialogBox.localScale, state ? Vector3.one * .0025f : Vector3.zero, Mathf.SmoothStep(0, 1, time));
			yield return new WaitForEndOfFrame();
		}
	}
}