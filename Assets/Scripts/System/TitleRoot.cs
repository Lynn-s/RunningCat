using UnityEngine;
using System.Collections;

//타이틀 화면
public class TitleRoot : MonoBehaviour {

	public Texture title_texture = null;
	public Texture start_texture = null;
    

	public enum STEP {

		NONE = -1,
		LOAD_SAVE_DATA = 0,	//데이터 로드
		WAIT_CLICK,			//클릭 대기
		START_ACTION,		//시작 액션
		GAME_START,			//게임 시작
	};

	public STEP step = STEP.NONE;
	public STEP next_step = STEP.NONE;

	private SoundControl sound_control = null;

	void Start() {

		this.next_step = STEP.LOAD_SAVE_DATA;
		this.sound_control = GameObject.Find("SoundRoot").GetComponent<SoundControl>();
    }

	void Update() { 

		if(this.next_step == STEP.NONE) {

			switch(this.step) {
	
				case STEP.LOAD_SAVE_DATA:				
					this.next_step = STEP.WAIT_CLICK;				
				    break;

				case STEP.WAIT_CLICK: 
					if(Input.GetMouseButtonDown(0)) {

						this.next_step = STEP.START_ACTION;
						this.sound_control.playSound(Sound.SOUND.CLICK);
					}				
				    break;

				case STEP.START_ACTION:
					this.next_step = STEP.GAME_START;
				    break;
			}
		}

        //상태 전환 시
		while(this.next_step != STEP.NONE) {

			this.step      = this.next_step;
			this.next_step = STEP.NONE;

			switch(this.step) {
	
				case STEP.LOAD_SAVE_DATA:
					GlobalParam.getInstance().loadSaveData();				
				    break;

				case STEP.GAME_START:
					Application.LoadLevel("GameScene");				
				    break;
			}
            
		}
        
	}

	void OnGUI() {
		Rect	rect = new Rect();

		//배경
		rect.x = 0.0f;
		rect.y = 0.0f;
		rect.width  = this.title_texture.width;
		rect.height = this.title_texture.height;

		GUI.DrawTexture(rect, this.title_texture);

        float scale = 1.0f;

        //시작 버튼	
        if (this.step == STEP.START_ACTION) { //클릭 시 버튼 효과

            scale = 1.2f;
        }
        rect.width  = this.start_texture.width*scale;
		rect.height = this.start_texture.height*scale;
		rect.x = Screen.width*0.8f  - rect.width/2.0f;
		rect.y = Screen.height*0.9f - rect.height/2.0f;

		GUI.DrawTexture(rect, this.start_texture);
		
	}
}
