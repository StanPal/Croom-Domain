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
    private bool _isSwinging;
    private TurnManager _TurnManager;
    private ActionManager _actionManager;
    private BattleManager _battleManager;
    private GameManager _gameManager;
    private Vector3 _startPos;
    private Vector3 _Player1Pos;
    private Vector3 _Player2Pos;

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

    public void OnAttack()
    {
        if (_canAttack)
        {

            int choice = Random.Range(0, 3);
            Debug.Log("Enemy Choice " + choice);  
            switch (choice)
            {
                case 0:
                    _isSwinging = true;
                    this.photonView.RPC("Swing", RpcTarget.All);
                    break;
                case 1:
                    this.photonView.RPC("Heal", RpcTarget.All);
                    break;
                case 2:
                    this.photonView.RPC("Cast", RpcTarget.All);
                    break;
                default:
                    break;
            }
            this.photonView.RPC("ActionQueueCall", RpcTarget.All);
        }
        //_actionManager.AttackPlayer(this.gameObject, _enemy.ClassType);
    }

    [PunRPC]
    private void ActionMove()
    {

    }

    [PunRPC] 
    private void Swing()
    {
        _animator.SetTrigger("PunchTrigger");
        _battleManager.EnemyAttackPlayer(_enemy.Attack);
        //StartCoroutine(PlayAnim());
        // StartCoroutine(SmoothLerp(3f, _startPos, _Player1Pos, new Vector3(-1f, 0f, 0f)));
    }

    [PunRPC]
    private void Heal()
    {
        _animator.SetTrigger("HealTrigger");
        _enemy.onHeal(10.0f);
        //  StartCoroutine(PlayPrayerAnim());
    }

    [PunRPC]
    private void Cast()
    {
        _animator.SetTrigger("CastTrigger");
        _battleManager.TargetAllPlayer(_enemy.Attack);
    }

    private IEnumerator PlayAnim()
    {        
        _animator.SetBool("IsPunching", true);
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        _animator.SetBool("IsPunching", false);
        _battleManager.EnemyAttackPlayer(_enemy.Attack);
    }

    private IEnumerator PlayPrayerAnim()
    {
        _animator.SetBool("IsPraying", true);
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        _animator.SetBool("IsPraying", false);
        _enemy.onHeal(10.0f);
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
            _battleManager.EnemyAttackPlayer(_enemy.Attack);
            transform.position = _startPos;
        }
    }    

    [PunRPC]
    public void ActionQueueCall()
    {
        _canAttack = false;
        _TurnManager.ActionQueue.Dequeue();
        _TurnManager.State = BattleState.TransitionPhase;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_canAttack);
        }
        else
        {
            this._canAttack = (bool)stream.ReceiveNext();
        }
    }
}
