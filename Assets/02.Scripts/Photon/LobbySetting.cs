using UnityEngine;
using System.Collections;

public class LobbySetting : Photon.PunBehaviour {
    //Photon Network Engine을 이용해서 제작합니다.
    //이 스크립트는 로비를 통해서 서버에 접속해서, 방을 만들고, 다른 사람의 방에 들어가는 것이 포함되어있습니다.
    public GameObject progressLabel;
    public GameObject controlPanel;

    void Awake () {
        PhotonNetwork.autoJoinLobby = false;
        PhotonNetwork.automaticallySyncScene = true;
    }

    void Start () {
        controlPanel.SetActive(true);
        progressLabel.SetActive(false);
    }
	
    public void ConnectToServer () {
        if (PhotonNetwork.connected) {
            PhotonNetwork.JoinRandomRoom();
        }
        else {
            PhotonNetwork.ConnectUsingSettings("Unithon 1.0.0");
        }
        progressLabel.SetActive(true);
        controlPanel.SetActive(false);
    }

    void OnGUI () {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }

    public override void OnConnectedToMaster () {
        Debug.Log("Region:" + PhotonNetwork.networkingPeer.CloudRegion);
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnPhotonRandomJoinFailed (object[] codeAndMsg) {
        Debug.Log("PhotonRandomJoinFailed");
        PhotonNetwork.CreateRoom(null);
    }

    public override void OnJoinedRoom () {
        PhotonNetwork.LoadLevel(1);
    }

    public override void OnDisconnectedFromPhoton () {
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
    }
}
