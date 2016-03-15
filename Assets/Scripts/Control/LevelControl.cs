using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelData
{
    public struct Range
    {
        public int min;
        public int max;
    };

    public LevelData()
    {
        //초기화
        this.end_time = 15.0f;
        this.player_speed = 6.0f;
        this.floor_count.min = 10;
        this.floor_count.max = 10;
        this.hole_count.min = 2;
        this.hole_count.max = 6;
        this.height_diff.min = 0;
        this.height_diff.max = 0;
        this.coin_interval.min = 1;
        this.coin_interval.max = 1;
    }

    public float end_time;      //종료 시간
    public float player_speed;  //플레이어 속도

    public Range floor_count;   //블록 개수 범위
    public Range hole_count;    //구멍 개수 범위
    public Range height_diff;   //발판 높이 범위
    public Range coin_interval; //코인 간격 범위
}

public class LevelControl
{

    private List<LevelData> level_datas = new List<LevelData>();

    public int HEIGHT_MAX = 20;     //최대 높이
    public int HEIGHT_MIN = -4;     //최소 높이

    public int current_level = 0;   //현재 레벨
    public int block_count = 0;     //생성 블록 수


    public struct CreationInfo
    {

        public Block.TYPE block_type;
        public int max_count;
        public int height;
        public int current_count;

    };

    public CreationInfo current_block;
    public CreationInfo next_block;

    public float getPlayerSpeed()
    {

        return (this.level_datas[this.current_level].player_speed);

    }


    //전달된 블록 초기화
    public void clear_next_block(ref CreationInfo block)
    {
        block.block_type = Block.TYPE.FLOOR;
        block.max_count = 15;
        block.height = 0;
        block.current_count = 0;
    }

    public void initialize()
    {
        this.block_count = 0; //블록 총 갯수 초기화
        this.current_level = 0;//레벨 초기화

        //현재, 다음 블록 초기화
        this.clear_next_block(ref this.current_block);
        this.clear_next_block(ref this.next_block);
    }

    private void update_level(ref CreationInfo current, CreationInfo previous, float passage_time)
    {

        //레벨 반복
        float local_time = Mathf.Repeat(passage_time, this.level_datas[this.level_datas.Count - 1].end_time);

        //현재 레벨 구하기
        int i;
        for (i = 0; i < this.level_datas.Count - 1; i++)
        {
            if (local_time <= this.level_datas[i].end_time) break;//레벨 구하고 빠져나감
        }
        this.current_level = i;//레벨 대입

        current.block_type = Block.TYPE.FLOOR;
        current.max_count = 1;
        
        if (this.block_count >= 10)
        {
            LevelData level_data;
            level_data = this.level_datas[this.current_level];

            if(previous.block_type == Block.TYPE.FLOOR) { //블록일 경우
                    current.block_type = Block.TYPE.HOLE;   //구멍 생성
                    current.max_count = Random.Range(level_data.hole_count.min, level_data.hole_count.max + 1);
                    current.height = previous.height;
            }
            else { //구멍일 경우
                current.block_type = Block.TYPE.FLOOR;  //블록 생성
                current.max_count = Random.Range(level_data.floor_count.min, level_data.floor_count.max);

                int height_min = previous.height + level_data.height_diff.min;
                int height_max = previous.height + level_data.height_diff.max;

                height_min = Mathf.Clamp(height_min, HEIGHT_MIN, HEIGHT_MAX);
                height_max = Mathf.Clamp(height_max, HEIGHT_MIN, HEIGHT_MAX);

                current.height = Random.Range(height_min, height_max + 1);;
            }
            
        }

    }

    public void update(float passage_time)
    {

        this.current_block.current_count++;

        if (this.current_block.current_count >= this.current_block.max_count)
        {

            this.current_block = this.next_block;

            //다음 생성 블록의 내용 초기화
            this.clear_next_block(ref this.next_block);
            //다음 생성 블록 설정
            this.update_level(ref this.next_block, this.current_block, passage_time);

        }

        this.block_count++;
    }

    public void loadLevelData(TextAsset level_data_text)
    {
        //텍스트를 문자열로 가져오기
        string level_texts = level_data_text.text;

        string[] lines = level_texts.Split('\n');

        foreach (var line in lines)
        {
            if (line == "") continue;
            
            string[] words = line.Split();
            int n = 0;

            LevelData level_data = new LevelData();

            foreach (var word in words)
            {
                if (word == "") continue;
                else if(word.StartsWith("#"))   break;

                switch (n)
                {
                    case 0: level_data.end_time = float.Parse(word); break;
                    case 1: level_data.player_speed = float.Parse(word); break;
                    case 2: level_data.floor_count.min = int.Parse(word); break;
                    case 3: level_data.floor_count.max = int.Parse(word); break;
                    case 4: level_data.hole_count.min = int.Parse(word); break;
                    case 5: level_data.hole_count.max = int.Parse(word); break;
                    case 6: level_data.height_diff.min = int.Parse(word); break;
                    case 7: level_data.height_diff.max = int.Parse(word); break;
                    case 8: level_data.coin_interval.min = int.Parse(word); break;
                    case 9: level_data.coin_interval.max = int.Parse(word); break;
                }
                n++;
            }

            if (n >= 10) 
            {   //10항목 이상 제대로 처리 시
                this.level_datas.Add(level_data);
            }
        }

        if (this.level_datas.Count == 0)
        {   //level_datas에 데이터가 하나도 없을 시
            Debug.LogError("LevelData Has No Data");
            this.level_datas.Add(new LevelData());//기본 LevelData 추가
        }

    }

    public LevelData getCurrentLevelData()
    {
        return (this.level_datas[this.current_level]);
    }

}
