using UnityEngine;
using System.Collections;

// 게임 진행 관리.
public class GameRoot : MonoBehaviour {
	private PlayerControl player = null;
	private ScoreControl score_control = null;

	public enum STEP {

		NONE = -1,
		PLAY = 0,		//게임 진행
		WAIT_CLICK,		//게임 끝난 후 클릭 대기
		RESULT			//결과
	};

	public STEP	step      = STEP.NONE;
	public STEP	next_step = STEP.NONE;
	public float step_timer = 0.0f;

	private SoundControl	sound_control = null;

	void	Start()	{
		this.player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
		this.score_control = this.gameObject.GetComponent<ScoreControl>();
		this.sound_control = GameObject.Find("SoundRoot").GetComponent<SoundControl>();

		this.next_step = STEP.PLAY;
	}

	void Update() {
		this.step_timer += Time.deltaTime;

		if(this.next_step == STEP.NONE) {

			switch(this.step) {

				case STEP.PLAY:				
					if(this.player.isPlayEnd()) {

						this.next_step = STEP.WAIT_CLICK;
					}				
				    break;

				case STEP.WAIT_CLICK:
					if(Input.GetMouseButtonDown(0)) {

						this.next_step = STEP.RESULT;
					}				
				    break;

			}
		}        

        	//상태 전환 시 초기화
		if(this.next_step != STEP.NONE){

			this.step      = this.next_step;
			this.next_step = STEP.NONE;

			switch(this.step) {

				case STEP.PLAY:				
					this.sound_control.playBgm(Sound.BGM.PLAY);				
				    break;

				case STEP.WAIT_CLICK:				
					this.sound_control.stopBgm();				
				    break;

				case STEP.RESULT:
					ScoreControl.Score	score = this.score_control.getCurrentScore();
					GlobalParam.getInstance().setLastScore(score);
					Application.LoadLevel("ResultScene");				
				    break;
			}

			this.step_timer = 0.0f;
		}

	}
    
    //진행 시간 반환
	public float getPlayTime() {
		float time;

		if(this.step == STEP.PLAY) {
			time = this.step_timer;
		}else {
			time = 0.0f;
		}

		return(time);
	}
}
