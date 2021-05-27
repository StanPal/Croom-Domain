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
    [SerializeField] private int _skillCooldown = 2;
    public int SkillCoolDown { set => _skillCooldown = value; }

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

    public void OnDeactiveHide()
    {
        this.photonView.RPC("PunDeActivateHide", RpcTarget.All);

    }

    public void OnHide()
    {
        this.photonView.RPC("PunHideSkill", RpcTarget.All);
    }

    public void OnKickSkill()
    {
        this.photonView.RPC("PunKickSkill", RpcTarget.All);
    }

    public void OnNormalAttack()
    {
        this.photonView.RPC("PunNormalAttack", RpcTarget.All);
    }

    [PunRPC]
    private void PunNormalAttack()
    {
        _animator.SetTrigger("ShotTrigger");
        _battleManager.PunAttackOtherPlayer(this.gameObject, _characterStats.Attack, NegativeStatusEffect.None);
        _characterUIHandler.ActionQueueCall();
    }

    [PunRPC]
    private void PunDeActivateHide()
    {
        _characterStats.CombatState = CombatState.Attacking;
        _animator.SetBool("IsHiding", false);
    }

    [PunRPC]
    private void PunHideSkill()
    {
        _characterStats.StanceState = StanceState.Hiding;
        _characterStats.CombatState = CombatState.Defending;
        _animator.SetBool("IsHiding", true);
        _characterUIHandler.ActionQueueCall();

    }

    [PunRPC]
    private void PunKickSkill()
    {
        _animator.SetTrigger("KickTrigger");
        _battleManager.PunAttackOtherPlayer(this.gameObject, _characterStats.Attack, NegativeStatusEffect.Stunned);
        _characterUIHandler.ActionQueueCall();

    }
}
