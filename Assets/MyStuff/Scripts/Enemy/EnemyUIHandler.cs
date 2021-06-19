using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUIHandler : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private Slider _healthbar;
    public GameObject projectile;
    public GameObject healEffect;
    private Enemy _enemy;
    private Animator _animator;
    private bool _canAttack;
    private bool _isSwinging;
    private TurnManager _TurnManager;
    private ActionManager _actionManager;
    private BattleManager _battleManager;
    private GameManager _gameManager;
    private Vector3 _startPos;
    private Vector3 _Player1Pos;
    private Vector3 _Player2Pos;
    private int choice = 0;
    public bool CanAttack { set => _canAttack = value; }

    private void Awake()
    {
        GameLoader.CallOnComplete(Initialize);
    }

    private void Initialize()
    {
        _animator = GetComponent<Animator>();
        _battleManager = FindObjectOfType<BattleManager>();
        _gameManager = FindObjectOfType<GameManager>();
        _actionManager = FindObjectOfType<ActionManager>();
        _TurnManager = FindObjectOfType<TurnManager>();
        _enemy = GetComponent<Enemy>();
        _startPos = transform.position;        
    }


    private void Start()
    {
        _Player1Pos = _gameManager.P1Pos.position;
        _Player2Pos = _gameManager.P2Pos.position;
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

    public void RandomRoll()
    {
        choice = Random.Range(0, 3);
    }

    public void OnAttack()
    {
        _canAttack = true;
        if(choice > 2)
        {
            choice = 0;
        }
        if (_enemy.StunTimer > 0)
        {
            _enemy.StunTimer = 0;
            _canAttack = false;
            ActionQueueCall();
        }
        else
        {
            if (_canAttack)
            {
                _canAttack = false;
                Debug.Log("Enemy Choice " + choice);
                if (choice == 0)
                {
                    _isSwinging = true;
                    CallSwing();
                }
                else if (choice == 1)
                {
                    _enemy.onHeal(5.0f);
                    CallHeal();
                }
                else if (choice == 2)
                {
                    CallCast();
                }
                ActionQueueCall();
            }
        }
        //_actionManager.AttackPlayer(this.gameObject, _enemy.ClassType);
    }

    private void CallHeal()
    {
        this.photonView.RPC("Heal", RpcTarget.All);
    }

    private void CallCast()
    {
        this.photonView.RPC("Cast", RpcTarget.All);
    }

    private void CallSwing()
    {
        this.photonView.RPC("Swing", RpcTarget.All);
    }

    [PunRPC]
    private void ActionMove()
    {

    }

    [PunRPC] 
    private void Swing()
    {
        //_animator.SetTrigger("PunchTrigger");

        StartCoroutine(PlayAnim());
        // StartCoroutine(SmoothLerp(3f, _startPos, _Player1Pos, new Vector3(-1f, 0f, 0f)));
    }

    [PunRPC]
    private void Heal()
    {
       // _animator.SetTrigger("HealTrigger");

          StartCoroutine(PlayPrayerAnim());
    }

    [PunRPC]
    private void Cast()
    {
        StartCoroutine(CastAnim());        
        //_animator.SetTrigger("CastTrigger");
    }

    private IEnumerator CastAnim()
    {
        _animator.SetBool("IsCasting", true);
        yield return new WaitForSeconds(1.15f);
        projectile.SetActive(true);
        yield return new WaitForSeconds(3f);
        projectile.SetActive(false);
        yield return new WaitForSeconds(3f);
        _animator.SetBool("IsCasting", false);
        _battleManager.TargetAllPlayer(_enemy.Attack);
    }

    private IEnumerator PlayAnim()
    {
            int choice = Random.Range(0, 2);
            Debug.Log("Enemy chose to hit " + choice);
            _animator.SetBool("IsPunching", true);
            yield return new WaitForSeconds(1.1f);
            _animator.SetBool("IsPunching", false);
            _battleManager.PunAttackPlayer(_enemy.Attack, choice);   
  
    }

    private IEnumerator PlayPrayerAnim()
    {
        _animator.SetBool("IsPraying", true);
        healEffect.SetActive(true);
        yield return new WaitForSeconds(1.1f);
        healEffect.SetActive(false);
      
        _animator.SetBool("IsPraying", false);
    }

    private IEnumerator SmoothLerp(float time, Vector3 starting, Vector3 Target, Vector3 offset)
    {
        Vector3 startingPos = starting;
        Vector3 finalPos = Target - offset;

        float elapsedTime = 0;
        _animator.SetBool("IsWalking", true);
        while (elapsedTime < time)
        {
            transform.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (_isSwinging)
        {
            _animator.SetBool("IsWalking", false);
            _isSwinging = false;
           // _battleManager.EnemyAttackPlayer(_enemy.Attack);
            transform.position = _startPos;
        }
    }    

    public void ActionQueueCall()
    {
        _canAttack = false;
        Debug.Log("Current Queue Count: " + _TurnManager.ActionQueue.Count);
        _TurnManager.ActionQueue.Dequeue();
        _TurnManager.TransitionPhase();
        choice++;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_canAttack);
            stream.SendNext(choice);
        }
        else
        {
            this._canAttack = (bool)stream.ReceiveNext();
            this.choice = (int)stream.ReceiveNext();
        }
    }
}
