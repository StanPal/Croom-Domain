using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { Start, PlayerTurn, EnmeyTurn, Won, Lost}

public class BattleManager : MonoBehaviour
{
    public BattleState State;
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerSpawn;
    public Transform enemySpawn;

    CharacterStats playerStats;
    CharacterStats enemyStats; 

    // Start is called before the first frame update
    void Start()
    {
        State = BattleState.Start;
        SetUpBattle();
    }

    void SetUpBattle()
    {
        GameObject playerOne = Instantiate(playerPrefab, playerSpawn);
        playerOne.GetComponent<CharacterStats>();
        Instantiate(enemyPrefab, enemySpawn);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
