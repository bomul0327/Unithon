using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerNameInputUI : MonoBehaviour {
    //플레이어 이름을 적는 input field  관련 스크립트

    //PlayerName을 저장해두는 곳
    static string playerNamePrefKey = "PlayerName";

	//전에 적은 이름을 불러옵니다.
	void Start () {
        string defaultName = "";
        InputField inputField = this.GetComponent<InputField>();
        if(inputField != null) {
            if (PlayerPrefs.HasKey(playerNamePrefKey)) {
                defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                inputField.text = defaultName;
            }
        }
        PhotonNetwork.playerName = defaultName;
	}
	
    //PhotonNetwork에 플레이어 이름을 지정합니다.
    public void SetPlayerName(string value) {
        PhotonNetwork.playerName = value + " ";
        PlayerPrefs.SetString(playerNamePrefKey, value);
    }
}
