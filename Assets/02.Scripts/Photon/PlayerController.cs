using UnityEngine;
using System.Collections;

public class PlayerController : Photon.PunBehaviour {
    /// <summary>
    /// 캐릭터 움직임 & 애니메이션 처리하는 스크립트
    /// </summary> 

    public static GameObject LocalPlayerInstance;

    public float moveSpeed = 3f;
    public float rotSpeed = 150f;

    private Animator anim;
    private PhotonView photonView;
    private CameraCtrl cameraCtrl;
	// Use this for initialization
	void Awake () {
        anim = GetComponent<Animator>();
        photonView = GetComponent<PhotonView>();
        cameraCtrl = Camera.main.GetComponent<CameraCtrl>();
	}

    void Start () {
        if(photonView.isMine) {
            cameraCtrl.enabled = true;
            cameraCtrl.Target = transform;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (!photonView.isMine) return;
        MoveCharacter();
	}

    void MoveCharacter () {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        transform.Rotate(0, h * 150f * Time.deltaTime, 0);
        transform.Translate(0, 0, v * 3f * Time.deltaTime);

        anim.SetFloat("moveSpeed", v * v);
        
    }
}
