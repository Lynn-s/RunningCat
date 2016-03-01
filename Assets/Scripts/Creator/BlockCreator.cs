using UnityEngine;
using System.Collections;

public class BlockCreator : MonoBehaviour {

	public MapCreator map_creator = null;
	public GameObject[]	blockPrefabs;	
	private int block_count = 0; //블록 개수

	void	Start()
	{	
	}
	
	void	Update()
	{
	}    

	//블록 생성
	public void	createBlock(LevelControl.CreationInfo current_block, Vector3 block_position) {

		if(current_block.block_type == Block.TYPE.FLOOR) {//현재 바닥일 시

			int	next_block_type = this.block_count%this.blockPrefabs.Length;

			GameObject go = GameObject.Instantiate(this.blockPrefabs[next_block_type]) as GameObject;
			BlockControl new_block = go.GetComponent<BlockControl>();

			new_block.transform.position = block_position;
			new_block.map_creator = this.map_creator;

			this.block_count++;
		}
	}
}