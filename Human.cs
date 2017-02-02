using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class Human : StartButton {

	public GameObject BoxUnityChan;
	public GameObject BoxUnityChan_Lying;
	public GameObject BoxUnityChan_Sitting;
	string csvname;

	public bool alchemize_human;
	public bool change_pose;
	public bool change_position;
	public bool center_position;


	int pos_set(float xa,float xi,float za,float zi,GameObject hito){
		// 位置ラベル付け用変数
		float pointx = (xa - xi) / 3.0f;
		float pointz = (za - zi) / 3.0f;
		//Debug.Log ("pointx" + pointx);
		//Debug.Log ("pointz" + pointz);
		GameObject kabex = GameObject.Find ("Kabe04");
		GameObject kabez = GameObject.Find ("Kabe02");
		float wallx = kabex.transform.position.x;
		float wallz = kabez.transform.position.z;
		//Debug.Log ("wallx" + wallx);
		//Debug.Log ("wallz" + wallz);
		//float disx = Vector2.Distance (hito.transform.position.x, wallx);
		//float disz = Vector2.Distance (hito.transform.position.z, wallz);
		float disx = hito.transform.position.x - wallx;
		float disz = hito.transform.position.z - wallz;
		//Debug.Log ("人の位置" + hito.transform.position);
		//Debug.Log ("disx" + disx);
		//Debug.Log ("disz" + disz);

		int pos = -1;

		// 出現位置によってラベル付け
		if (disx < pointx && disz < pointz) {
			pos = 0;
		} else if (pointx < disx & disx < pointx * 2.0f && disz < pointz) {
			pos = 1;
		} else if (pointx * 2.0f < disx & disx < pointx * 3.0f && disz < pointz) {
			pos = 2;
		} else if (disx < pointx && pointz < disz & disz < pointz * 2.0f) {
			pos = 3;
		} else if (pointx < disx & disx < pointx * 2.0f && pointz < disz & disz < pointz * 2.0f) {
			pos = 4;
		} else if (pointx * 2.0f < disx & disx < pointx * 3.0f && pointz < disz & disz < pointz * 2.0f) {
			pos = 5;
		} else if (disx < pointx && pointz * 2.0f < disz & disz < pointz * 3.0f) {
			pos = 6;
		} else if (pointx < disx & disx < pointx * 2.0f && pointz * 2.0f < disz & disz < pointz * 3.0f) {
			pos = 7;
		} else if (pointx * 2.0f < disx & disx < pointx * 3.0f && pointz * 2.0f < disz & disz < pointz * 3.0f) {
			pos = 8;
		} else {
			Debug.Log ("nohit");
		}
		return pos;
	}

/*
	void Get_pos(GameObject hito){
		Vector3 human_pos = hito.transform.position;
		return human_pos;
	}
*/

	void Start(){
		GameObject sensor = GameObject.Find ("sensor");
		csvname=sensor.GetComponent <ray_information>().csvname;
		Application.targetFrameRate = 120;
	}

	//ボタンを押したら部屋サイズを変えるスクリプトを呼び出す
	protected override void OnClick(string objectName) {
		if ("Button06".Equals (objectName)) {
			this.Button06Click ();
		} else if ("Button08".Equals (objectName)) {
			this.Button08Click ();
		} else if ("Button10".Equals (objectName)) {
			this.Button10Click ();
		} else {
			throw new System.Exception ("Not implemented!!");
		}
	}

	//押したボタンによって部屋のスケールを変える
	private void Button06Click() {
		GameObject.Find ("House01").transform.localScale = new Vector3 (0.28125f, 0.7f, 0.375f);
		//StartCoroutine ("HumanAlchemize");
	}

	private void Button08Click (){
		GameObject.Find ("House01").transform.localScale = new Vector3 (0.375f, 0.7f, 0.375f);
		//StartCoroutine ("HumanAlchemize");
	}

	private void Button10Click (){
		GameObject.Find ("House01").transform.localScale = new Vector3 (0.375f, 0.7f, 0.46875f);
		//StartCoroutine ("HumanAlchemize");
	}

	int num;
	int pos;
	Vector3 human_pos;

	//人体錬成スクリプト
	public void CreateHuman(){
		if (human != null) {
			Destroy (human);
		}
		if (alchemize_human == true) {  //trueで人体錬成を行う

			//numはポーズナンバー
			//0が起立、1が着座、2が横たわる
			num = UnityEngine.Random.Range (0, 3);

			//人体が部屋の外に錬成されないよう、部屋のサイズを取得
			Vector3 size = GameObject.Find ("House01").transform.lossyScale;
			float xs = size.x;
			//float ys = size.y;   //部屋の高さをいじりたい場合は使う
			float zs = size.z;

			// 人間を錬成する範囲を指定
			// -5f,4.5fは壁の位置座標の最小と最大、それにスケールを掛けることで位置が算出される
			float xmin = -5f * xs;
			float xmax = 4.5f * xs;
			float zmin = -5f * zs;
			float zmax = 4.5f * zs;

			// 人間を錬成する対位/位置/角度
			// 位置を変えたいときはVector3のxとzの、角度を変えたい時はquaternionのyの値をいじる
			if (num == 0) {
				if (change_position) {
					human = (GameObject)GameObject.Instantiate (BoxUnityChan, new Vector3 (UnityEngine.Random.Range (xmin, xmax), 0, UnityEngine.Random.Range (zmin, zmax)), Quaternion.Euler (0, UnityEngine.Random.Range (0.0f, 359.0f), 0));
				} else if (center_position) {
					human = (GameObject)GameObject.Instantiate (BoxUnityChan, new Vector3 (UnityEngine.Random.Range (-0.1f, 0.1f), 0, UnityEngine.Random.Range (-0.8f, 0.1f)), Quaternion.Euler (0, UnityEngine.Random.Range (0.0f, 359.0f), 0)); 
				} else {
					human = (GameObject)GameObject.Instantiate (BoxUnityChan, new Vector3 (0, 0, 0), Quaternion.Euler (0, UnityEngine.Random.Range (0.0f, 359.0f), 0)); 
				}
			} else if (num == 1) {
				if (change_position) {
					human = (GameObject)GameObject.Instantiate (BoxUnityChan_Lying, new Vector3 (UnityEngine.Random.Range (xmin, xmax), 0, UnityEngine.Random.Range (zmin, zmax)), Quaternion.Euler (270, UnityEngine.Random.Range (0.0f, 359.0f), 0));
				} else if (center_position) {
					human = (GameObject)GameObject.Instantiate (BoxUnityChan_Lying, new Vector3 (UnityEngine.Random.Range (-0.1f, 0.1f), 0, UnityEngine.Random.Range (-0.8f, 0.1f)), Quaternion.Euler (270, UnityEngine.Random.Range (0.0f, 359.0f), 0)); 
				} else {
					human = (GameObject)GameObject.Instantiate (BoxUnityChan_Lying, new Vector3 (0, 0, 0), Quaternion.Euler (270, UnityEngine.Random.Range (0.0f, 359.0f), 0));
				}
			} else if (num == 2) {
				if (change_position) {
					human = (GameObject)GameObject.Instantiate (BoxUnityChan_Sitting, new Vector3 (UnityEngine.Random.Range (xmin, xmax), 0, UnityEngine.Random.Range (zmin, zmax)), Quaternion.Euler (0, UnityEngine.Random.Range (0.0f, 359.0f), 0));
				} else if (center_position) {
					human = (GameObject)GameObject.Instantiate (BoxUnityChan_Sitting, new Vector3 (UnityEngine.Random.Range (-0.1f, 0.1f), 0, UnityEngine.Random.Range (-0.8f, 0.1f)), Quaternion.Euler (0, UnityEngine.Random.Range (0.0f, 359.0f), 0)); 
				} else {
					human = (GameObject)GameObject.Instantiate (BoxUnityChan_Sitting, new Vector3 (0, 0, 0), Quaternion.Euler (0, UnityEngine.Random.Range (0.0f, 359.0f), 0));
				}
			}
			pos = pos_set (xmax, xmin, zmax, zmin, human); //部屋を9分割したポジション
			//human_pos = Get_pos (human); //座標を取得
		}
	}
	/*
	public void GetHumanCoordinate(){
		Vector3 coordinate = human_pos;
		return coordinate;
	}
	*/
	GameObject human;
	int labelcount=0; //デバッグログ用

	//ラベルづけ
	public void GetHumanLabel(){
		GameObject alchemize = GameObject.Find ("Alchemize");
		if (alchemize.GetComponent<SimulationManager> ().isstart) {
			//Debug.Log ("update");
			/* if (Input.GetKey (KeyCode.Alpha1)) { 
			StopCoroutine ("HumanAlchemize");
		} */
			if (num == 0) {
				labelSave ("pose", num);
				labelSave ("position", pos);
			} else if (num == 1){
				labelSave ("pose", num);
				labelSave ("position", pos);
			} else if (num == 2) {
				labelSave ("pose", num);
				labelSave ("position", pos);
			}
			labelcount++;  //デバッグログ用
			Debug.Log ("labelcount"+labelcount);  //デバッグログ用
		}
	}

	/*
	// 人体錬成コルーチン (HumanAlchemize = 人体錬成)
	IEnumerator HumanAlchemize() {
		while (true) {
			
			Vector3 size = GameObject.Find("House01").transform.lossyScale;
			float xs = size.x;
			//float ys = size.y;   //まあ別にいらない
			float zs = size.z;

			// 人間を錬成する範囲
			float xmin = -5f * xs;
			float xmax = 4.5f * xs;
			float zmin = -5f * zs;
			float zmax = 4.5f * zs;

			int num = UnityEngine.Random.Range(0, 3);
			if (num == 0) {
				GameObject human = (GameObject)GameObject.Instantiate (boxunitychan, new Vector3 (UnityEngine.Random.Range (xmin, xmax), 0, UnityEngine.Random.Range (zmin, zmax)), Quaternion.Euler (0, UnityEngine.Random.Range (0.0f, 359.0f), 0));
				int pos = pos_set (xmax,xmin,zmax,zmin,human);
				labelSave ("pose", num);
				labelSave ("position", pos);
				//yield return new WaitForSeconds (0.03f);
				yield return new WaitForEndOfFrame();
				Destroy (human);
			} else if (num == 1) {
				GameObject human = (GameObject)GameObject.Instantiate (BoxUnityChan_Lying, new Vector3 (UnityEngine.Random.Range (xmin, xmax), 0, UnityEngine.Random.Range (zmin, zmax)), Quaternion.Euler (270, UnityEngine.Random.Range (0.0f, 359.0f), 0));
				int pos = pos_set (xmax,xmin,zmax,zmin,human);
				labelSave ("pose", num);
				labelSave ("position", pos);
				//yield return new WaitForSeconds (0.03f);
				yield return new WaitForEndOfFrame();
				Destroy (human);
			} else if (num == 2) {
				GameObject human = (GameObject)GameObject.Instantiate (BoxUnityChan_Sitting, new Vector3 (UnityEngine.Random.Range (xmin, xmax), 0, UnityEngine.Random.Range (zmin, zmax)), Quaternion.Euler (0, UnityEngine.Random.Range (0.0f, 359.0f), 0));
				int pos = pos_set (xmax,xmin,zmax,zmin,human);
				labelSave("pose",num);
				labelSave ("position", pos);
				//yield return new WaitForSeconds (0.03f);
				yield return new WaitForEndOfFrame();
				Destroy (human);
			}
			Debug.Log ("a");
		}
	}
	:*/
 
	void labelSave(string name,float label) {
		//Debug.Log(height);
		StreamWriter sw;
		FileInfo fi;
		fi = new FileInfo(csvname+"_"+name + "_label.csv");
		sw = fi.AppendText();
		//string line = string.Format("{0},{1}",
		//11, 13);
		//クラス分け用(distance)
		int l=(int)label;

		if (name == "pose") {
			l = (int)(label);
		}

		if (name == "position"){
			l = (int)(label);
		}

		/*
		if (name == "x") {
			l = (int)(label * 2);
		} else if (name == "y") {
			l = (int)(label);
		} else if (name == "z") {
			l = (int)(label*2)+9;
		}

		if (name == "distance") {
			l = Mathf.RoundToInt(label * 2) - 2;
			if (l < 0) l = 0;
		} else if (name == "height") {
			l = Mathf.RoundToInt(label * 2) - 2;
			//l = Mathf.RoundToInt(label ) ;
			if (l < 0) l = 0;
		} else if (name == "angle") {
			l = Mathf.RoundToInt(label);
			l = l / 15;
			if (l < 0) l = 0;
			else if (l > 6) l = 6;
		}
		*/
		sw.Write("{0},", l);
		sw.Write(sw.NewLine);
		//sw.Write(line);
		sw.Flush();
		sw.Close();
	}
}