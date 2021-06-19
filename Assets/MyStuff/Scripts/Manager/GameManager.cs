using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject gameManager;
    private SpawnManager _spawnManager;
    private TurnManager _TurnManager;
    public static GameManager Instance;
    private int playercount;
    [SerializeField] private Transform p1Pos;
    [SerializeField] private Transform p2Pos;
    [SerializeField] private Transform p3Pos;

    public Transform P1Pos { get => p1Pos; }
    public Transform P2Pos { get => p2Pos; }
    public Transform P3Pos { get => p3Pos; }

    private bool p1spawned;
    private bool p2spawned;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        _TurnManager = FindObjectOfType<TurnManager>();
        _spawnManager = FindObjectOfType<SpawnManager>();

    }

    private void Start()
    {
        Instance = this;

        if (Enemy.localPlayerInstance == null)
        {
            if (PhotonNetwork.IsMasterClient && PhotonNetwork.CountOfPlayers == 1)
            {
                Debug.LogFormat("We are Instantiating Master Client Player from {0}", SceneManagerHelper.ActiveSceneName);
                PhotonNetwork.Instantiate("Enemy", p3Pos.position, Quaternion.identity);
            }
        }

        if (WarriorSkills.localPlayerInstance == null && PhotonNetwork.CountOfPlayers == 2 && !p1spawned)
        {
            PhotonNetwork.Instantiate("Player1", p1Pos.position, Quaternion.identity);
            p1spawned = true;
        }
        if (ArcherSkills.localPlayerInstance == null && PhotonNetwork.CountOfPlayers == 3 && !p2spawned)
        {
            PhotonNetwork.Instantiate("Player2", p2Pos.position, Quaternion.identity);
            p2spawned = true;
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
        PhotonNetwork.LoadLevel("Room Battle");
    }

}
