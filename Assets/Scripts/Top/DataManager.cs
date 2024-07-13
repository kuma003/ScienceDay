using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct FlightData
{
    public List<float> time; // 時刻
    public List<Vector3> coord; // 座標
    public List<float> vel; // 速度
    public List<float> zenith; // 天頂角 (= 90 - 仰角)
    public List<float> azimuth; // 方位角 (東が0)
};
public struct RocketStatus
{
    public Vector3 coord;
    public float vel;
    public float zenith;
    public float azimuth;
}
public struct Event
{
    public float time;  // 時刻
    public string name; // イベント名
};
public struct Profile
{
    public float flight_time;
    public float apogee;
    public float max_vel;
    public float angle;
    public float direction;
    public string rocket_name;
}
public struct RocketStructure
{
    // ノーズ
    public float noseLength;
    // ボディ
    public float bodyLength;
    public float bodyDiameter;
    // フィン
    public string finType; // trapezoidal or elliptical
    public int finCount;
    public Vector3[] finPoints; // フィンの頂点. OpenRocketに準じて配列の先頭は翼根機首側, 末尾は翼根底部側として時計回りに格納.
    public readonly bool Equals(RocketStructure other)
    {
        return (
            this.noseLength == other.noseLength &&
            this.bodyLength == other.bodyLength &&
            this.bodyDiameter == other.bodyDiameter &&
            this.finType == other.finType &&
            this.finCount == other.finCount &&
            this.finPoints == other.finPoints);
    }
}



public class DataManager : MonoBehaviour
{
    public FlightData trajectory = new FlightData() { time = new List<float>(), coord = new List<Vector3>(), vel = new List<float>(), zenith = new List<float>(), azimuth = new List<float>() };
    public List<Event> events = new List<Event> ();
    public Profile profile = new Profile();
    public RocketStructure rocket = new RocketStructure();


    private static DataManager instance;

    
    public static DataManager Instance
    {
        get {
            if (null == instance)
            {
                instance = (DataManager)FindObjectOfType(typeof(DataManager));
                if (null == instance)
                {
                    Debug.Log("Data Manager Instance Error");
                }

            } 
        return instance;
        }
    }


    //シーン間でもインスタンスのオブジェクトが1つになるようにする
    void Awake()
    {
        GameObject[] obj = GameObject.FindGameObjectsWithTag("DataManager");
        if (1 < obj.Length)
        {
            // 既に存在しているなら削除
            Destroy(gameObject);
        }
        else
        {
            // シーン遷移では破棄させない
            DontDestroyOnLoad(gameObject);
        }
    }
}
