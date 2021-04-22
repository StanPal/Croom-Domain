using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BattleUI : MonoBehaviour
{
    [SerializeField] private Button _attackButton;
    [SerializeField] private BattleManager _battleManager;

    private void Awake()
    {
        _battleManager = FindObjectOfType<BattleManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
