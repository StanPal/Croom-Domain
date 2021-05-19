using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    private SpawnManager _spawnManager;
    private Animator _animator;
    private bool _onAttack = false;

    public bool IsAttacking { get => _onAttack; set => _onAttack = value; }

    private void Awake()
    {
        _spawnManager = FindObjectOfType<SpawnManager>();
        _animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(_onAttack)
        {
             this.photonView.RPC("PunAttackTrigger", RpcTarget.All);
        }
    }



    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        _spawnManager.PlayerModelList.Add(this.gameObject);
        if(_spawnManager.PlayerModelList.Count == 1)
        {
             this.photonView.RPC("RotatePlayer1Instantiated", RpcTarget.All); 
        }
        if(_spawnManager.PlayerModelList.Count == 2)
        {
             this.photonView.RPC("RotatePlayer2Instantiated", RpcTarget.All);
        }
    }

    [PunRPC]
    public void RotatePlayer1Instantiated()
    {
        _spawnManager.PlayerModelList[0].transform.parent = _spawnManager.PlayerList[0].transform;
        _spawnManager.PlayerModelList[0].transform.localScale = _spawnManager.PlayerList[0].transform.localScale;
        _spawnManager.PlayerModelList[0].transform.rotation = Quaternion.Euler(0f, 90f, 0f);
       // _spawnManager.PlayerList[0].GetComponent<CharacterUIHandler>().Animator = _spawnManager.PlayerModelList[0].GetComponent<Animator>();
    }

    [PunRPC]
    public void RotatePlayer2Instantiated()
    {
        _spawnManager.PlayerModelList[1].transform.parent = _spawnManager.PlayerList[1].transform;
        _spawnManager.PlayerModelList[1].transform.localScale = _spawnManager.PlayerList[1].transform.localScale;
        //PlayerModelList[1].transform.rotation = Quaternion.Euler(0f, 90f, 0f);
       // _spawnManager.PlayerList[1].GetComponent<CharacterUIHandler>().Animator = _spawnManager.PlayerModelList[1].GetComponent<Animator>();
    }

    [PunRPC]
    public void PunAttackTrigger()
    {
        _animator.SetTrigger("AttackTrigger");
        _onAttack = false;
    }
}
