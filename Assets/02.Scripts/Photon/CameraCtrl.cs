using UnityEngine;
using System.Collections;

public class CameraCtrl : MonoBehaviour {
    /// <summary>
    /// 카메라 움직임 및 회전 관련 스크립트
    /// </summary>
    /// 
    public Transform Target; //따라다닐 오브젝트의 Transform
    public float Dist = 10.0f; //카메라와 오브젝트와의 거리
    public float Height = 5.0f; //카메라의 높이
    public float DampRotate = 5.0f; //부드러운 회전을 도와주는 변수

    private Transform Tr;

    void Start () {
        Tr = GetComponent<Transform>();
    }

    void LateUpdate () {
        if (Target == null) return;

        //카메라 Y축을 타겟의 Y축 회전각도로 부드럽게 회전하는 연산
        float CurrentYAngle = Mathf.LerpAngle
        (Tr.eulerAngles.y, Target.eulerAngles.y, DampRotate * Time.deltaTime);

        Quaternion Rotation = Quaternion.Euler(0, CurrentYAngle, 0);

        //카메라의 위치를 Target이 회전한 각도만큼 회전한 이후로
        //Dist만큼 뒤쪽으로 배치하고 Height만큼 위로 올린다.
        Tr.position = Target.position - (Rotation * Vector3.forward * Dist)
         + (Vector3.up * Height);

        //카메라가 Target을 바라보게 설정한다.
        Tr.LookAt(Target);
    }
}
