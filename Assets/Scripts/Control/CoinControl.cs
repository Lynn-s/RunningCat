using UnityEngine;
using System.Collections;

public class CoinControl : MonoBehaviour {

    public static float COLLISION_SIZE = 1.0f; //지름

    public ScoreControl score_control = null;
    public MapCreator map_creator = null;
    public Vector3 goal_position;
    public float height_offset = 0.0f;

    void Start() {

        this.goal_position = this.transform.position;
    }

    void Update() {

        this.height_offset *= 0.90f * (Time.deltaTime / (1.0f / 60.0f));
        this.transform.position = this.goal_position + this.height_offset * Vector3.up;

        //코인 회전
        float spin_speed = (360.0f / 2.0f);
        this.transform.rotation *= Quaternion.AngleAxis(spin_speed * Time.deltaTime, Vector3.up);

        //화면 벗어날 시 삭제
        if (this.map_creator.isDelete(this.gameObject)) {
            GameObject.Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter(Collider other) {
        
        //플레이어와 동전이 닿으면 점수 상승
        if (other.tag == "Player") {

            this.score_control.addCoinScore();
        }

        GameObject.Destroy(this.gameObject);
    }
    
}
