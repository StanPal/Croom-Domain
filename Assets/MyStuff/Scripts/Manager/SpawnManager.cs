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

    public Transform Player1Pos { get => _player1Pos; }
    public Transform Player2Pos { get => _player2Pos; }


    void Start()
    {
      
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
