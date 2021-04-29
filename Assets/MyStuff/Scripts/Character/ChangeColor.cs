using Photon.Pun;
using UnityEngine;

public class ChangeColor : MonoBehaviourPun
{
    private bool _toggleColor = false;

    // Update is called once per frame
    void Update()
    {
        //Toggling this makes only the person who first loads this gameobject be able to control it 
        //if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        //{
        //    return;
        //}
        //else
        //{
            if (Input.GetKeyDown(KeyCode.Space) || Input.touchCount > 0)
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
        //}

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
