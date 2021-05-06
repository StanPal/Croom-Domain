using Photon.Pun;
using UnityEngine;

public class CharacterStats : MonoBehaviour, IPunInstantiateMagicCallback
{
    private SpawnManager _spawnManager;

    private int _characterMaxHealth;
    [SerializeField] private string _characterName;
    [SerializeField] private int _characterHealth = 100;
    [SerializeField] private int _characterAttack = 10;
    [SerializeField] private int _characterID = 0;

    public int MaxHealth { get => _characterMaxHealth; }
    public int CurrentHealth { get => _characterHealth; set => _characterHealth = value; }
    public int Attack { get => _characterAttack; set => _characterAttack = value; }
    public int ID { get => _characterID; }


    private void Awake()
    {
        _spawnManager = FindObjectOfType<SpawnManager>();
    }

    public void Start()
    {
        _characterMaxHealth = _characterHealth;
    }

    public void TakeDamage(int damage)
    {
        _characterHealth -= damage;
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        _spawnManager.PlayerList.Add(this.gameObject);
        Debug.Log("Adding Instatiated Player to List");
    }
}
