using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { Start, PlayerTurn, EnmeyTurn, Won, Lost}

public class BattleManager : MonoBehaviour
{
    public System.Action OnDamage;
    
    private SpawnManager _spawnManager;
    public BattleState State;
    //public GameObject playerPrefab;
    //public GameObject enemyPrefab;

    //public Transform playerSpawn;
    //public Transform enemySpawn;

    CharacterStats player1;
    CharacterStats player2;



    private void Awake()
    {
        _spawnManager = FindObjectOfType<SpawnManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        State = BattleState.Start;

    }

    private void Update()
    {
        switch (State)
        {
            case BattleState.Start:
                break;
            case BattleState.PlayerTurn:
                break;
            case BattleState.EnmeyTurn:
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
}
