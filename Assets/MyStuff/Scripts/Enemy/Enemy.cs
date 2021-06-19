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
    [SerializeField] private CharacterClass _class = CharacterClass.Mage;
    private bool enemyHit;

    private float _enemyMaxHealth;
    private bool isStunned;
    private int _stunTimer;
    public float Speed { get => _enemySpeed; }
    public string PlayerName { get => _enemyName; }
    public float MaxHealth { get => _enemyMaxHealth; }
    public float CurrentHealth { get => _enemyHealth; set => _enemyHealth = value; }
    public float Attack { get => _enemyAttack; set => _enemyAttack = value; }
    public bool Stunned { get => isStunned; }
    public int StunTimer { get => _stunTimer; set => _stunTimer = value; }
    public CharacterClass ClassType { get => _class; }
    

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
        if(enemyHit)
        {
            this.photonView.RPC("PlayEnemyHitAnim", RpcTarget.All);
        }
        if(_stunTimer > 0)
        {
            this.photonView.RPC("PlayStun", RpcTarget.All);
        }
        if (_stunTimer == 0)
        {
            this.photonView.RPC("DisableStun", RpcTarget.All);
        }
    }

    public void TakeDamage(float damage)
    {
        _enemyHealth -= damage;
        //enemyHit = true;
        this.photonView.RPC("PlayEnemyHitAnim", RpcTarget.All);
        
    }

    public void DisableStunEffect()
    {
        this.photonView.RPC("DisableStun", RpcTarget.All);
    }

    [PunRPC]
    private void PlayStun()
    {
        _animator.SetBool("IsStun", true);
    }


    [PunRPC] 
    private void DisableStun()
    {
        _stunTimer = -1;
        _animator.SetBool("IsStun", false);
    }

    [PunRPC]
    private void PlayEnemyHitAnim()
    {
        _animator.SetTrigger("OnHitTrigger");
        //StartCoroutine(OnHitRoutine());
    }

    private IEnumerator OnHitRoutine()
    {
        enemyHit = false;
        _animator.SetBool("OnHit", true);
        yield return new WaitForSeconds(1.3f);
        _animator.SetBool("OnHit", false);

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

    public void onHeal(float healAmount)
    {
        Debug.Log("Enemy Health: " + _enemyHealth);
        if (_enemyHealth != _enemyMaxHealth)
        {
            _enemyHealth += healAmount;
        }
        if (_enemyHealth > _enemyMaxHealth)
        {
            _enemyHealth = _enemyMaxHealth;
        }
        Debug.Log("Enemy Health: " + _enemyHealth);
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
            stream.SendNext(_stunTimer);
        }
        else
        {
            //We are reading input to our health and write it back to our client and synced across the network
            this._enemyHealth = (float)stream.ReceiveNext();
            this._stunTimer = (int)stream.ReceiveNext();
        }
    }
}
