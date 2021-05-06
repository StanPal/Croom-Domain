using Photon.Pun;
using UnityEngine;

public class SpawnManager : MonoBehaviourPun
{    
    [SerializeField] private Transform _player1Pos;
    [SerializeField] private Transform _player2Pos; 

    // Start is called before the first frame update
    void Start()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate("Cube", _player1Pos.position, Quaternion.identity);
        }
        else
        {
            PhotonNetwork.Instantiate("Cube", _player2Pos.position, Quaternion.identity);
        }
    }
}
