using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private string lobbySceneName = ""; // Used to change to the lobby scene, if the player leaves.
    [SerializeField] private GameObject playerPrefab = null; // Used to spawn a player prefab model.

    private void Start()
    {
        PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(Random.Range(-2, 2), 1f, Random.Range(-2, 2)), Quaternion.identity);
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(lobbySceneName); // Sends us back to the lobby scene.
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom(); // Tells the network that we're leaving the room.
    }

    private void LoadArena()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master client");
        }
        
    }

    public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting


        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
        }
    }


    public override void OnPlayerLeftRoom(Player other)
    {
        Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects


        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
        }
    }
}
