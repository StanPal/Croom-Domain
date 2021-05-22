using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class CharacterUIHandler : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private Text _playerName;
    [SerializeField] private Slider _healthbar;
    [SerializeField] private Button _attackBtn;
    [SerializeField] private List<Button> _actionButtonlist = new List<Button>();
    private Animator _animator;
    private ActionManager _actionManager;
    private Model _model;
    private CharacterStats _characterStats;
    private BattleUI _battleUI;
    private BattleManager _battleManager;

    private bool _canMove = false;
    private bool _canAttack = false;
    private bool _playerTurn = false;
    private bool _actionsReset = false;
    private bool _onHit;
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
        _actionManager = FindObjectOfType<ActionManager>();   
        _animator = GetComponent<Animator>();
        _battleUI = FindObjectOfType<BattleUI>();
        _spawnManager = ServiceLocator.Get<SpawnManager>();
        _characterStats = GetComponent<CharacterStats>();
        _battleManager = FindObjectOfType<BattleManager>();
    }

    void Start()
    {
        ResetActionButtons();
        ResetSkill();
        _healthbar.maxValue = _characterStats.MaxHealth;
        _playerName.text = _characterStats.PlayerName;
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
        if(_characterStats.CombatState == CombatState.None)
        {
            ResetSkill();
        }
    }

    public void OnAttack()
    {        
        this.photonView.RPC("TakeDamage", RpcTarget.All);
    }

    public void UpdateHealthBar()
    {
        _healthbar.value = _characterStats.CurrentHealth;
        _onHit = false;
    }

    public void OnSkillOne()
    {
        _actionManager.InvokeSkill(this.gameObject, _characterStats.ClassType);
    }

    public void ResetSkill()
    {
        _actionManager.ResetSkillBehaviours(this.gameObject, _characterStats.ClassType); 
    }

    [PunRPC]
    private void TakeDamage()
    {
        if (_characterStats.ClassType == CharacterClass.Archer)
        {
            _animator.SetTrigger("ShotTrigger");
        }
        else if (_characterStats.ClassType == CharacterClass.Warrior)
        {
            _animator.SetTrigger("SlashTrigger");

        }
        _battleUI.PunAttackOtherPlayer(this.gameObject);
        Invoke("ResetSkill",0.5f);
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
        _canMove = false;        
        if (_battleManager.ActionQueue.Count > 0)
        {
            _battleManager.ActionQueue.Dequeue();
        }
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
