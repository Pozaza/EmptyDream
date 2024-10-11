using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSlider : MonoBehaviour, IPointerDownHandler, IPointerExitHandler, IPointerClickHandler, IDropHandler {
	public Color dynamicSliderFillColor;

	public Image sliderFill;
	Color _staticSliderFillColor;
	ScaleLerper _lerper;

	void Awake() {
		_staticSliderFillColor = sliderFill.color;
		_lerper = sliderFill.GetComponent<ScaleLerper>();
	}
	public void OnPointerDown(PointerEventData eventData) {
		_lerper.enabled = true;
		sliderFill.color = dynamicSliderFillColor;
		_lerper.ToSize();
	}
	public void OnPointerExit(PointerEventData eventData) {
		sliderFill.color = _staticSliderFillColor;
		_lerper.ToOriginalSize();
	}
	public void OnPointerClick(PointerEventData eventData) {
		sliderFill.color = _staticSliderFillColor;
		_lerper.ToOriginalSize();
	}
	public void OnDrop(PointerEventData eventData) {
		sliderFill.color = _staticSliderFillColor;
		_lerper.ToOriginalSize();
	}
}
