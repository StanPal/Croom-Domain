using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{

    private SpawnManager _spawnManager;

    private void Awake()
    {
        _spawnManager = FindObjectOfType<SpawnManager>();
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate("Player1", _spawnManager.Player1Pos.position, Quaternion.identity);
            PhotonNetwork.Instantiate("Paladin", _spawnManager.Player1Pos.position, Quaternion.identity);
        }
        else
        {
            PhotonNetwork.Instantiate("Player2", _spawnManager.Player2Pos.position, Quaternion.identity);
            PhotonNetwork.Instantiate("Archer", _spawnManager.Player2Pos.position, Quaternion.identity);
        }
    }

    /// Called when the local player left the room. We need to load the launcher scene.
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnPlayerEnteredRoom(Player other)
    {
        // not seen if you're the player connecting
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); 
        if (PhotonNetwork.IsMasterClient)
        {
            // called before OnPlayerLeftRoom
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); 
            LoadArena();
        }
    }

    public override void OnPlayerLeftRoom(Player other)
    {
        // seen when other disconnects
        Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); 
        if (PhotonNetwork.IsMasterClient)
        {
            // called before OnPlayerLeftRoom
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); 
            LoadArena();
        }
    }

    public void LoadArena()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
        }
        Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
        PhotonNetwork.LoadLevel("Room for " + 1);
    }

}
