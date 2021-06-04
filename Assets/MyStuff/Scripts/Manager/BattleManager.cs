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
    private GameManager _gameManager;
    private GameObject player1;
    private GameObject player2;
    private bool _isMoving;

    private void Awake()
    {
        GameLoader.CallOnComplete(Initialize);
    }

    private void Initialize()
    {
        _spawnManager = ServiceLocator.Get<SpawnManager>();
        _TurnManager = FindObjectOfType<TurnManager>();
        _gameManager = FindObjectOfType<GameManager>();
    }

    public void EnemyAttackPlayer(float damage)
    {
        this.photonView.RPC("PunAttackPlayer", RpcTarget.All, damage);
    }

    [PunRPC]
    public void PunAttackOtherPlayer(GameObject player, float damage, NegativeStatusEffect negativeStatus)
    {
        if (_spawnManager.PlayerList[0] == player)
        {
            _spawnManager.EnemyList[0].GetComponent<Enemy>().TakeDamage(damage);
            _spawnManager.EnemyList[0].GetComponent<EnemyUIHandler>().UpdateHealthBar();
            _spawnManager.EnemyList[0].GetComponent<Enemy>().OnStatusEffect(negativeStatus);

        }
        else
        {
            _spawnManager.EnemyList[0].GetComponent<Enemy>().TakeDamage(damage);
            _spawnManager.EnemyList[0].GetComponent<EnemyUIHandler>().UpdateHealthBar();
            _spawnManager.EnemyList[0].GetComponent<Enemy>().OnStatusEffect(negativeStatus);
        }

        //if (_spawnManager.PlayerList[0] == player)
        //{
        //    _spawnManager.PlayerList[1].GetComponentInChildren<CharacterStats>().TakeDamage(damage);
        //    _spawnManager.PlayerList[1].GetComponentInChildren<CharacterStats>().OnStatusEffect(negativeStatus);
        //    _spawnManager.PlayerList[1].GetComponent<CharacterUIHandler>().UpdateHealthBar();

        //}
        //else
        //{
        //    _spawnManager.PlayerList[0].GetComponentInChildren<CharacterStats>().TakeDamage(damage);
        //    _spawnManager.PlayerList[0].GetComponent<CharacterUIHandler>().UpdateHealthBar();
        //}
    }

    [PunRPC]
    public void PunAttackPlayer (float damage)
    {
        int targetPlayer = Random.Range(0, _spawnManager.PlayerList.Count);
        Debug.Log("Enemy Chose: " + targetPlayer);
        _spawnManager.PlayerList[targetPlayer].GetComponent<CharacterStats>().TakeDamage(damage);
    }


}
