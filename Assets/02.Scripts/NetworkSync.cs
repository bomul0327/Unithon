using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
public class NetworkSync : NetworkBehaviour {
    //이 스크립트에서는 서버에서 받아오는 값들을 보간해서 플레이어의 움직임, 회전을 부드럽게 만듭니다.
    //네트워크 게임에서 움직임을 부드럽게 하는 방법에는 여러가지가 있지만,
    //우린 Linear Interpolation(Lerp in Unity)를 이용합니다.
    //Unity Network는 기본적으로 Server authorized 시스템입니다.

    //매 패킷마다 sync하는 variables
    [Header("sync variables")]

    //SyncVarAttribute: 동기화하는 변수 선언 시에 사용.
    //server와 client끼리 직렬화하여 따로 패킷을 보낼 필요없이 변수만 갱신해주면 됨.
    [SyncVar(hook = "SyncPositionValues")]
    private Vector3 syncPos;

    //rotation은 캐릭터의 y축 값만 보간해줍니다. 매 패킷마다 Quaternion을 보내지 않기 위해서
    [SyncVar(hook = "SyncRotationValues")]
    private float syncRot;
    [Space(10)]

    //lerp(선형 보간) 하는 비율
    //Latency에 따라서 lerpRate가 변경 됩니다.
    private float lerpRate;
    [SerializeField]
    private float normalLerpRate = 16;
    [SerializeField]
    private float fasterLerpRate = 27;

    private Vector3 lastPos;
    private float lastRot;

    //Object가 가만히 있다고 판단되면 Client가 서버에 position의 값을 보내지 않습니다.
    [SerializeField]
    private float positionThreshold = 0.5f;
    [SerializeField]
    private float positionCloseEnough = 0.2f;
    [SerializeField]
    private float rotationThreshold = 0.3f;
    [SerializeField]
    private float rotationCloseEnough = 0.4f;

    //syncPosList.Count에 따라서 lerpRate가 변경됨
    private List<Vector3> syncPosList = new List<Vector3>();
    private List<float> syncRotList = new List<float>();

	void Start () {
        lerpRate = normalLerpRate;
	}
	
	void Update () {
        LerpValues();
	}

    void FixedUpdate () {
        TransmitValues();
    }

    void LerpValues () {
        LerpPosition();
        LerpRotation();
    }

    void TransmitValues () {
        TransmitPosition();
        TransmitRotation();
    }
    
    //Position Lerping
    void LerpPosition () {
        if (!isLocalPlayer) {
            if(syncPosList.Count > 0) {
                transform.position = Vector3.Lerp(transform.position, syncPosList[0], lerpRate * Time.deltaTime);

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


    //Rotation Lerping
    //Rotation Lerping은 Latency에 따라서 lerpRate가 바뀌지 않고, normalLerpRate로 고정해놨습니다.
    //문제가 되면 그 때 고칩시다.
    void LerpRotation () {
        if (!isLocalPlayer) {
            if(syncRotList.Count > 0) {
                Quaternion newRotation = Quaternion.Euler(0, syncRotList[0], 0);
                transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, normalLerpRate * Time.deltaTime);

                if(Mathf.Abs(transform.localEulerAngles.y - syncRotList[0]) < rotationCloseEnough) {
                    syncRotList.RemoveAt(0);
                }
            }
        }
    }

    [Command]
    void CmdProvideRotationToServer(float rot) {
        syncRot = rot;
    }

    [Client]
    void TransmitRotation () {
        if(isLocalPlayer && Mathf.Abs(transform.localEulerAngles.y - lastRot) > rotationThreshold) {
            CmdProvideRotationToServer(transform.localEulerAngles.y);
            lastRot = transform.localEulerAngles.y;
        }
    }

    [Client]
    void SyncRotationValues (float latestRot) {
        syncRot = latestRot;
        syncRotList.Add(syncRot);
    }
}
