using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviourPun
{
    public System.Action OnAllPlayersInstantiated; 
    [SerializeField] private Transform _player1Pos;
    [SerializeField] private Transform _player2Pos;
    public List<GameObject> PlayerList;

    // Start is called before the first frame update
    void Start()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            GameObject player1 = PhotonNetwork.Instantiate("Player1", _player1Pos.position, Quaternion.identity);
        }
        else
        {
            GameObject player2 = PhotonNetwork.Instantiate("Player2", _player2Pos.position, Quaternion.identity);
            //this.photonView.RPC("InvokePlayerInstantiated", RpcTarget.All);
        }
    }


    [PunRPC]
    public void InvokePlayerInstantiated()
    {
        OnAllPlayersInstantiated?.Invoke();
    }

    //[PunRPC]
    //public void AddPlayerToList(int id)
    //{
    //    PlayerIDList.Add(id);
    //}
}
