using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class ray_information : MonoBehaviour {
    public float x;
    public float y;
    public float z;

    public bool is_ramdom;
    public bool is_change_angle;

    public string csvname;
    //need infomation
    float distance;
    float height;
    float angle;
    float vec;
    float person_with_sensor_angle_acquisition;
    Vector3 sensor_angle;
	Vector3 sensor_pos;
    bool is_hitting;
    int log_num;
	int rtem;
    struct ray_struct
    {
        public Ray ray;
        public int num;
        public bool hit;
    }
    ray_struct[] ray = new ray_struct[64];
    
    void Start()
    {
        Application.targetFrameRate = 120;
        ray_set();
        is_hitting = false;
        log_num = 0;
    }



    void ray_set() {
        for (int i = 0; i <= 63; i++)
        {
            Vector3 direction = Vector3.down;
            
            direction = Quaternion.Euler(30f + (-(60f / 7f) * (i % 8)), 0f, 30f + (-(60f / 7f) * ((i) / 8))) * direction;
            direction = Quaternion.Euler(this.gameObject.transform.rotation.eulerAngles) * direction;
            //-60 -
            //if (i%8==0)
            //if(i>=0&&i<8)
                ray[i].ray = new Ray(this.gameObject.transform.position, direction);
            
        }
    }
	//センサ位置ランダム機能
    void Random_sensor()
    {
        Vector3 pos= this.gameObject.transform.position;
        Vector3 ang=this.gameObject.transform.eulerAngles;

        //pos.x = Mathf.RoundToInt(UnityEngine.Random.Range(1.0f + 0.00001f, 4.5f - 0.00001f) *2)/2.0f;
        pos.x = -1.85f +Mathf.RoundToInt(UnityEngine.Random.Range(-x + 0.00001f, x - 0.00001f) * 2) / 2.0f;
        pos.y = 1.4f +Mathf.RoundToInt(UnityEngine.Random.Range(0.0f+0.00001f, y - 0.00001f) *2)/2.0f;
        //pos.y = UnityEngine.Random.Range(0.5f , 3.5f );
        pos.z = 0+Mathf.RoundToInt(UnityEngine.Random.Range(-z + 0.00001f, z- 0.00001f) *2)/2.0f;
        //pos.z = UnityEngine.Random.Range(-4.0f , 4.0f );
        
        //pos.z = Mathf.RoundToInt(UnityEngine.Random.Range(0, 2));
        if (pos.z == 0)
            pos.z = -1;
        ang.x = Mathf.RoundToInt(UnityEngine.Random.Range(0.0f, 90.0f) / 15.0f) * 15;
        //ang.x = Mathf.RoundToInt(UnityEngine.Random.Range(0.0f-14.99999999f, 90.0f + 14.99999999f) / 15.0f) * 15;
        this.gameObject.transform.position = pos;
        if(is_change_angle)this.gameObject.transform.eulerAngles=ang;
    }

	int datacount=0;
	void Update () {
		GameObject alchemize = GameObject.Find ("Alchemize");
		Vector3 pos= this.gameObject.transform.position; //センサの位置
		if (alchemize.GetComponent<SimulationManager> ().isstart) {
			Human human = GameObject.Find ("Alchemize").GetComponent<Human> ();
			human.CreateHuman ();
			Vector3 human_coordinate = human.GetHumanCoordinate ();
			//Debug.Log ("coo"+human_coordinate);
			ray_set ();
			RaycastHit hit;
			is_hitting = false;  
			float min_distance = 6;
			GameObject House01 = GameObject.Find ("House01");  
			rtem = House01.GetComponent <room_temperature>().room_tem;  
			if (is_ramdom)
				Random_sensor ();  
			for (int i = 0; i <= 63; i++) {
				if (Physics.Raycast (ray [i].ray, out hit, 5.0f)) {
					if (hit.transform.gameObject.tag == "human") {
						is_hitting = true;
						ray [i].num = hit_any (ray [i], hit);
						if (min_distance > hit.distance)
							min_distance = hit.distance;
						// ヒットしたオブジェクトのカラーを変更
						//hit.transform.gameObject.GetComponent<Renderer>().material.color = Color.red;
						//Debug.Log(hit.distance);
						//Debug.Log(person_with_sensor_angle_acquisition(this.gameObject.transform.rotation.eulerAngles, ray[i].ray.direction));
						//Debug.Log(Vector3.Angle(this.gameObject.transform.position, ray[i].ray.direction));
						if (this.gameObject.transform.position.z < hit.transform.gameObject.transform.position.z)
							vec = 1;
						else
							vec = 0;
					}
				} else
					//ray [i].num = 12 * 4 + UnityEngine.Random.Range (0, 8); //室温
					ray [i].num = rtem * 4 + UnityEngine.Random.Range (0, 8);
				//ray[i].num = 12 * 4;
				//if(i/8==0)
				Debug.DrawRay (ray [i].ray.origin, ray [i].ray.direction * 5.0f, Color.red);
            
			}
			distance = min_distance;
			height = this.gameObject.transform.position.y;
			angle = this.gameObject.transform.rotation.eulerAngles.x;
			if (is_hitting) {
				logSave ();
				Debug.Log (log_num);
				datacount++;
				Debug.Log ("datacount" + datacount);
				//GameObject.Find ("Alchemize").GetComponent<Human> ().GetHumanLabel ();
				human.GetHumanLabel();
			}
		}
    }
	int hit_any(ray_struct ray,RaycastHit hit)  
	{
        ray.num = hit.transform.gameObject.GetComponent<temperature_information>().temperature + UnityEngine.Random.Range(0, 12);
		return ray.num;
    }
    void angle_infomation(Vector3 hit_pos)
    {
        person_with_sensor_angle_acquisition= Vector3.Angle(this.gameObject.transform.position, hit_pos);
        sensor_angle = this.gameObject.transform.rotation.eulerAngles;
    }
    
    public void logSave()
    {
        StreamWriter sw;
        FileInfo fi;
        //fi = new FileInfo("koteiangle_2m_test6.csv");
        fi = new FileInfo(csvname+".csv");
        sw = fi.AppendText();
        log_num += 1;
        //string line = string.Format("{0},{1}",
        //11, 13);

        //正規化
        float data;
        for (int i = 0; i <= 63; i++)
        {
            //ray[i].num -= (int)(2 * (distance - 1));
            data=(float)ray[i].num / 100f;
            data = (float)ray[i].num ;
            sw.Write("{0},", data);
        }
        sw.Write(sw.NewLine);

        //sw.Write(line);
        sw.Flush();
        sw.Close();

        //--------------------------------------------------
        //ここから先がラベルについて labelSave("ファイル名", 変数);
        //--------------------------------------------------
        //labelSave("leftright", vec);
        //labelSave("distance", distance);
        //labelSave("angle", angle);
        //labelSave("height", height);
        
          
    //  labelSave("x", this.gameObject.transform.position.x);
        //labelSave("y", height);
        //labelSave("z", this.gameObject.transform.position.z);
    }
    
	void labelSave(string name,float label)
    {
        //Debug.Log(height);
        StreamWriter sw;
        FileInfo fi;
        fi = new FileInfo("random_"+name + csvname+".csv");
        sw = fi.AppendText();
        //string line = string.Format("{0},{1}",
        //11, 13);
        //クラス分け用(distance)
        int l=(int)label;
        if (name == "x")
        {
            l = (int)(label * 2);
        }
        else if (name == "y")
        {
            l = (int)(label);
        }
        else if (name == "z")
        {
            l = (int)(label*2)+9;
        }
        if (name == "distance")
        {
            

            l = Mathf.RoundToInt(label * 2) - 2;
            
            if (l < 0) l = 0;
        }
        else if (name == "height")
        {
            l = Mathf.RoundToInt(label * 2) - 2;
            //l = Mathf.RoundToInt(label ) ;
            if (l < 0) l = 0;
        }
        else if (name == "angle")
        {
            l = Mathf.RoundToInt(label);
            l = l / 15;
            if (l < 0) l = 0;
            else if (l > 6) l = 6;
        }
        sw.Write("{0},", l);
        sw.Write(sw.NewLine);
        //sw.Write(line);
        sw.Flush();
        sw.Close();
    }

}
