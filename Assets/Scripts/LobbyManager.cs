using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    const string playerNamePrefKey = "PlayerName"; // Used to store the players name in player prefs.

    [SerializeField] private string gameSceneName = ""; // Used to change scene when we are join a room.
    [SerializeField] private GameObject controlPanel = null; // Used to show/hide the play button and input field.
    [SerializeField] private GameObject progressLabel = null; // Used to display "Connecting..." to once the Connect() funtion is called.
    [SerializeField] private InputField playerNameInput = null; // Used to retrieve the player's entered nickname.
    [SerializeField] private byte maxPlayersPerRoom = 5; // Used to set a limit to the number of players in a room.

    private string gameVersion = "1"; // Used to separate users from each other by gameVersion.
    private bool isConnecting; // Used to stop us from immediately joining the room if we leave it.

    private void Awake()
    {
        // Means we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        controlPanel.SetActive(true);
        progressLabel.SetActive(false);
    }

    public void Connect()
    {
        if (playerNameInput.text == "") // If the players input is blank, print an error.
        {
            Debug.LogError("Assets/Scripts/Launcher: No username has been set.");
            return;
        }

        // Otherwise, set the player prefs variable to the player's input, and set it as their photon network nickname.
        PlayerPrefs.SetString(playerNamePrefKey, playerNameInput.text);
        PhotonNetwork.NickName = playerNameInput.text;

        // Hide the play button and the player name input field, and show the 'connecting...' text.
        controlPanel.SetActive(false);
        progressLabel.SetActive(true);

        isConnecting = true; // Stops us from immediately joining the room, again, if we leave.

        if (PhotonNetwork.IsConnected) // Are we connected to the Photon Online Server?
        {
            // Attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            // Otherwise, connect to Photon Online Server.
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        if (isConnecting)
        {
            Debug.Log("Assets/Scripts/Launcher: OnConnectedToMaster() was called.");
            PhotonNetwork.JoinRandomRoom(); //Try and join a potential existing room, else, we'll call OnJoinRandomFailed().
        }
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        controlPanel.SetActive(true);
        progressLabel.SetActive(false);
        Debug.LogWarningFormat("Assets/Scripts/Launcher: OnDisconnected() was called with reason {0}.", cause);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Assets/Scripts/Launcher: OnJoinRandomFailed() was called, as no random room available, so we create one by calling PhotonNetwork.CreateRoom.");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }

    public override void OnJoinedRoom()
    {
        Debug.LogFormat("We load the {0}", gameSceneName);
        PhotonNetwork.LoadLevel(gameSceneName);
        Debug.Log("Assets/Scripts/Launcher: OnJoinedRoom() called, so this client is now in a room.");
    }
}
