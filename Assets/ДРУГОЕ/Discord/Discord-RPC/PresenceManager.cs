using System;
using UnityEngine;
using UnityEngine.Events;

namespace DiscordPresence {

    [Serializable]
    public class DiscordJoinEvent : UnityEvent<string> {
    }

    [Serializable]
    public class DiscordSpectateEvent : UnityEvent<string> {
    }

    [Serializable]
    public class DiscordJoinRequestEvent : UnityEvent<DiscordRpc.JoinRequest> {
    }

    public class PresenceManager : MonoBehaviour {
        public static PresenceManager Instance;

        public DiscordRpc.RichPresence presence = new();
        public string applicationId;
        public string optionalSteamId;
        public DiscordRpc.JoinRequest joinRequest;
        public UnityEvent onConnect;
        public UnityEvent onDisconnect;
        public UnityEvent hasResponded;
        public DiscordJoinEvent onJoin;
        public DiscordJoinEvent onSpectate;
        public DiscordJoinRequestEvent onJoinRequest;

        DiscordRpc.EventHandlers handlers;

        public void RequestRespondYes() {
            Debug.Log("Discord: responding yes to Ask to Join request");
            DiscordRpc.Respond(joinRequest.userId, DiscordRpc.Reply.Yes);
            hasResponded.Invoke();
        }

        public void RequestRespondNo() {
            Debug.Log("Discord: responding no to Ask to Join request");
            DiscordRpc.Respond(joinRequest.userId, DiscordRpc.Reply.No);
            hasResponded.Invoke();
        }

        #region Discord Callbacks
        public void ReadyCallback() {
            Debug.Log("Discord: ready");
            onConnect.Invoke();
        }

        public void DisconnectedCallback(int errorCode, string message) {
            Debug.Log(string.Format("Discord: disconnect {0}: {1}", errorCode, message));
            onDisconnect.Invoke();
        }

        public void ErrorCallback(int errorCode, string message) {
            Debug.Log(string.Format("Discord: error {0}: {1}", errorCode, message));
        }

        public void JoinCallback(string secret) {
            Debug.Log(string.Format("Discord: join ({0})", secret));
            onJoin.Invoke(secret);
        }

        public void SpectateCallback(string secret) {
            Debug.Log(string.Format("Discord: spectate ({0})", secret));
            onSpectate.Invoke(secret);
        }

        public void RequestCallback(ref DiscordRpc.JoinRequest request) {
            Debug.Log(string.Format("Discord: join request {0}#{1}: {2}", request.username, request.discriminator, request.userId));
            joinRequest = request;
            onJoinRequest.Invoke(request);
        }
        #endregion

        #region Monobehaviour Callbacks

        void Awake() {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
        }

        void Update() => DiscordRpc.RunCallbacks();

        void OnEnable() {
            handlers = new() {
                readyCallback = ReadyCallback
            };

            handlers.disconnectedCallback += DisconnectedCallback;
            handlers.errorCallback += ErrorCallback;
            handlers.joinCallback += JoinCallback;
            handlers.spectateCallback += SpectateCallback;
            handlers.requestCallback += RequestCallback;

            DiscordRpc.Initialize(applicationId, ref handlers, true, optionalSteamId);
        }

        void OnDisable() => DiscordRpc.Shutdown();
        #endregion

        #region Update Presence Method
        public static void UpdatePresence(string detail = null, string state = null, long start = -1, long end = -1, string largeKey = null, string largeText = null, string smallKey = null, string smallText = null, string partyId = null, int size = -1, int max = -1, string match = null, string join = null, string spectate = null) {
            Instance.presence.details = detail ?? Instance.presence.details;
            Instance.presence.state = state ?? Instance.presence.state;
            Instance.presence.startTimestamp = (start == -1) ? Instance.presence.startTimestamp : start;
            Instance.presence.endTimestamp = (end == -1) ? Instance.presence.endTimestamp : end;
            Instance.presence.largeImageKey = largeKey ?? Instance.presence.largeImageKey;
            Instance.presence.largeImageText = largeText ?? Instance.presence.largeImageText;
            Instance.presence.smallImageKey = smallKey ?? Instance.presence.smallImageKey;
            Instance.presence.smallImageText = smallText ?? Instance.presence.smallImageText;
            Instance.presence.partyId = partyId ?? Instance.presence.partyId;
            Instance.presence.partySize = (size == -1) ? Instance.presence.partySize : size;
            Instance.presence.partyMax = (max == -1) ? Instance.presence.partyMax : max;
            Instance.presence.matchSecret = match ?? Instance.presence.matchSecret;
            Instance.presence.joinSecret = join ?? Instance.presence.joinSecret;
            Instance.presence.spectateSecret = spectate ?? Instance.presence.spectateSecret;
            DiscordRpc.UpdatePresence(Instance.presence);
        }

        public static void ClearPresence() {
            Instance.presence.details = "";
            Instance.presence.state = "";
            Instance.presence.startTimestamp = 0;
            Instance.presence.endTimestamp = 0;
            Instance.presence.largeImageKey = "";
            Instance.presence.largeImageText = "";
            Instance.presence.smallImageText = "";
            Instance.presence.smallImageKey = "";
            Instance.presence.partyId = "";
            Instance.presence.partySize = 0;
            Instance.presence.partyMax = 0;
            Instance.presence.matchSecret = "";
            Instance.presence.joinSecret = "";
            Instance.presence.spectateSecret = "";
        }

        public static void ClearAndUpdate() {
            ClearPresence();
            DiscordRpc.UpdatePresence(Instance.presence);
        }
        #endregion
    }
}
