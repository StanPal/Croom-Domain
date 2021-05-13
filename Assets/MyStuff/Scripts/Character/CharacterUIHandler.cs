using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class CharacterUIHandler : MonoBehaviourPun

{
    public event System.Action InvokeOnHit;
    [SerializeField] private Text playerName;
    [SerializeField] private Slider _healthbar;
    private CharacterStats _characterStats;
    public Slider RecieverSlider { get => _healthbar; }
    private BattleUI _battleUI;
    private bool onHit; 

    private void Awake()
    {
        _healthbar = GetComponentInChildren<Slider>();
        _characterStats = GetComponent<CharacterStats>();
        _battleUI = FindObjectOfType<BattleUI>();
    }

    void Start()
    {
        _healthbar.maxValue = _characterStats.MaxHealth;
        playerName.text = _characterStats.PlayerName;
    }

    // Update is called once per frame
    void Update()
    {
        _healthbar.value = _characterStats.CurrentHealth;
        //if(onHit)
        //{
        //    this.photonView.RPC("UpdateHealthBarPun", RpcTarget.All);
        //}
    }

    public void OnHit()
    {        
        this.photonView.RPC("TakeDamage", RpcTarget.All);
    }

    [PunRPC]
    private void TakeDamage()
    {
        _battleUI.PunAttackOtherPlayer(this.gameObject);
        //_characterStats.CurrentHealth -= damage;
        //_healthbar.value = _characterStats.CurrentHealth;

    }

    public void UpdateHealthBar()
    {
        //onHit = true;
        this.photonView.RPC("UpdateHealthBarPun", RpcTarget.All);
    }

    [PunRPC]
    public void UpdateHealthBarPun()
    {
        _healthbar.value = _characterStats.CurrentHealth;
        onHit = false;
    }
}
