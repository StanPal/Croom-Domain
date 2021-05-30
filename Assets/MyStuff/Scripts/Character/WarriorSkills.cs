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
    private GameManager _gameManager;
    private int _maxCooldown;
    private bool _finishedWalking;
    private bool _isSwinging;
    private Vector3 _startPos;
    private Vector3 _enemyPos;
    private Vector3 _offSetPos = new Vector3 ( 5f, 0f, 0f );
    [SerializeField] private float _timeOffset = 0.7f;
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
        _gameManager = FindObjectOfType<GameManager>();
        _maxCooldown = _skillCooldown;
        _enemyPos = _gameManager.P3Pos.position;
        _startPos = transform.position;
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
        PunDeActivateShield();
        _animator.SetBool("IsWalking", true);
         PunMoveToTarget();
        _isSwinging = true;
    }

    [PunRPC]
    public void PunSwing()
    {
        _isSwinging = false;
        _animator.SetTrigger("SlashTrigger");
        _battleManager.PunAttackOtherPlayer(this.gameObject, _characterStats.Attack, NegativeStatusEffect.None);
        StartCoroutine(PlayAnim());
        //PunMoveBackToPos();
        _skillCooldown--;
        CheckCoolDownFinished();
        _characterUIHandler.ActionQueueCall();
    }

    private IEnumerator PlayAnim()
    {
        Debug.Log(_animator.GetCurrentAnimatorStateInfo(0).length);
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length + _timeOffset);
        _animator.SetBool("IsWalking", true);
        PunMoveBackToPos();
    }

    [PunRPC] void PunMoveBackToPos()
    {
        StartCoroutine(SmoothLerp(3f, transform.position, _startPos, Vector3.zero));
        _finishedWalking = true;
    }

    [PunRPC]
    public void PunMoveToTarget()
    {
        StartCoroutine(SmoothLerp(3f, _startPos, _enemyPos, _offSetPos));
    }

    private IEnumerator SmoothLerp(float time, Vector3 starting, Vector3 Target, Vector3 offset)
    {
        Vector3 startingPos = starting;
        Vector3 finalPos = Target - offset;

        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            transform.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
  
        if(_isSwinging)
        {
            _animator.SetBool("IsWalking", false);
            PunSwing();
        }
        if (_finishedWalking)
        {
            _animator.SetBool("IsWalking", false);
        }
    }


    [PunRPC]
    private void PunDeActivateShield()
    {
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
        _animator.SetBool("IsShielding", true);
        _characterUIHandler.ActionQueueCall();
    }

    [PunRPC]
    private void PunSkyAttackSkill()
    {
        PunDeActivateShield();
        _skillCooldown = _maxCooldown;
        _animator.SetTrigger("JumpAttackTrigger");
        _skyAttackButton.interactable = false;
        _battleManager.PunAttackOtherPlayer(this.gameObject, _characterStats.Attack * 1.5f, NegativeStatusEffect.None);
        _characterUIHandler.ActionQueueCall();
    }
}