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
    [SerializeField] private List<Button> _actionButtonlist = new List<Button>();
    private Animator _animator;
    private Model _model;
    private CharacterStats _characterStats;
    private BattleUI _battleUI;
    private BattleManager _battleManager;
    private bool _canMove = false;
    private bool _canAttack = false;
    private bool onHit;
    private SpawnManager _spawnManager;

    public Slider RecieverSlider { get => _healthbar; }
    public bool CanMove { get => _canMove; set => _canMove = value; }
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
            if (_canMove)
            {
                for (int i = 0; i < _actionButtonlist.Count; i++)
                {
                    _actionButtonlist[i].interactable = true;
                }
            }
            else
            {
                ResetActionButtons();
            }
        }
    }
    
    public void OnMove()
    {
        _canMove = true;

    }

    public void OnAttack()
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
        if (_characterStats.ClassType == CharacterStats.CharacterClass.Archer)
        {
            _animator.SetTrigger("ShotTrigger");
        }
        else if (_characterStats.ClassType == CharacterStats.CharacterClass.Warrior)
        {
            _animator.SetTrigger("SlashTrigger");

        }
        _battleUI.PunAttackOtherPlayer(this.gameObject);
        _canMove = false;
        ActionQueueCall();
    }

    public void ResetActionButtons()
    {
        for (int i = 0; i < _actionButtonlist.Count; i++)
        {
            _actionButtonlist[i].interactable = false;
        }
    }

    public void ActionQueueCall()
    {
        _battleManager.ActionQueue.Dequeue();
        Debug.Log("Current Queue Count: " + _battleManager.ActionQueue.Count);
        if (_battleManager.ActionQueue.Count == 0)
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_canAttack);
        }
        else
        {          
            this._canAttack = (bool)stream.ReceiveNext();
        }
    }
}
