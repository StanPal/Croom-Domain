using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

/// Player name input field. Let the user input his name, will appear in the battle UI.
[RequireComponent(typeof(InputField))]
public class PlayerNameInputField : MonoBehaviour
{
    // Store the PlayerPref Key to avoid typos
    const string playerNamePrefKey = "PlayerName";

    void Start()
    {
        string defaultName = string.Empty;
        InputField _inputField = this.GetComponent<InputField>();
        if (_inputField != null)
        {
            //PlayerPrefs stores data of players in between game sessions
            if (PlayerPrefs.HasKey(playerNamePrefKey))
            {
                defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                _inputField.text = defaultName;
            }
        }
        //This is main point of this script, setting up the name of the player over the network. 
        //The script uses this in two places, once during Start() after having check if the name was stored in the PlayerPrefs, and inside the public method
        
        PhotonNetwork.NickName = defaultName;
    }

    public void SetPlayerName(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            Debug.LogError("Player Name is null or empty");
            return;
        }
        PhotonNetwork.NickName = value;

        PlayerPrefs.SetString(playerNamePrefKey, value);
    }
};
