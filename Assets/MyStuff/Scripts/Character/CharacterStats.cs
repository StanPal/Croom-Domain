using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [SerializeField] private string _characterName;
    [SerializeField] private float _characterHealth = 100f;
    [SerializeField] private float _characterMana = 100f;
    [SerializeField] private float _characterMaxMana = 100f;
    [SerializeField] private float _characterAttack = 20f;
    [SerializeField] private float _MaxCharacterHealth = 100f;
   
    
    void Start()
    {
        _characterHealth = _MaxCharacterHealth;
        _characterMana = _characterMaxMana;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float Damage)
    {
        _characterHealth -= Damage;
    }
}
