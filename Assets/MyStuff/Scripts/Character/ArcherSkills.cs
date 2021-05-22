using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherSkills : MonoBehaviourPun
{
    private Animator _animator;
    private CharacterUIHandler _characterUIHandler;
    private CharacterStats _characterStats;
    private BattleManager _battleManager;

    private void Awake()
    {
        GameLoader.CallOnComplete(Initialize);
    }

    private void Initialize()
    {
        _animator = GetComponent<Animator>();
        _characterStats = GetComponent<CharacterStats>();
        _characterUIHandler = GetComponent<CharacterUIHandler>();
        _battleManager = FindObjectOfType<BattleManager>();        
    }

    public void OnNormalAttack()
    {
        this.photonView.RPC("PunNormalAttack", RpcTarget.All);
    }

    [PunRPC]
    private void PunNormalAttack()
    {
        _animator.SetTrigger("ShotTrigger");
        _battleManager.PunAttackOtherPlayer(this.gameObject, _characterStats.Attack);
        _characterUIHandler.ActionQueueCall();
    }
}
