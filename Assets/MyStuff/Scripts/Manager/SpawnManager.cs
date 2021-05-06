using Photon.Pun;
using UnityEngine;

public class SpawnManager : MonoBehaviourPun
{
    [SerializeField] private GameObject _cube; 

    // Start is called before the first frame update
    void Start()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate("Cube", new Vector3(0, 2.5f, 0), Quaternion.identity);
        }
    }
}
