using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;

public class BattleUI : MonoBehaviourPun
{    
    [SerializeField] private BattleManager _battleManager;
    [SerializeField] private Slider _PlayerOneHealthBar;
    [SerializeField] private Slider _PlayerTwoHealthBar;

    private SpawnManager _spawnManager;
    private PlayerListManager _playerListManager;
    private CharacterStats player1;
    private CharacterStats player2;

    private void Awake()
    {
        _battleManager = FindObjectOfType<BattleManager>();
        _spawnManager = FindObjectOfType<SpawnManager>();
        _playerListManager = FindObjectOfType<PlayerListManager>();
    }

    private void Start()
    {
        foreach (GameObject character in _spawnManager.PlayerList)
        {
            character.GetComponent<CharacterUIHandler>().InvokeOnHit += AttackOtherPlayer;
        }
    }

    private void SetUpHealthBars()
    {
        //player1 = _spawnManager.PlayerList[0].GetComponent<CharacterStats>();
        //player2 = _spawnManager.PlayerList[1].GetComponent<CharacterStats>();
        //this.photonView.RPC("SetUpHealthBarCallBack",RpcTarget.All, player1.MaxHealth, player2.MaxHealth);
    }

    [PunRPC]
    private void SetUpHealthBarCallBack(int p1Health, int p2Health)
    {
        _PlayerOneHealthBar.maxValue = player1.MaxHealth;
        _PlayerTwoHealthBar.maxValue = player2.MaxHealth;


    }
    private void UpdateHealthBar()
    {
        _PlayerOneHealthBar.value = player1.CurrentHealth;
        _PlayerTwoHealthBar.value = player2.CurrentHealth;
    }

    public void AttackOtherPlayer()
    {
        _spawnManager.PlayerList[0].GetComponent<CharacterStats>().TakeDamage(_spawnManager.PlayerList[1].GetComponent<CharacterStats>().Attack);
    }

    public void AttackPlayerOne()
    {
        _battleManager.PlayerDamaged(player1, player2);
    }

    public void AttackPlayerTwo()
    {
        _battleManager.PlayerDamaged(player2, player1);
    }

}
