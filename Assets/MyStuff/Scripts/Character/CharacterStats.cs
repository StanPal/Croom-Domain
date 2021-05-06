using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    private float _characterMaxHealth;
    [SerializeField] private string _characterName;
    [SerializeField] private float _characterHealth = 100f;
    [SerializeField] private float _characterDamage = 10f;


    public void Start()
    {
        _characterMaxHealth = _characterHealth;
    }

    public void TakeDamage(float damage)
    {
        _characterHealth -= damage;
    }
}
