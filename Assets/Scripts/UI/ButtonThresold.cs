using UnityEngine;
using UnityEngine.UI;

public class ButtonThresold : MonoBehaviour {
	void Start() => GetComponent<Image>().alphaHitTestMinimumThreshold = 1;
}
