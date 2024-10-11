using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonToggle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler {
	public Text text;
	public bool textColor;
	[ShowIf("textColor")] public Color enabledTextColor;
	[EnableIf("textColor")] Color _disabledTextColor;

	public Image image;

	public bool imageSprite;
	[ShowIf("imageSprite")] public Sprite enabledImageSprite;
	[ShowIf("imageSprite")] Sprite _disabledImageSprite;

	public bool imageColor;
	[ShowIf("imageColor")] public Color enabledImageColor;
	[EnableIf("imageColor")] Color _disabledImageColor;

	public AudioClip mouseEnter, mouseClick;

	ScaleLerper _lerper;
	Toggle _toggle;


	void Awake() {
		_toggle = GetComponent<Toggle>();
		image = image == null ? transform.GetComponentInParent<Image>() : image;
		text = text == null ? GetComponent<Text>() : text;
		if (imageColor)
			_disabledImageColor = image.color;
		if (textColor)
			_disabledTextColor = text.color;
		if (imageSprite)
			_disabledImageSprite = image.sprite;
		_lerper = image.GetComponent<ScaleLerper>();
	}
	void Start() {
		_toggle.onValueChanged.AddListener(delegate {
			ToggleValueChanged();
		});
		ToggleValueChanged();
	}
	void ToggleValueChanged() {
		if (imageColor)
			image.color = _toggle.isOn ? enabledImageColor : _disabledImageColor;
		if (textColor)
			text.color = _toggle.isOn ? enabledTextColor : _disabledTextColor;
		if (imageSprite)
			image.sprite = _toggle.isOn ? enabledImageSprite : _disabledImageSprite;
	}
	public void OnPointerEnter(PointerEventData eventData) {
		_lerper.enabled = true;
		_lerper.ToSize();
		GameManager.Instance?.uiSounds?.PlayOneShot(mouseEnter);
	}
	public void OnPointerExit(PointerEventData eventData) => _lerper.ToOriginalSize();
	public void OnPointerDown(PointerEventData data) {
		_lerper.ToOriginalSize();
		GameManager.Instance?.uiSounds?.PlayOneShot(mouseClick);
	}
	public void OnPointerClick(PointerEventData data) => _lerper.ToSize();
}
