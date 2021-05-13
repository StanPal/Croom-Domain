using Photon.Pun;
using UnityEngine;

public class CharacterStats : MonoBehaviourPunCallbacks, IPunObservable , IPunInstantiateMagicCallback
{
    private SpawnManager _spawnManager;

    private float _characterMaxHealth;
    [SerializeField] private string _characterName;
    [SerializeField] private float _characterHealth = 100;
    [SerializeField] private float _characterAttack = 10;
    [SerializeField] private float _characterSpeed;
    [SerializeField] private int _characterID = 0;

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


    private void Awake()
    {
        _spawnManager = FindObjectOfType<SpawnManager>();
        _characterMaxHealth = _characterHealth;    
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
        Debug.Log("Adding Instatiated Player to List");
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
