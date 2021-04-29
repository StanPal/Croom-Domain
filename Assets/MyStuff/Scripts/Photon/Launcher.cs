using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Launcher : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _controlPanel;
    [SerializeField] private GameObject _progressLabel; 
    [SerializeField] private byte _maxPlayerPerRoom = 2;
    private bool _isConnecting;

    private void Awake()
    {
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    
    void Start()
    {
        Debug.Log("Connecting to server...");
        _progressLabel.SetActive(false);
        _controlPanel.SetActive(true);
    }

    public override void OnConnectedToMaster()
    {
        if(_isConnecting)
        {
            PhotonNetwork.JoinRandomRoom();
            _isConnecting = false; 
        }
        Debug.Log("Connected to server");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        _isConnecting = false; 
        Debug.Log("Disconnected from server: " + cause.ToString());
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one. Calling: PhotonNetwork.CreateRoom");

        // we failed to join a random room, maybe none exists or they are all full. Creating a new room.
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = _maxPlayerPerRoom });
    }

    public override void OnJoinedRoom()
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Debug.Log("Load Room for 1");
            PhotonNetwork.LoadLevel("Room for 1");
        }
        Debug.Log("Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
    }

    public void Connect()
    {
        _progressLabel.SetActive(true);
        _controlPanel.SetActive(false);

        //Check if we are connected or not, join if we are else instantiate a connection to the server
        if(PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            //Locks users to this specific version, helps block users who have different game versions from interacting with each other. 
            _isConnecting = PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = "0.0.1";
        }
    }
}
