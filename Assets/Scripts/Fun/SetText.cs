using UnityEngine;
using UnityEngine.UI;

public class SetText : MonoBehaviour {
    void Awake() {
        string text = GetComponent<Text>().text;

        text = text.Replace("{user}", SystemInfo.deviceName);

        GetComponent<Text>().text = text;
    }
}
