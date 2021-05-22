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
    [SerializeField] private Button _guardButton;
    
    private void Awake()
    {
        GameLoader.CallOnComplete(Initialize);
    }

    private void Initialize()
    {
        _animator = GetComponent<Animator>();
        _characterStats = GetComponent<CharacterStats>();
        _characterUIHandler = GetComponent<CharacterUIHandler>();
    }

    public void OnDeactivateGuard()
    {
        this.photonView.RPC("PunDeActivateShield", RpcTarget.All);
    }

    public void OnGuard()
    {
        this.photonView.RPC("PunShieldSkill", RpcTarget.All);
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
        _characterStats.Shield = true;
        _animator.SetBool("IsShielding", true);
        _characterUIHandler.ActionQueueCall();
    }
}
