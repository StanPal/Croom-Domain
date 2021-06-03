using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviourPunCallbacks, IPunObservable, IPunInstantiateMagicCallback
{
    public static GameObject localPlayerInstance;
    private SpawnManager _spawnManager;
    private Animator _animator;

    [SerializeField] private string _enemyName;
    [SerializeField] private float _enemyHealth;
    [SerializeField] private float _enemyAttack;
    [SerializeField] private float _enemySpeed;
    [SerializeField] private int _enemyLevel;

    private float _enemyMaxHealth;
    private int _stunTimer;
    public float Speed { get => _enemySpeed; }
    public string PlayerName { get => _enemyName; }
    public float MaxHealth { get => _enemyMaxHealth; }
    public float CurrentHealth { get => _enemyHealth; set => _enemyHealth = value; }
    public float Attack { get => _enemyAttack; set => _enemyAttack = value; }

    private void Awake()
    {
        _spawnManager = FindObjectOfType<SpawnManager>();
        _enemyMaxHealth = _enemyHealth;
        if(photonView.IsMine)
        {
            Enemy.localPlayerInstance = this.gameObject;
        }
        DontDestroyOnLoad(this.gameObject);
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if(CurrentHealth <= 0f)
        {
            this.gameObject.SetActive(false);
        }
    }

    public void TakeDamage(float damage)
    {
        _enemyHealth -= damage;
        _animator.SetTrigger("OnHitTrigger");
    }

    public void OnStatusEffect(NegativeStatusEffect negativeStatus)
    {
        switch (negativeStatus)
        {
            case NegativeStatusEffect.None:
                break;
            case NegativeStatusEffect.Stunned:
                _stunTimer = 1;
                break;
            case NegativeStatusEffect.Burning:
                break;
            default:
                break;
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        _spawnManager.EnemyList.Clear();
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
        if (stream.IsWriting)
        {
            stream.SendNext(CurrentHealth);
        }
        else
        {
            //We are reading input to our health and write it back to our client and synced across the network
            this._enemyHealth = (float)stream.ReceiveNext();
        }
    }
}
