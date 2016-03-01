using UnityEngine;
using System.Collections;

public class ScoreDisp : MonoBehaviour {

	public	Texture[]	number_textures;

	private static float TEXTURE_WIDTH = 48; //폭
	private static float TEXTURE_HEIGHT	= 48; //높이
	private static int MAX_DIGITS = 4; //최대 자리수
	private static float SPACE = 32; //문자 간격
    

	void	Start()	{
	}
	
	void Update() { 
	}
	
	//점수 표시
	public void	dispNumber(Vector2 pos, int number)	{

		int	i;
		int digits;//자리수

		if(number <= 0) {
			number = 0;
			digits = 1;
		} else {
			digits = (int)Mathf.Log10(number) + 1;
		}

		//위치
		Vector2	p = pos;
		p.x += (MAX_DIGITS - 1)*SPACE;

		int	n = number;
        
		for(i = 0;i < digits;i++) { //10으로 나누어가며 자리수 표시

			int	digit = n%10;
			Texture	texture = this.number_textures[digit];
			GUI.DrawTexture(new Rect(p.x, p.y, TEXTURE_WIDTH, TEXTURE_HEIGHT), texture);
			p.x -= SPACE;
			n /= 10;
		}
	}
}
