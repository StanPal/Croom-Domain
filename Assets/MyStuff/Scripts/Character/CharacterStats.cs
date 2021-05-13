using Photon.Pun;
using UnityEngine;

public class CharacterStats : MonoBehaviourPunCallbacks, IPunObservable , IPunInstantiateMagicCallback
{
   [SerializeField] private GameObject model;
    private SpawnManager _spawnManager;
    
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

    public string PlayerName { get => _characterName; }
    public float MaxHealth { get => _characterMaxHealth; }
    public float CurrentHealth { get => _characterHealth; set => _characterHealth = value; }
    public float Attack { get => _characterAttack; set => _characterAttack = value; }
    public int ID { get => _characterID; }
    public CharacterUIHandler CharacterUIHandler { get => _characterUIHandler; }

    private void Awake()
    {
        _spawnManager = FindObjectOfType<SpawnManager>();
        _characterMaxHealth = _characterHealth;
        _characterUIHandler = GetComponent<CharacterUIHandler>();
    }
    

    public void TakeDamage(float damage)
    {   
        _characterHealth -= damage;
        Debug.Log(_characterName + " HP: " + _characterHealth);
        if(_characterHealth <= 0)
        {
            _spawnManager.PlayerList.Remove(this.gameObject);
            PhotonNetwork.Destroy(this.gameObject);
            Debug.Log("Destroy Player");
            PhotonNetwork.Disconnect();
        }
    }
    

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        _spawnManager.PlayerList.Add(this.gameObject);
        model.transform.position = this.transform.position;
        this.photonView.RPC("rotateModel", RpcTarget.All);

        Debug.Log("Adding Instatiated Player to List");
    }

    [PunRPC]
    private void rotateModel()
    {
        if (_spawnManager.PlayerList.Count == 1)
        {
            model.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
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
