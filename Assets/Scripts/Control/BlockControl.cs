using UnityEngine;
using System.Collections;

public class BlockControl : MonoBehaviour {

	public MapCreator map_creator = null;
	private	GameObject model = null;
	private bool trigger_stepped = false; //밟힌 순간

	//스프링 효과
	private struct Spring {

		public	float	velocity;
		public	float	position;
	};
	private	Spring spring;

	void Start()	{
		this.model = this.transform.FindChild("model").gameObject;

		this.spring.velocity = 0.0f;
		this.spring.position = 0.0f;
	}
	
	void Update() {

		if(this.trigger_stepped) {//밟힐 시

			this.spring.velocity -= 2.0f;
			this.trigger_stepped = false;
		}
        
		if(this.spring.velocity < 1.0f) {

			this.spring.velocity += 6.0f*Time.deltaTime;
		}

		this.spring.position += this.spring.velocity*Time.deltaTime;

		if(this.spring.position >= 0.0f) { //원래 위치로

			this.spring.position = 0.0f;
			this.spring.velocity = 0.0f;
		}
        
        //화면에서 벗어날 시 삭제
		if(this.map_creator.isDelete(this.gameObject)) {

			GameObject.Destroy(this.gameObject);
		}
	}

	void	LateUpdate() {

		this.model.transform.localPosition += Vector3.up*this.spring.position;
	}
    
	public void	onStepped()	{

		this.trigger_stepped = true;
	}
}
