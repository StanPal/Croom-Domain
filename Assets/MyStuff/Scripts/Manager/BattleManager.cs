using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;

public class BattleManager : MonoBehaviourPun
{
    [SerializeField] private TurnManager _TurnManager;
    [SerializeField] private Slider _PlayerOneHealthBar;
    [SerializeField] private Slider _PlayerTwoHealthBar;

    private SpawnManager _spawnManager;

    private GameObject player1;
    private GameObject player2;

    private void Awake()
    {
        GameLoader.CallOnComplete(Initialize);
    }

    private void Initialize()
    {
        _spawnManager = ServiceLocator.Get<SpawnManager>();
        _TurnManager = ServiceLocator.Get<TurnManager>();
    }

    [PunRPC]
    public void PunAttackOtherPlayer(GameObject player)
    {
        if (_spawnManager.PlayerList[0] == player)
        {
            _spawnManager.PlayerList[1].GetComponentInChildren<CharacterStats>().TakeDamage(_spawnManager.PlayerList[0].GetComponentInChildren<CharacterStats>().Attack);
            _spawnManager.PlayerList[1].GetComponent<CharacterUIHandler>().UpdateHealthBar();

        }
        else
        {
            _spawnManager.PlayerList[0].GetComponentInChildren<CharacterStats>().TakeDamage(_spawnManager.PlayerList[1].GetComponentInChildren<CharacterStats>().Attack);
            _spawnManager.PlayerList[0].GetComponent<CharacterUIHandler>().UpdateHealthBar();
        }
    }


}
