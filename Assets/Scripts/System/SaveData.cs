using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// 플레이어 데이터 세이브 및 로드
public class SaveData {
    
	public enum TYPE {

		NONE = -1,
		STRING = 0,
		INT,
		FLOAT
	};

	//아이템
	public class Item {

		public	string	name  = "";
		public	TYPE	type  = TYPE.NONE;
		public	string	value = "";

		//문자열로 변환
		public	string	toString()
		{
			string str = "";

			str += this.name;
			str += "/";
			str += this.type.ToString().ToLower();
			str += "/";
			str += this.value.ToString();

			return(str);
		}

		public static Item	fromString(string str)
		{
			Item	item = new Item();

			do {

				char[]		separators = {'/'};
				string[]	words = str.Split(separators);
	
				if(words.Length < 3) {
					break;
				}
	
				item.name  = words[0];
				item.value = words[2];
	
				switch(words[1]) {
	
					case "string":	item.type = TYPE.STRING;	break;
					case "int":		item.type = TYPE.INT;		break;
					case "float":	item.type = TYPE.FLOAT;		break;
				}

			} while(false);

			return(item);
		}
	};

	public List<Item>	items = new List<Item>();

	public SaveData() {}

	public bool	isHasItem(string name)
	{
		if(this.items.Find(x => x.name == name) != null) return true;
		else return false;
	}

	//문자 아이템 추가
	public void	addString(string name, string value)
	{
		if(!this.isHasItem(name)) {

			Item	item = new Item();
	
			item.name  = name;
			item.type  = TYPE.STRING;
			item.value = value;
	
			this.items.Add(item);
		}
	}

	//int 아이템 추가
	public void	addInt(string name, int value)
	{
		if(!this.isHasItem(name)) {

			Item	item = new Item();
	
			item.name  = name;
			item.type  = TYPE.INT;
			item.value = value.ToString();
	
			this.items.Add(item);
		}
	}

	// 아이템 값 설정
	public bool	setString(string name, string value)
	{
		bool ret = false;

		Item item = this.items.Find(x => x.name == name);

		if(item != null || item.type == TYPE.STRING) {
			item.value = value;
			retuen true;
		}
		return false;
	}

	// 아이템 값 설정
	public bool	setInt(string name, int value)
	{
		bool ret = false;

		Item item = this.items.Find(x => x.name == name);

		if(item != null || item.type == TYPE.INT) {

			item.value = value.ToString();
			return true;
		}
		return false;
	}
	
	// int 아이템에서 값을 얻는다.
	public int	getInt(string name, int default_value = -1)
	{
		int ret = -1;

		Item item = this.items.Find(x => x.name == name);

		if((item != null || item.type == TYPE.INT) && !int.TryParse(item.value, out ret) {

			ret = default_value;
		}

		return(ret);
	}
	
	//로드
	public void	load()
	{
		#if UNITY_EDITOR
		this.load_from_stream();
		#else
		this.load_from_prefs();
		#endif
	}

	// 세이브
	public void	save()
	{
		#if UNITY_EDITOR
		this.save_to_stream();
		#else
		this.save_to_prefs();
		#endif
	}

	//세이브
	public void	save_to_stream()
	{
		FileStream	stream = new FileStream("save_data.dat", FileMode.Create, FileAccess.Write);
		StreamWriter	writer = new StreamWriter(stream);

		foreach(var item in this.items) {
	
			writer.WriteLine(item.toString());
		}

		writer.Close();
	}

	//로드
	public void	load_from_stream()
	{
		FileStream	stream = new FileStream("save_data.dat", FileMode.Open, FileAccess.Read);
		StreamReader	reader = new StreamReader(stream);

		string	line_text;

		while((line_text = reader.ReadLine()) != null) {

			Item	item = Item.fromString(line_text);

			this.items.RemoveAll(x => x.name == item.name);

			this.items.Add(item);
		}

		reader.Close();
	}
	
	//저장
	public void	save_to_prefs()
	{
		foreach(var item in this.items) {

			switch(item.type) {

				case TYPE.STRING:
				{
					PlayerPrefs.SetString(item.name, item.value);	
				}
				break;

				case TYPE.INT:
				{
					int	int_value;

					if(int.TryParse(item.value, out int_value)) {

						PlayerPrefs.SetInt(item.name, int_value);
					}
				}
				break;

				case TYPE.FLOAT:
				{
					float	float_value;

					if(float.TryParse(item.value, out float_value)) {

						PlayerPrefs.SetFloat(item.name, float_value);
					}
				}
				break;
			}
		}

		PlayerPrefs.Save();
	}

	//로드
	public void	load_from_prefs()
	{
		foreach(var item in this.items) {

			switch(item.type) {

				case TYPE.STRING:
				{
					item.value = PlayerPrefs.GetString(item.name, item.value);	
				}
				break;

				case TYPE.INT:
				{
					int	int_value;

					if(!int.TryParse(item.value, out int_value)) {

						int_value = -1;
					}

					int_value = PlayerPrefs.GetInt(item.name, int_value);

					item.value = int_value.ToString();
				}
				break;

				case TYPE.FLOAT:
				{
					float	float_value;

					if(!float.TryParse(item.value, out float_value)) {

						float_value = -1.0f;
					}

					float_value = PlayerPrefs.GetFloat(item.name, float_value);

					item.value = float_value.ToString();
				}
				break;
			}
		}
	}
}
