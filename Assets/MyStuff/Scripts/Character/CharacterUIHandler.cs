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
    private BattleManager _BattleManager;
    private TurnManager _TurnManager;

    private bool _canMove = false;
    private bool _canAttack = false;
//    private bool _playerTurn = false;
//    private bool _actionsReset = false;
//    private bool _onHit;
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
        _BattleManager = FindObjectOfType<BattleManager>();
        _spawnManager = ServiceLocator.Get<SpawnManager>();
        _characterStats = GetComponent<CharacterStats>();
        _TurnManager = FindObjectOfType<TurnManager>();
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
 
        if(_characterStats.CombatState == CombatState.None)
        {
            ResetSkill();
        }
    }

    public void OnAttack()
    {
        _actionManager.AttackingOtherPlayer(this.gameObject, _characterStats.ClassType);
       // this.photonView.RPC("TakeDamage", RpcTarget.All);
    }

    public void UpdateHealthBar()
    {
        _healthbar.value = _characterStats.CurrentHealth;
  //      _onHit = false;
    }

    public void OnSkillOne()
    {
        _actionManager.InvokeSkill(this.gameObject, _characterStats.ClassType);
    }

    public void ResetSkill()
    {
        _actionManager.ResetSkillBehaviours(this.gameObject, _characterStats.ClassType); 
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
        Debug.Log("Current Queue Count: " + _TurnManager.ActionQueue.Count);
        _TurnManager.ActionQueue.Dequeue();
        _TurnManager.State = BattleState.TransitionPhase;
        
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
