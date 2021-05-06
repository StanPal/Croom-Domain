using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUIHandler : MonoBehaviour
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
        _characterStats.CurrentHealth -= 10f;
        _healthbar.value = _characterStats.CurrentHealth;
    }
}
