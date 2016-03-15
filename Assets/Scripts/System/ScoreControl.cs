using UnityEngine;
using System.Collections;

public class ScoreControl : MonoBehaviour {

	public struct Score {

        	public int score; //점수
		public int coins; //코인 수
	};

	private ScoreDisp score_disp = null;

	private static float POS_X = 0.0f; //위치 설정
	private static float POS_Y = 10.0f;

	private static float POS_COIN_X = 640.0f - 64.0f*3;
	private static float POS_HIGHSCORE_X = 640.0f/2.0f - 64.0f;

	private Score score; //현재 점수
	private Score high_score;

	private SoundControl	sound_control = null;

	public Texture 		icon_totalcoin = null;
	public Texture 		icon_top = null;

	public static ScoreControl	getInstance() {

		ScoreControl	score_control = GameObject.FindGameObjectWithTag("Game Root").GetComponent<ScoreControl>();

		return(score_control);
	}

	void	Start()	{

		this.score.score = 0;//초기 점수
		this.score.coins = 0;

		this.score_disp = GameObject.FindGameObjectWithTag("Score Disp").GetComponent<ScoreDisp>();
		this.high_score = GlobalParam.getInstance().getHighScore();

		this.score.coins = this.high_score.coins;

		this.sound_control = GameObject.Find("SoundRoot").GetComponent<SoundControl>();
	}
	
	void	Update() {

		GlobalParam.getInstance().setLastScore(this.score);
		this.high_score = GlobalParam.getInstance().getHighScore();
	}

	void	OnGUI()	{

		this.score_disp.dispNumber(new Vector2(POS_X, POS_Y), this.score.score);
		this.score_disp.dispNumber(new Vector2(POS_HIGHSCORE_X, POS_Y), this.high_score.score);
		this.score_disp.dispNumber(new Vector2(POS_COIN_X, POS_Y), this.score.coins);
        
		GUI.DrawTexture(new Rect(POS_COIN_X -16, POS_Y, this.icon_totalcoin.width/2, this.icon_totalcoin.height/2), this.icon_totalcoin);
		GUI.DrawTexture(new Rect(POS_HIGHSCORE_X -32, POS_Y, this.icon_top.width, this.icon_top.height), this.icon_top);
	}

    	//코인 획득 시 점수 증가
	public void	addCoinScore() {

		this.score.score += 10;
		this.score.coins++;
		this.sound_control.playSound(Sound.SOUND.COIN_GET);
	}

	//현재 점수 반환
	public ScoreControl.Score getCurrentScore() {

        return (this.score);
	}
}
