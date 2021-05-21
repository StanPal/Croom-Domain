using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class CharacterUIHandler : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private Text playerName;
    [SerializeField] private Slider _healthbar;
    [SerializeField] private Button _attackBtn;
    private Animator _animator;
    private Model _model;
    private CharacterStats _characterStats;
    private BattleUI _battleUI;
    private BattleManager _battleManager;
    private bool _canAttack = false;
    private bool onHit;
    private SpawnManager _spawnManager;

    public Slider RecieverSlider { get => _healthbar; }    
    public bool CanAttack { get => _canAttack; set => _canAttack = value; }
    public Animator Animator { get => _animator; set => _animator = value; }

    private void Awake()
    {
        GameLoader.CallOnComplete(Initialize);
        
    }

    private void Initialize()
    {
        _animator = GetComponent<Animator>();
        _battleUI = FindObjectOfType<BattleUI>();
        _spawnManager = ServiceLocator.Get<SpawnManager>();        
        _characterStats = GetComponent<CharacterStats>();
        _battleManager = FindObjectOfType<BattleManager>();
    }

    void Start()
    {
        _attackBtn.interactable = false;
        
        _healthbar.maxValue = _characterStats.MaxHealth;
        playerName.text = _characterStats.PlayerName;
    }
   
    void Update()
    {
        _healthbar.value = _characterStats.CurrentHealth;
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }
        else
        {
            if (_canAttack)
            {
                _attackBtn.interactable = true;
            }
            else
            {
                _attackBtn.interactable = false;
            }
        }
    }
    
    public void OnAttack()
    {
        _canAttack = true;

    }

    public void OnHit()
    {        
        this.photonView.RPC("TakeDamage", RpcTarget.All);    
    }


    public void UpdateHealthBar()
    {
        _healthbar.value = _characterStats.CurrentHealth;
        onHit = false;
    }

    [PunRPC]
    private void PunOnAttack()
    {
        _canAttack = true;
    }

    [PunRPC]
    private void TakeDamage()
    {
        //if(_spawnManager.PlayerList[0].GetComponent<CharacterUIHandler>().CanAttack)
        //{
        //    _spawnManager.PlayerModelList[0].GetComponent<Model>().IsAttacking = true;
        //}
        //if(_spawnManager.PlayerList[1].GetComponent<CharacterUIHandler>().CanAttack)
        //{
        //    _spawnManager.PlayerModelList[1].GetComponent<Model>().IsAttacking = true;
        //}
        if (_characterStats.ClassType == CharacterStats.CharacterClass.Archer)
        {
            _animator.SetTrigger("ShotTrigger");
        }
        else if (_characterStats.ClassType == CharacterStats.CharacterClass.Warrior)
        {
            _animator.SetTrigger("SlashTrigger");

        }
        _battleUI.PunAttackOtherPlayer(this.gameObject);
        _canAttack = false;
        //_attackBtn.interactable = false;
        _battleManager.ActionQueue.Dequeue();
        if(_battleManager.ActionQueue.Count == 0)
        {
            _battleManager.State = BattleState.Start;
        }
        else if (_battleManager.State == BattleState.EnemyTurn)
        {
            _battleManager.State = BattleState.PlayerTurn;
        }
        else if (_battleManager.State == BattleState.PlayerTurn)
        {
            _battleManager.State = BattleState.EnemyTurn;
        }
    }

    [PunRPC]
    public void UpdateHealthBarPun()
    {
        _healthbar.value = _characterStats.CurrentHealth;
        onHit = false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_canAttack);
        }
        else
        {
            //We are reading input to our health and write it back to our client and synced across the network
            this._canAttack = (bool)stream.ReceiveNext();
        }
    }
}
