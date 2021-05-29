using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUIHandler : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private Slider _healthbar;
    private Enemy _enemy;
    private Animator _animator;
    private bool _canAttack;
    private void Awake()
    {
        GameLoader.CallOnComplete(Initialize);

    }

    private void Initialize()
    {        
        _animator = GetComponent<Animator>();
        //_BattleManager = FindObjectOfType<BattleManager>();
        //_spawnManager = ServiceLocator.Get<SpawnManager>();
        _enemy = GetComponent<Enemy>();
    }


    private void Start()
    {
        _healthbar.maxValue = _enemy.MaxHealth;
    }

    private void Update()
    {
        _healthbar.value = _enemy.CurrentHealth;
    }

    public void UpdateHealthBar()
    {
        _healthbar.value = _enemy.CurrentHealth;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
       if(stream.IsWriting)
        {
            stream.SendNext(_canAttack);
        }
       else
        {
            this._canAttack = (bool)stream.ReceiveNext();
        }
    }
}
