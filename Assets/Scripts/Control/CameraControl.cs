using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

    private GameObject player = null;
    private Vector3 position_offset = Vector3.zero;

    void Start() {

        //player 변수에 Player 오브젝트 할당
        this.player = GameObject.FindGameObjectWithTag("Player");
        //카메라 위치와 플레이어 위치의 차이를 저장
        this.position_offset = this.transform.position - this.player.transform.position;
    }


    void Update() {

        //카메라 현재 위치
        Vector3 new_position = this.transform.position;
        //플레이어 x좌표 + 차이값을 new_position의 x에 대입
        new_position.x = this.player.transform.position.x + this.position_offset.x;
        //카메라 위치 갱신
        this.transform.position = new_position;
    }
}
