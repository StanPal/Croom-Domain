using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviourPun
{
    private bool _toggleColor = false;

    // Start is called before the first frame update
    //void Start()
    //{
    //    CameraWork _cameraWork = this.gameObject.GetComponent<CameraWork>();


    //    if (_cameraWork != null)
    //    {
    //        if (photonView.IsMine)
    //        {
    //            _cameraWork.OnStartFollowing();
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
    //    }
    //}

    private void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if(_toggleColor)
                {
                    _toggleColor = false; 
                }
                else
                {
                    _toggleColor = true;
                }
                this.photonView.RPC("ToggleColor", RpcTarget.All, _toggleColor);
            } 
        }

    }

    [PunRPC]
    private void ToggleColor(bool Toggle)
    {
        if (Toggle)
        {
            this.GetComponent<MeshRenderer>().material.color = Color.red;
        }
        else
        {
            this.GetComponent<MeshRenderer>().material.color = Color.blue;
        }
    }
}
