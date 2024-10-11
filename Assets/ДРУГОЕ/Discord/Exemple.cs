using DiscordPresence;
using UnityEngine;

public class Exemple : MonoBehaviour {
    public string detail;
    public string state;

    void OnEnable() => PresenceManager.UpdatePresence(detail: detail, state: state);
}