using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviourPun
{
    public System.Action OnAllPlayersInstantiated;
    [SerializeField] private Transform _player1Pos;
    [SerializeField] private Transform _player2Pos;
    public List<GameObject> PlayerList;
    public List<GameObject> PlayerModelList;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate("Player1", _player1Pos.position, Quaternion.identity);
            PhotonNetwork.Instantiate("Paladin", _player1Pos.position, Quaternion.identity);
        }
        else
        {
            PhotonNetwork.Instantiate("Player2", _player2Pos.position, Quaternion.identity);
            PhotonNetwork.Instantiate("Archer", _player2Pos.position, Quaternion.identity);
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
