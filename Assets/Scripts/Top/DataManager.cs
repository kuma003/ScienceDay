using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public struct FlightData
{
    public List<float> time; // 時間
    public List<Vector3> coord; // 座標
    public List<float> vel; // 速度
    public List<float> zenith; // 天頂角 (= 90 - 仰角)
    public List<float> azimuth; // 方位角 (北を0)
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
    public float time;  // 時間
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
    public Vector3[] finPoints; // フィンの座標。OpenRocketに基づき配列の順序は固定、詳細は仕様書を参照。
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
    public List<Event> events = new List<Event>();
    public Profile profile = new Profile();
    public RocketStructure rocket = new RocketStructure();

    public string filepath = "settings.json"; // 保存先のファイルパス

    [SerializeField]private SaveData _saveData = new SaveData(); // 保存データ

    public SaveData saveData
    {
        get { return _saveData; }
        // set { _saveData = value; }
    }


    private static DataManager instance;


    public static DataManager Instance
    {
        get
        {
            if (null == instance)
            {
                instance = (DataManager)FindFirstObjectByType(typeof(DataManager));
                if (null == instance)
                {
                    Debug.Log("Data Manager Instance Error");
                }

            }
            return instance;
        }
    }


    // シーン間でインスタンスのオブジェクトを1つにするようにする
    void Awake()
    {
        GameObject[] obj = GameObject.FindGameObjectsWithTag("DataManager");
        if (1 < obj.Length)
        {
            // 重複している場合は削除
            Destroy(gameObject);
        }
        else
        {
            // シーン遷移では破棄されないようにする
            DontDestroyOnLoad(gameObject);
        }

        if (File.Exists(filepath))
        {
            _saveData = Load(filepath);
        }
        else
        {
            Save(new SaveData());
        }
    }


    void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        StreamWriter writer = new StreamWriter(filepath, false);
        writer.Write(json);
        writer.Close();
    }

    SaveData Load(string path)
    {
        StreamReader reader = new StreamReader(path);
        string json = reader.ReadToEnd();
        reader.Close();

        return JsonUtility.FromJson<SaveData>(json);
    }


}