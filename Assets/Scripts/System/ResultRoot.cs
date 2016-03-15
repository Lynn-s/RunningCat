using UnityEngine;
using System.Collections;

public class ResultRoot : MonoBehaviour {

    	//위치 설정
	private static float RESULT_SCORE_POS_X = 640.0f/2.0f -64.0f;	
	private static float RESULT_SCORE_POS_Y = 120.0f;
	private static float RESULT_HIGH_SCORE_POS_X = 640.0f/2.0f -64.0f;	
	private static float RESULT_HIGH_SCORE_POS_Y = 240.0f;
	private static float RESULT_HIGH_COIN_POS_X = 640.0f/2.0f -64.0f;
	private static float RESULT_HIGH_COIN_POS_Y = 340.0f;
    
	public Texture	result_texture = null;
	public Texture	next_texture   = null;

	private ScoreDisp score_disp = null;

	private ScoreControl.Score high_score; //최고 기록
	private ScoreControl.Score cur_score; //현재 기록

	private int disp_last_score = 0; //표시 점수    

	private SoundControl sound_control = null;

	public enum STEP {

		NONE = -1,
		RESULT = 0, //결과
		RESULT_ACTION, //결과화면 클릭
		TITLE //타이틀 화면
	};

	public STEP step = STEP.NONE;
	public STEP next_step = STEP.NONE;
	public float step_timer = 0.0f;

	static private float ACTION_TIME = 1.0f;

	void Start()	{

		this.score_disp = GameObject.FindGameObjectWithTag("Score Disp").GetComponent<ScoreDisp>();
		this.high_score = GlobalParam.getInstance().getHighScore();
		this.cur_score = GlobalParam.getInstance().getLastScore();
		this.next_step = STEP.RESULT;

		this.sound_control = GameObject.Find("SoundRoot").GetComponent<SoundControl>();
	}

	void Update() {

		this.step_timer += Time.deltaTime;

		if(this.next_step == STEP.NONE) {

			switch(this.step) {

				case STEP.RESULT://결과 화면
					if(Input.GetMouseButtonDown(0)) {//클릭 시 타이틀 화면 단계 준비 

						this.next_step = STEP.RESULT_ACTION;
						this.sound_control.playSound(Sound.SOUND.CLICK);
					}
				    break;

				case STEP.RESULT_ACTION://타이틀 화면 이동
					if(this.step_timer > ACTION_TIME) {

						this.next_step = STEP.TITLE;
					}
				    break;
			}
		}

        while (this.next_step != STEP.NONE) {

            this.step = this.next_step;
            this.next_step = STEP.NONE;

            switch (this.step) {

                case STEP.RESULT:
                    //최고 기록 저장
                    GlobalParam.getInstance().saveSaveData();
                    this.sound_control.playBgm(Sound.BGM.RESULT);
                    break;

                case STEP.RESULT_ACTION:
                    this.sound_control.stopBgm();
                    this.disp_last_score = this.cur_score.score; //최종 점수 넘김		
                    break;

                case STEP.TITLE:
                    Application.LoadLevel("TitleScene");//타이틀씬으로 이동				
                    break;
            }

            this.step_timer = 0.0f;
        }
		
		switch(this.step) {

			case STEP.RESULT:
			this.disp_last_score += (int)(100 *Time.deltaTime);
			this.disp_last_score = Mathf.Clamp(this.disp_last_score, 0, this.cur_score.score);
			if(this.disp_last_score < this.cur_score.score) {//코인 획득 시 효과음
				this.sound_control.playSound(Sound.SOUND.COIN_GET);
			}
			break;
		}
	}

	void OnGUI() {

		Rect rect = new Rect();

		Texture	back_texture;

		switch(this.step) {
			default:			
			    back_texture = this.result_texture;			
			break;
		}

        	//위치 설정
		rect.x = 0.0f;
		rect.y = 0.0f;
		rect.width  = back_texture.width;
		rect.height = back_texture.height;

		GUI.DrawTexture(rect, back_texture);
        

		float scale = 1.0f;

		if(this.step == STEP.RESULT_ACTION) { //클릭 시 버튼 효과
            		scale = 1.2f;
		}

		rect.width  = this.next_texture.width*scale;
		rect.height = this.next_texture.height*scale;

		rect.x = Screen.width*0.9f - rect.width/2.0f;
		rect.y = Screen.height*0.9f - rect.height/2.0f;

		GUI.DrawTexture(rect, this.next_texture);
        
		switch(this.step) {
			case STEP.RESULT:
			case STEP.RESULT_ACTION:
			case STEP.TITLE:
			{
				this.score_disp.dispNumber(new Vector2(RESULT_SCORE_POS_X, RESULT_SCORE_POS_Y), this.disp_last_score);// this.cur_score.score);
				this.score_disp.dispNumber(new Vector2(RESULT_HIGH_SCORE_POS_X, RESULT_HIGH_SCORE_POS_Y), this.high_score.score);
				this.score_disp.dispNumber(new Vector2(RESULT_HIGH_COIN_POS_X, RESULT_HIGH_COIN_POS_Y), this.high_score.coins);
			}
			break;
		}
	}
}
