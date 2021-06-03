using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawn : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback, IPunObservable
{
    [SerializeField] private bool _isPlayer = false;
    private SpawnManager _spawnManager;


    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        _spawnManager = ServiceLocator.Get<SpawnManager>();
    }


    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        _spawnManager.PlayerList.Add(this.gameObject);
        this.photonView.RPC("rotateModel", RpcTarget.All);
    }

    [PunRPC]
    private void rotateModel()
    {
        if (_isPlayer)
        {
            transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, -90f, 0f);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //Sync Health & name
        if (stream.IsWriting)
        {
            stream.SendNext(_isPlayer);
        }
        else
        {
            //We are reading input to our health and write it back to our client and synced across the network
            this._isPlayer = (bool)stream.ReceiveNext();
        }
    }
}
