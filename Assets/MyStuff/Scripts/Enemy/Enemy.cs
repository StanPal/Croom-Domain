using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static GameObject localPlayerInstance;
    private SpawnManager _spawnManager;

    [SerializeField] private string _enemyName;
    [SerializeField] private float _enemyHealth;
    [SerializeField] private float _enemyAttack;
    [SerializeField] private float _enemySpeed;
    [SerializeField] private int _enemyLevel;

    private float _enemyMaxHealth;

    public float Speed { get => _enemySpeed; }
    public string PlayerName { get => _enemyName; }
    public float MaxHealth { get => _enemyMaxHealth; }
    public float CurrentHealth { get => _enemyHealth; set => _enemyHealth = value; }
    public float Attack { get => _enemyAttack; set => _enemyAttack = value; }
}
