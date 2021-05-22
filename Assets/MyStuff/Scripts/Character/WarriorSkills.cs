using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarriorSkills : MonoBehaviourPun
{
    private Animator _animator;
    private CharacterUIHandler _characterUIHandler;
    private CharacterStats _characterStats;
    private BattleManager _battleManager;
    private int _maxCooldown;
    [SerializeField] private int _skillCooldown = 2;
    [SerializeField] private Button _guardButton;
    [SerializeField] private Button _skyAttackButton;
    
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
        _maxCooldown = _skillCooldown;
    }

    public void OnDeactivateGuard()
    {
        this.photonView.RPC("PunDeActivateShield", RpcTarget.All);
    }

    public void OnGuard()
    {
        this.photonView.RPC("PunShieldSkill", RpcTarget.All);
    }

    public void OnNormalAttack()
    {
        this.photonView.RPC("PunNormalAttack", RpcTarget.All);
    }

    public void OnSkyAttack()
    {
        this.photonView.RPC("PunSkyAttackSkill", RpcTarget.All);
    }

    private void CheckCoolDownFinished()
    {
        if(_skillCooldown == 0 && _characterStats.CombatState != CombatState.Defending)
        {
            _skyAttackButton.interactable = true;
        }
    }

    [PunRPC]
    private void PunNormalAttack()
    {
        _animator.SetTrigger("SlashTrigger");
        _battleManager.PunAttackOtherPlayer(this.gameObject, _characterStats.Attack);
        _skillCooldown--;
        CheckCoolDownFinished();
        _characterUIHandler.ActionQueueCall();
    }

    [PunRPC]
    private void PunDeActivateShield()
    {
        _characterStats.Shield = false;
        _characterStats.CombatState = CombatState.Attacking;
        _animator.SetBool("IsShielding", false);
    }

    [PunRPC]
    private void PunShieldSkill()
    {
        _characterUIHandler.ResetActionButtons();
        _characterStats.CombatState = CombatState.Defending;
        _skillCooldown--;
        CheckCoolDownFinished();
        _characterStats.Shield = true;
        _animator.SetBool("IsShielding", true);
        _characterUIHandler.ActionQueueCall();
    }

    [PunRPC]
    private void PunSkyAttackSkill()
    {
        _skillCooldown = _maxCooldown;
        _animator.SetTrigger("JumpAttackTrigger");
        _skyAttackButton.interactable = false;
        _battleManager.PunAttackOtherPlayer(this.gameObject, _characterStats.Attack * 1.5f);
        _characterUIHandler.ActionQueueCall();
    }
}
