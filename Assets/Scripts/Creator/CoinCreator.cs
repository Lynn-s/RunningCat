using UnityEngine;
using System.Collections;

public class CoinCreator : MonoBehaviour {

	public GameObject coin_prefab = null;	
	private int next_block = 10;

	private ScoreControl score_control  = null;
	public	MapCreator map_creator = null;    

	public static CoinCreator getInstance() {
		CoinCreator	coin_creator = GameObject.FindGameObjectWithTag("Game Root").GetComponent<CoinCreator>();

		return(coin_creator);
	}    

	void	Start()	{

		this.score_control  = this.gameObject.GetComponent<ScoreControl>();
	}
	
	void	Update() {	
	}
    
	public void	createCoin(LevelData level_data, int block_count, Vector3 block_position) {
		//블록 수에 따라 코인 생성
		if(block_count >= this.next_block) {
			Vector3	p0 = block_position;
	
			//코인 높이 설정
			p0.y += MapCreator.BLOCK_HEIGHT/2.0f + CoinControl.COLLISION_SIZE/2.0f;	
			p0.y += PlayerControl.COLLISION_SIZE;
	
			this.create_coin_object(p0); //코인 생성

			//다음 코인 만들 블록 갱신
			this.next_block += Random.Range(level_data.coin_interval.min, level_data.coin_interval.max + 1);
		}
	}	

	//오브젝트 생성
	private CoinControl	create_coin_object(Vector3 position) { 
		GameObject	go = GameObject.Instantiate(this.coin_prefab) as GameObject;

		CoinControl	coin = go.GetComponent<CoinControl>();

		coin.score_control = this.score_control;
		coin.map_creator   = this.map_creator;

		coin.transform.position = position;

		return(coin);
	}	

}
