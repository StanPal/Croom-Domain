using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState {SetupStage, Start, PlayerTurn, TransitionPhase, EnemyTurn, Won, Lost}

public class TurnManager : MonoBehaviourPun
{
    
    public static TurnManager Instance;
    public System.Action OnDamage;
    [SerializeField] private BattleState _state;
    [SerializeField] private Queue<GameObject> _actionQueue;
    [SerializeField] private int minCharacterCount = 2;
    //public GameObject playerPrefab;
    //public GameObject enemyPrefab;

    //public Transform playerSpawn;
    //public Transform enemySpawn;
    private SpawnManager _spawnManager;
    private CharacterStats _player1;
    private CharacterStats _player2;
    private Enemy _enemy1;
    public BattleState State { get => _state; set => _state = value; }
    public Queue<GameObject> ActionQueue { get => _actionQueue; }

    private void Awake()
    {
        ServiceLocator.Register<TurnManager>(this);
        _spawnManager = FindObjectOfType<SpawnManager>();
    }

    private void Initialize()
    {
       
    }

    // Start is called before the first frame update
    void Start()
    {
        _state = BattleState.SetupStage;
        _actionQueue = new Queue<GameObject>();
    }

    private void Update()
    {
        if(_state == BattleState.SetupStage)
        {
            SetUpPhase();
        }
        if (_state == BattleState.TransitionPhase)
        {
            TransitionPhase();
        }
    }

    public void PlayerDamaged(CharacterStats reciever, CharacterStats sender)
    {
        reciever.TakeDamage(sender.Attack);
        OnDamage?.Invoke();
    }

    private void SetUpPhase()
    {
        if(_spawnManager.PlayerList.Count >= minCharacterCount)
        {
            _player1 = _spawnManager.PlayerList[0].GetComponentInChildren<CharacterStats>();
            _player2 = _spawnManager.PlayerList[1].GetComponentInChildren<CharacterStats>();
            _enemy1 = _spawnManager.EnemyList[0].GetComponent<Enemy>();
            //_state = BattleState.Start;
            SetUpTurnQueue();
        }
    }

    private void SetUpTurnQueue()
    {
        if(_player1.Speed > _player2.Speed && _player1.Speed > _enemy1.Speed && _player2.Speed > _enemy1.Speed)
        {
            _actionQueue.Enqueue(_player1.gameObject);
            _actionQueue.Enqueue(_player2.gameObject);
            _actionQueue.Enqueue(_enemy1.gameObject);
        }
        else if (_player1.Speed > _player2.Speed && _player1.Speed > _enemy1.Speed && _enemy1.Speed > _player2.Speed)
        {
            _actionQueue.Enqueue(_player1.gameObject);
            _actionQueue.Enqueue(_enemy1.gameObject);
            _actionQueue.Enqueue(_player2.gameObject);
        }
        else if (_player2.Speed > _player1.Speed && _player2.Speed > _enemy1.Speed && _player1.Speed > _enemy1.Speed)
        {
            _actionQueue.Enqueue(_player2.gameObject);
            _actionQueue.Enqueue(_player1.gameObject);
            _actionQueue.Enqueue(_enemy1.gameObject);
        }
        else if (_player2.Speed > _player1.Speed && _player2.Speed > _enemy1.Speed && _player1.Speed < _enemy1.Speed)
        {
            _actionQueue.Enqueue(_player2.gameObject);
            _actionQueue.Enqueue(_enemy1.gameObject);
            _actionQueue.Enqueue(_player1.gameObject);
        }
        else if (_enemy1.Speed > _player1.Speed && _enemy1.Speed > _player2.Speed && _player1.Speed > _player2.Speed)
        {
            _actionQueue.Enqueue(_enemy1.gameObject);
            _actionQueue.Enqueue(_player1.gameObject);
            _actionQueue.Enqueue(_player2.gameObject);
        }
        else if (_enemy1.Speed > _player1.Speed && _enemy1.Speed > _player2.Speed && _player1.Speed < _player2.Speed)
        {
            _actionQueue.Enqueue(_enemy1.gameObject);
            _actionQueue.Enqueue(_player2.gameObject);
            _actionQueue.Enqueue(_player1.gameObject);
        }

        Debug.Log(_actionQueue.Count);
        if (_actionQueue.Peek() == _enemy1.gameObject)
        {
            EnemyTurn();
        }
        else
        {
            PlayerTurn();
        }
    }

    private void TransitionPhase()
    {
        Debug.Log("Action Queue Count: " + ActionQueue.Count);
        if (ActionQueue.Count.Equals(0))
        {
            _state = BattleState.SetupStage;
            SetUpTurnQueue();
        }
        else if (_actionQueue.Peek().TryGetComponent<EnemyUIHandler>(out EnemyUIHandler enemy))
        {
            EnemyTurn();
            _state = BattleState.EnemyTurn;
        }
        else if (_actionQueue.Peek().TryGetComponent<CharacterUIHandler>(out CharacterUIHandler player))
        {
            _state = BattleState.PlayerTurn;
            PlayerTurn();
        }
    }


    public void PlayerTurn()
    {
        if(_actionQueue.Peek().TryGetComponent<CharacterUIHandler>(out CharacterUIHandler characterUI))
        {   
            characterUI.OnMove(); 
        }
    }

    private void EnemyTurn()
    {
       if(_actionQueue.Peek().TryGetComponent<EnemyUIHandler>(out EnemyUIHandler enemyUI))
        {   
            enemyUI.OnAttack();
        }
    }

}
