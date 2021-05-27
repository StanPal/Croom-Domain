using Photon.Pun;
using UnityEngine;

public class CharacterStats : MonoBehaviourPunCallbacks, IPunObservable , IPunInstantiateMagicCallback
{
    public static GameObject localPlayerInstance; 
    private SpawnManager _spawnManager;
    private CharacterUIHandler _characterUIHandler;
    

    [SerializeField] private string _characterName;
    [SerializeField] private float _characterHealth = 100;
    [SerializeField] private float _characterAttack = 10;
    [SerializeField] private float _characterSpeed = 10;
    [SerializeField] private int _characterID = 0;
    [SerializeField] private CharacterClass _class = CharacterClass.Warrior;
    [SerializeField] private CombatState _combatState = CombatState.None;
    [SerializeField] private StanceState _stanceStace = StanceState.None;

    private float _characterMaxHealth;
    private bool _isShielding;
    private int _stunTimer = 1; 

    public float Speed { get => _characterSpeed; }
    public string PlayerName { get => _characterName; }
    public float MaxHealth { get => _characterMaxHealth; }
    public float CurrentHealth { get => _characterHealth; set => _characterHealth = value; }
    public float Attack { get => _characterAttack; set => _characterAttack = value; }
    public int ID { get => _characterID; }
    public CharacterUIHandler CharacterUIHandler { get => _characterUIHandler; }
    public CharacterClass ClassType { get => _class; } 
    public CombatState CombatState { get => _combatState; set => _combatState = value; }
    public StanceState StanceState { get => _stanceStace; set => _stanceStace = value; }
    public bool Shield { get => _isShielding; set => _isShielding = value; }
    public int StunTimer { get => _stunTimer; set => _stunTimer = value; }
    private void Awake()
    {
        _spawnManager = FindObjectOfType<SpawnManager>();
        _characterMaxHealth = _characterHealth;
        if(photonView.IsMine)
        {
            CharacterStats.localPlayerInstance = this.gameObject;
        }
            DontDestroyOnLoad(this.gameObject);
        
    }

    private void Update()
    {
       if(CurrentHealth <= 0f)
        {
            this.gameObject.SetActive(false);
            //_spawnManager.PlayerList.Remove(this.gameObject);
            //PhotonNetwork.Destroy(this.gameObject);
            //GameManager.Instance.LeaveRoom();
        }
    }
    
    public void TakeDamage(float damage)
    {   
        if(_stanceStace == StanceState.Shielding)
        {
            _combatState = CombatState.None;
            _stanceStace = StanceState.None;
          _characterHealth -= (damage * 0.5f);
        }
        else if (_stanceStace == StanceState.Hiding)
        {
            int hitChance = 1;
            hitChance *= Random.Range(0, 100);
            if(hitChance > 50)
            {
                _characterAttack -= damage;
            }
            Debug.Log(hitChance);
            _combatState = CombatState.None;
            _stanceStace = StanceState.None;
        }
        else
        {
            _characterHealth -= damage;
        }
        Debug.Log(_characterName + " HP: " + _characterHealth);
    }

    public void OnStatusEffect(NegativeStatusEffect negativeStatus)
    {
        switch (negativeStatus)
        {
            case NegativeStatusEffect.None:
                break;
            case NegativeStatusEffect.Stunned:
                _stunTimer = 1;
                break;
            case NegativeStatusEffect.Burning:
                break;
            default:
                break;
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
       _spawnManager.PlayerList.Add(this.gameObject);  
        this.photonView.RPC("rotateModel", RpcTarget.All);
    }

    [PunRPC]
    private void rotateModel()
    {
        if(ClassType == CharacterClass.Warrior)
        {
            transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, -45f, 0f);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //Sync Health & name
        if(stream.IsWriting)
        {
            stream.SendNext(CurrentHealth);
            stream.SendNext(_characterName);
        }
        else
        {
            //We are reading input to our health and write it back to our client and synced across the network
            this._characterHealth = (float)stream.ReceiveNext();
            this._characterName = (string)stream.ReceiveNext();
        }        
    }
}
