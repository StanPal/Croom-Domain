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
        _animator = GetComponent<Animator>();
        _characterStats = GetComponent<CharacterStats>();
        _characterUIHandler = GetComponent<CharacterUIHandler>();
    }

    private void Update()
    {
 
    }

    

    public void OnShield()
    {
        this.photonView.RPC("PunShieldSkill", RpcTarget.All);
    }

    [PunRPC]
    private void PunDeActivateShield()
    {
        _characterStats.Shield = false;
        _animator.SetBool("IsShielding", false);
    }

    [PunRPC]
    private void PunShieldSkill()
    {
        _characterUIHandler.ResetActionButtons();
        _characterStats.Shield = true;
        _animator.SetBool("IsShielding", true);
        _characterUIHandler.ActionQueueCall();
    }
}
