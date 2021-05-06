using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class CharacterUIHandler : MonoBehaviourPun
{
    public event System.Action InvokeOnHit;
    private Slider _healthbar;
    private CharacterStats _characterStats;
    public Slider RecieverSlider { get => _healthbar; }

    private void Awake()
    {
        _healthbar = GetComponentInChildren<Slider>();
        _characterStats = GetComponent<CharacterStats>();
    }

    void Start()
    {
        _healthbar.maxValue = _characterStats.MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnHit()
    {
        
        this.photonView.RPC("TakeDamage", RpcTarget.All, 10f);
    }

    [PunRPC]
    private void TakeDamage(float damage)
    {
        _characterStats.CurrentHealth -= damage;
        _healthbar.value = _characterStats.CurrentHealth;

    }
}
