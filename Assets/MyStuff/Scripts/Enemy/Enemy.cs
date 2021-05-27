using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviourPunCallbacks, IPunObservable, IPunInstantiateMagicCallback
{
    public static GameObject localPlayerInstance;
    private SpawnManager _spawnManager;

    [SerializeField] private string _enemyName;
    [SerializeField] private float _enemyHealth;
    [SerializeField] private float _enemyAttack;
    [SerializeField] private float _enemySpeed;
    [SerializeField] private int _enemyLevel;

    private float _enemyMaxHealth;

    public float Speed { get => _enemySpeed; }
    public string PlayerName { get => _enemyName; }
    public float MaxHealth { get => _enemyMaxHealth; }
    public float CurrentHealth { get => _enemyHealth; set => _enemyHealth = value; }
    public float Attack { get => _enemyAttack; set => _enemyAttack = value; }

    private void Awake()
    {
        _spawnManager = FindObjectOfType<SpawnManager>();
        if(photonView.IsMine)
        {
            Enemy.localPlayerInstance = this.gameObject;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        _spawnManager.EnemyList.Add(this.gameObject);
        this.photonView.RPC("RotateModel", RpcTarget.All);
    }

    [PunRPC]
    private void RotateModel()
    {
        transform.rotation = Quaternion.Euler(0f, -90f, 0f);

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }
}
