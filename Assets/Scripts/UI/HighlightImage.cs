using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HighlightImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	public Color enter;
	Color _exit;
	Image _image;

	void Awake() {
		_image = GetComponent<Image>();
		_exit = _image.color;
	}

	public void OnPointerEnter(PointerEventData eventData) => _image.color = enter;

	public void OnPointerExit(PointerEventData eventData) => _image.color = _exit;
}
