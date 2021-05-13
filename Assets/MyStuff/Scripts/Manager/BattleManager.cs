using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState {SetupStage, Start, PlayerTurn, EnemyTurn, Won, Lost}

public class BattleManager : MonoBehaviour
{

    public System.Action OnDamage;
    [SerializeField] private BattleState _state;
    [SerializeField] private Queue<CharacterStats> _actionQueue;
    [SerializeField] private int minCharacterCount = 2;
    //public GameObject playerPrefab;
    //public GameObject enemyPrefab;

    //public Transform playerSpawn;
    //public Transform enemySpawn;
    private SpawnManager _spawnManager;
    private CharacterStats _player1;
    private CharacterStats _player2;
    public BattleState State { get => _state; set => _state = value; }
    public Queue<CharacterStats> ActionQueue { get => _actionQueue; }

    private void Awake()
    {
        _spawnManager = FindObjectOfType<SpawnManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _state = BattleState.SetupStage;
        _actionQueue = new Queue<CharacterStats>();
    }

    private void Update()
    {
        switch (_state)
        {
            case BattleState.SetupStage:
                SetUpPhase();
                break;
            case BattleState.Start:
                SetUpTurnQueue();
                break;
            case BattleState.PlayerTurn:
                PlayerTurn();
                break;
            case BattleState.EnemyTurn:
                EnemyTurn();
                break;
            case BattleState.Won:
                break;
            case BattleState.Lost:
                break;
            default:
                break;
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
            _player1 = _spawnManager.PlayerList[0].GetComponent<CharacterStats>();
            _player2 = _spawnManager.PlayerList[1].GetComponent<CharacterStats>();
            _state = BattleState.Start;
        }
    }

    private void SetUpTurnQueue()
    {
        if(_player1.Speed > _player2.Speed)
        {
            _actionQueue.Enqueue(_player1);
            _actionQueue.Enqueue(_player2);
        }
        else
        {
            _actionQueue.Enqueue(_player2);
            _actionQueue.Enqueue(_player1);
        }

        _state = BattleState.PlayerTurn;
    }

    private void PlayerTurn()
    {
        _actionQueue.Peek().CharacterUIHandler.OnAttack();
    }

    private void EnemyTurn()
    {
        _actionQueue.Peek().CharacterUIHandler.OnAttack();
    }

}
