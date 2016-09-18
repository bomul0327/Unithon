using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
public class NetworkSync : NetworkBehaviour {
    //이 스크립트에서는 서버에서 받아오는 값들을 보간해서 움직임 등을 부드럽게 만듭니다.
    //네트워크 게임에서 움직임을 부드럽게 하는 방법에는 여러가지가 있지만,
    //우린 Linear Interpolation(Lerp in Unity)를 이용합니다.
    //Unity Network는 기본적으로 Server authorized 시스템입니다.

    //매 패킷마다 sync하는 variables
    [Header("sync variables")]

    //SyncVarAttribute: 동기화하는 변수 선언 시에 사용.
    //server와 client끼리 직렬화하여 따로 패킷을 보낼 필요없이 변수만 갱신해주면 됨.
    [SyncVar(hook = "SyncPositionValues")]
    private Vector3 syncPos;
    [Space(10)]

    //lerp(선형 보간) 하는 비율
    //Latency에 따라서 lerpRate가 변경 됩니다.
    private float lerpRate;
    [SerializeField]
    private float normalLerpRate = 16;
    [SerializeField]
    private float fasterLerpRate = 27;

    private Vector3 lastPos;

    //Object가 가만히 있다고 판단되면 Client가 서버에 position의 값을 보내지 않습니다.
    [SerializeField]
    private float positionThreshold = 0.5f;
    [SerializeField]
    private float positionCloseEnough = 0.2f;

    //syncPosList.Count에 따라서 lerpRate가 변경됨
    private List<Vector3> syncPosList = new List<Vector3>();
    
	void Start () {
        lerpRate = normalLerpRate;
	}
	
	void Update () {
        LerpValues();
	}

    void FixedUpdate () {

    }

    void LerpValues () {
        LerpPosition();
    }

    void LerpPosition () {
        if (!isLocalPlayer) {
            if(syncPosList.Count > 0) {
                //Position Lerping
                transform.position = Vector3.Lerp(transform.position, syncPosList[0], Time.deltaTime * lerpRate);

                if (Vector3.Distance(transform.position, syncPosList[0]) < positionCloseEnough) {
                    syncPosList.RemoveAt(0);
                }
                if(syncPosList.Count > 10) {
                    lerpRate = fasterLerpRate;
                }
                else {
                    lerpRate = normalLerpRate;
                }
            }
        }
    }
    //Command Attribute: 호출은 client에서, but 실행은 server에서
    [Command]
    void CmdProvidePositionToServer(Vector3 pos) {
        syncPos = pos;
    }

    //Cliend Attribute: Client에서만 실행 가능함.
    //자신이 컨트롤하고 있는 오브젝트의 position을 서버에 보낸다.
    [Client]
    void TransmitPosition () {
        if(isLocalPlayer && Vector3.Distance(transform.position, lastPos) > positionThreshold) {
            CmdProvidePositionToServer(transform.position);
            lastPos = transform.position;
        }
    }

    [Client]
    void SyncPositionValues (Vector3 latestPos) {
        syncPos = latestPos;
        syncPosList.Add(syncPos);
    }
}
