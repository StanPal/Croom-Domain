using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageSkills : MonoBehaviourPun
{
    private Animator _animator;
    private EnemyUIHandler _enemyUIHandler;
    private Enemy _enemy;
    private BattleManager _battleManager;
    [SerializeField] private int _skillCooldown = 2;
    public int SkillCoolDown { set => _skillCooldown = value; }

    private void Awake()
    {
        GameLoader.CallOnComplete(Initialize);
    }

    private void Initialize()
    {
        _animator = GetComponent<Animator>(); 
        _battleManager = FindObjectOfType<BattleManager>();
        _enemy = GetComponent<Enemy>();
        _enemyUIHandler = GetComponent<EnemyUIHandler>();
    }   

    public void OnNormalAttack()
    {
        this.photonView.RPC("PunNormalAttack", RpcTarget.All);
    }

    [PunRPC]
    private void PunNormalAttack()
    {        
        _animator.SetTrigger("CastTrigger");
        _battleManager.EnemyAttackPlayer(_enemy.Attack);
        _enemyUIHandler.ActionQueueCall();
    }
}
