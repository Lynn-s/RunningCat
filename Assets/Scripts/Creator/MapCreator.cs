using UnityEngine;
using System.Collections;

public class Block {

    //블록 종류 구분
    public enum TYPE {

        NONE = -1,
        FLOOR = 0,      //바닥
        HOLE,           //구멍
        NUM,            //블록 종류 수
    };
}

public class MapCreator : MonoBehaviour {

    public static float BLOCK_WIDTH = 1.0f;
    public static float BLOCK_HEIGHT = 0.2f;
    public static int BLOCK_NUM_IN_SCREEN = 24;
    public TextAsset level_data_text = null;

    private LevelControl level_control = null;

    //블록 관리
    private struct FloorBlock
    {
        public bool is_created;
        public Vector3 position;
    };

    private PlayerControl player = null;
    private FloorBlock last_block;
    private GameRoot game_root = null;
    private BlockCreator block_creator = null;
    private CoinCreator coin_creator = null;

    void Start() {

        this.player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        this.last_block.is_created = false;

        this.game_root = this.gameObject.GetComponent<GameRoot>();
        this.block_creator = this.gameObject.GetComponent<BlockCreator>();
        this.coin_creator = this.gameObject.GetComponent<CoinCreator>();

        this.level_control = new LevelControl();
        this.level_control.initialize();
        this.level_control.loadLevelData(this.level_data_text);
        this.player.level_control = this.level_control;

        this.block_creator.map_creator = this;
        this.coin_creator.map_creator = this;

        this.create_floor_block();

    }

    void Update() {

        float block_generate_x = this.player.transform.position.x;

        block_generate_x += BLOCK_WIDTH * ((float)BLOCK_NUM_IN_SCREEN + 1) / 2.0f;

        while (this.last_block.position.x < block_generate_x)
        {

            this.create_floor_block();

        }

    }

    private void create_floor_block() {

        //Vector3 block_position;
        //if (!this.last_block.is_created) {
        //    block_position = this.player.transform.position;
        //    //블록의 x좌표를 화면 절반만큼 왼쪽으로 이동
        //    block_position.x -= BLOCK_WIDTH * ((float)BLOCK_NUM_IN_SCREEN / 2.0f);
        //    //y좌표를 0으로
        //    block_position.y = 0.0f;
        //}
        //else {
        //    block_position = this.last_block.position;
        //}

        ////1블록 이동
        //block_position.x += BLOCK_WIDTH;

        //this.level_control.update(this.game_root.getPlayTime());

        ////current_block의 높이를 씬 상의 좌표로 변환
        //block_position.y = level_control.current_block.height * BLOCK_HEIGHT;

        //LevelControl.CreationInfo current = this.level_control.current_block;

        ////블록 생성
        //if(current.block_type == Block.TYPE.FLOOR) {
        //    this.block_creator.createBlock(block_position);
        //}

        ////코인 생성
        //LevelData level_data = this.level_control.getCurrentLevelData();
        //this.coin_creator.createCoin(level_data, this.level_control.block_count, block_position);

        ////last_block 위치 갱신
        //this.last_block.position = block_position;
        //this.last_block.is_created = true;

        Vector3 block_position;
        
        //다음 블록 타입 결정
        this.level_control.update(this.game_root.getPlayTime());

        //직전 블록 위치 구함
        if (!this.last_block.is_created)
        {
            //블록이 없을 시 화면 왼쪽을 기준으로
            block_position = this.player.transform.position;
            block_position.x -= BLOCK_WIDTH * ((float)BLOCK_NUM_IN_SCREEN / 2.0f);

        }
        else {

            block_position = this.last_block.position;
        }

        block_position.x += BLOCK_WIDTH;
        block_position.y = (float)this.level_control.current_block.height * BLOCK_HEIGHT;

       
        //블록 생성
        LevelControl.CreationInfo current = this.level_control.current_block;
        this.block_creator.createBlock(current, block_position);

        //코인 생성
        LevelData level_data = this.level_control.getCurrentLevelData();
        this.coin_creator.createCoin(level_data, this.level_control.block_count, block_position);

        //블록 위치 갱신
        this.last_block.position = block_position;
        this.last_block.is_created = true;
    }

    public bool isDelete(GameObject block_object) {

        bool ret = false;

        float left_limit = this.player.transform.position.x - BLOCK_WIDTH * ((float)BLOCK_NUM_IN_SCREEN / 2.0f);

        if (block_object.transform.position.x < left_limit) {

            ret = true;
        }

        return (ret);
    }

}
