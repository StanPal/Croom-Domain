using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUIHandler : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private Slider _healthbar;
    private Enemy _enemy;
    private Animator _animator;
    private bool _canAttack;
    private TurnManager _TurnManager;
    private ActionManager _actionManager;
    private BattleManager _battleManager;

    private void Awake()
    {
        GameLoader.CallOnComplete(Initialize);

    }

    private void Initialize()
    {        
        _animator = GetComponent<Animator>();
        _battleManager = FindObjectOfType<BattleManager>();
        //_spawnManager = ServiceLocator.Get<SpawnManager>();
        _actionManager = FindObjectOfType<ActionManager>();
        _TurnManager = FindObjectOfType<TurnManager>();
        _enemy = GetComponent<Enemy>();
    }


    private void Start()
    {
        _healthbar.maxValue = _enemy.MaxHealth;
    }

    private void Update()
    {
        _healthbar.value = _enemy.CurrentHealth;
    }

    public void UpdateHealthBar()
    {
        _healthbar.value = _enemy.CurrentHealth;
    }

    public void OnAttack()
    {
        int choice = Random.Range(0, 1);
        Debug.Log("Enemy Choice" + choice);
        switch (choice)
        {
            case 0:
                PlayAnim();
                break;
            case 1:
                PlayPrayerAnim();
                break;
            default:
                break;
        }

        StartCoroutine(PlayAnim());
        
        //_actionManager.AttackPlayer(this.gameObject, _enemy.ClassType);
    }

    private IEnumerator PlayAnim()
    {
        _canAttack = true;
        _animator.SetTrigger("PunchTrigger");
        yield return new WaitForSeconds(1.0f);
        if (_canAttack)
        {
            _battleManager.EnemyAttackPlayer(_enemy.Attack);
            ActionQueueCall();
            StopCoroutine(PlayAnim());
            _canAttack = false;
        }
    }

    private IEnumerator PlayPrayerAnim()
    {
        _animator.SetBool("IsPraying", true);
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        _animator.SetBool("IsPraying", false);
        _enemy.onHeal(10.0f);
    }

    public void ActionQueueCall()
    { 
        _TurnManager.ActionQueue.Dequeue();
        _TurnManager.State = BattleState.TransitionPhase;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
       if(stream.IsWriting)
        {
            stream.SendNext(_canAttack);
        }
       else
        {
            this._canAttack = (bool)stream.ReceiveNext();
        }
    }
}
