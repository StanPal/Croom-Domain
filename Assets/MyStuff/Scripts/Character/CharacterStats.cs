using Photon.Pun;
using UnityEngine;

public class CharacterStats : MonoBehaviourPunCallbacks, IPunObservable , IPunInstantiateMagicCallback
{
    private SpawnManager _spawnManager;
    public static GameObject localPlayerInstance; 

    private float _characterMaxHealth;
    private CharacterUIHandler _characterUIHandler;
    [SerializeField] private string _characterName;
    [SerializeField] private float _characterHealth = 100;
    [SerializeField] private float _characterAttack = 10;
    [SerializeField] private float _characterSpeed = 10;
    [SerializeField] private int _characterID = 0;

    public float Speed { get => _characterSpeed; }

    public enum CharacterClass
    {
        Warrior,
        Archer,
        Mage
    }

    [SerializeField] private CharacterClass _class = CharacterClass.Warrior;

    public string PlayerName { get => _characterName; }
    public float MaxHealth { get => _characterMaxHealth; }
    public float CurrentHealth { get => _characterHealth; set => _characterHealth = value; }
    public float Attack { get => _characterAttack; set => _characterAttack = value; }
    public int ID { get => _characterID; }
    public CharacterUIHandler CharacterUIHandler { get => _characterUIHandler; }
    public CharacterClass ClassType { get => _class; } 
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
            _spawnManager.PlayerList.Remove(this.gameObject);
            PhotonNetwork.Destroy(this.gameObject);
            GameManager.Instance.LeaveRoom();
        }
    }
    
    public void TakeDamage(float damage)
    {   
        _characterHealth -= damage;
        Debug.Log(_characterName + " HP: " + _characterHealth);        
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
       // info.Sender.TagObject = this.gameObject;
       _spawnManager.PlayerList.Add(this.gameObject);  
        this.photonView.RPC("rotateModel", RpcTarget.All);
        //     Debug.Log("Adding Instatiated Player to List");
    }

    [PunRPC]
    private void rotateModel()
    {
        if (_spawnManager.PlayerList[0].gameObject == this.gameObject)
        {
            transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, -90f, 0f);

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
