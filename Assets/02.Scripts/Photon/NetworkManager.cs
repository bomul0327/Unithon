using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class NetworkManager : Photon.PunBehaviour {
    //플레이어가 접속한 후, PlayScene에서 플레이어 관리하는 manager 스크립트입니다.
    public static NetworkManager Instance;

    public GameObject playerPrefab;

    void Awake () {
        Instance = this;
    }

    void Start () {
        if (PlayerController.LocalPlayerInstance == null) {
            PhotonNetwork.Instantiate(this.playerPrefab.name, Vector3.zero, Quaternion.identity, 0);
        }
    }
    //방에서 떠났을 때, 초기화면으로 돌아갑니다.
    public override void OnLeftRoom () {
        SceneManager.LoadScene(0);
    }

    //플레이하고 있는 방에서 나갑니다.
    public void LeaveRoom () {
        PhotonNetwork.LeaveRoom();
    }

    //플레이 씬으로 이동합니다.
    void LoadArena () {
        if (!PhotonNetwork.isMasterClient) {
            Debug.Log("Not Master Client");
        }
        PhotonNetwork.LoadLevel(1);
    }

    //마스터 클라이언트(방장)가 연결되었을 때, 플레이 씬을 불러옵니다.
    public override void OnPhotonPlayerConnected (PhotonPlayer newPlayer) {
        Debug.Log(newPlayer.name + " Connected");
        if (PhotonNetwork.isMasterClient) {
            LoadArena();
        }
    }

    //마스터 클라이언트가 연결 해제 되었을 때, 플레이 씬을 불러옵니다.
    public override void OnPhotonPlayerDisconnected (PhotonPlayer otherPlayer) {
        if (PhotonNetwork.isMasterClient) {
            LoadArena();
        }
    }

    void OnLevelWasLoaded(int level) {
        transform.position = Vector3.zero;
    }
}
