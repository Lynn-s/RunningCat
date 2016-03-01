using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {

    public static float ACCELERATION = 10.0f;           //가속도
    public static float SPEED_MIN = 5.0f;               //최소 속도
    public static float SPEED_MAX = 10.0f;               //최대 속도
    public static float JUMP_HEIGHT_MAX = 3.0f;         //점프 최대 높이
    public static float JUMP_KEY_RELEASE_REDUCE = 0.5f; //점프 후 감속
    public static float FALLEN_HEIGHT = -5.0f;          //떨어진 위치
    public float current_speed = 0.0f;                  //현재 속도
    public LevelControl level_control = null;           //LevelControl 저장
    private float click_timer = -1.0f;                  //버튼이 눌린 후의 시간
    private float CLICK_GRACE_TIME = 0.5f;              //점프 의사를 받아들일 시간

    public static float COLLISION_SIZE = 1.0f;              //충돌 지름

    private ScoreControl score_control = null;      //점수 관리
    private BlockControl stepped_block = null;


    public enum STEP {

        NONE = -1,
        RUN = 0,
        JUMP,
        MISS,
        NUM,
    };

    public STEP step = STEP.NONE;
    public STEP next_step = STEP.NONE;

    public float step_timer = 0.0f; //경과 시간
    private bool is_landed = false; //착지 여부
    private bool is_colided = false; //충돌 여부
    private bool is_key_released = false; //클릭 여부

    private SoundControl sound_control = null;
    
    void Start() {

        this.next_step = STEP.RUN;
        this.score_control = ScoreControl.getInstance();        
        this.sound_control = GameObject.Find("SoundRoot").GetComponent<SoundControl>();
    }

   
    void Update() {

        Vector3 velocity = this.GetComponent<Rigidbody>().velocity;//속도 설정
        this.current_speed = this.level_control.getPlayerSpeed();
        this.check_landed();//착지 여부 체크

        switch (this.step) {

            case STEP.RUN:
            case STEP.JUMP:
                //플레이어가 떨어졌을 시
                if (this.transform.position.y < FALLEN_HEIGHT) {
                    this.next_step = STEP.MISS;//실패
                }
                break;

        }
        this.step_timer += Time.deltaTime;//경과 시간

        if (Input.GetMouseButtonDown(0)) {//버튼이 눌릴 경우
            this.click_timer = 0.0f;    //타이머 리셋
        }
        else {
            if (this.click_timer >= 0.0f) {//아닌 경우
                this.click_timer += Time.deltaTime; //경과 시간 더함
            }
        }

        if (this.next_step == STEP.NONE) {

            switch (this.step) {

                case STEP.RUN:
                    //타이머가 0 이상 수용 시간 이하일 때
                    if (0.0f <= this.click_timer && this.click_timer <= CLICK_GRACE_TIME) {
                        if (this.is_landed) {   //착지 시
                            this.click_timer = -1.0f;   //버튼이 눌리지 않은 상태를 나타냄
                            this.next_step = STEP.JUMP;
                        }
                    }
                    break;

                case STEP.JUMP:
                    if (this.is_landed) {
                        this.sound_control.playSound(Sound.SOUND.TDOWN);
                        velocity.x = PlayerControl.SPEED_MIN;
                        this.next_step = STEP.RUN;
                        if (this.stepped_block != null) {
                            this.stepped_block.onStepped();
                        }
                    }
                    break;
            }
        }
        while (this.next_step != STEP.NONE) {//상태가 변할 때

            this.step = this.next_step;//현재 상태를 다음 상태로
            this.next_step = STEP.NONE;//다음 상태를 NONE으로

            switch (this.step) {
                case STEP.JUMP:
                    velocity.y = Mathf.Sqrt(1.5f * 9.8f * PlayerControl.JUMP_HEIGHT_MAX);
                    this.is_key_released = false;
                    break;
            }
            this.step_timer = 0.0f;
        }

        switch (this.step) {

            case STEP.RUN://달릴 때
                velocity.x += PlayerControl.ACCELERATION * Time.deltaTime;//가속

                if (Mathf.Abs(velocity.x) > PlayerControl.SPEED_MAX) {//최고 속도 초과 시
                    velocity.x *= this.current_speed / Mathf.Abs(velocity.x);
                }
                if (Mathf.Abs(velocity.x) < PlayerControl.SPEED_MIN) {//속도 임시 제한
                    velocity.x += PlayerControl.SPEED_MAX;
                }
                break;

            case STEP.JUMP://점프 시
                do {
                    if (!Input.GetMouseButtonUp(0)) {//버튼이 떨어지지 않았을 때
                        break;
                    }
                    if (this.is_key_released) {//감속 시
                        break;
                    }
                    if (velocity.y <= 0.0f) {//하강 시
                        break;
                    }

                    //velocity.y *= JUMP_KEY_RELEASE_REDUCE;//상승 시 감속

                    this.is_key_released = true;

                } while (false);
                break;

            case STEP.MISS: //플레이어의 속도를 줄인다
                velocity.x -= PlayerControl.ACCELERATION * Time.deltaTime;
                if (velocity.x < 0.0f) {
                    velocity.x = 0.0f;
                }
                break;

        }
        this.GetComponent<Rigidbody>().velocity = velocity;//속도 갱신
    }

    private void check_landed() {
        this.is_landed = false;
        do {
            Vector3 s = this.transform.position;
            Vector3 e = s + Vector3.down * 1.0f;
            RaycastHit hit;
            if (!Physics.Linecast(s, e, out hit)) { //현재 위치 s와 아래로 1.0f 이동한 위치에 아무 것도 없을 때. 히트 시 true 반환
                break;
            }

            if (this.step == STEP.JUMP)
            {
                if (this.step_timer < Time.deltaTime * 3.0f)
                {
                    break;
                }
            }
            BlockControl block = hit.collider.GetComponent<BlockControl>();

            if (block != null) {

                this.stepped_block = block;//밟힌 블록
            }
            this.is_landed = true;
        } while (false);
    }

    //게임이 끝났는지 판별
    public bool isPlayEnd() {

        bool ret = false;

        switch (this.step) {
            case STEP.MISS:
                ret = true;
                break;
        }
        return (ret);
    }


}
