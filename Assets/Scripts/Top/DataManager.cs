using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct FlightData
{
    public List<float> time; // ����
    public List<Vector3> coord; // ���W
    public List<float> vel; // ���x
    public List<float> zenith; // �V���p (= 90 - �p)
    public List<float> azimuth; // ���ʊp (����0)
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
    public float time;  // ����
    public string name; // �C�x���g��
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
    // �m�[�Y
    public float noseLength;
    // �{�f�B
    public float bodyLength;
    public float bodyDiameter;
    // �t�B��
    public string finType; // trapezoidal or elliptical
    public int finCount;
    public Vector3[] finPoints; // �t�B���̒��_. OpenRocket�ɏ����Ĕz��̐擪�͗����@��, �����͗����ꕔ���Ƃ��Ď��v���Ɋi�[.
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


    //�V�[���Ԃł��C���X�^���X�̃I�u�W�F�N�g��1�ɂȂ�悤�ɂ���
    void Awake()
    {
        GameObject[] obj = GameObject.FindGameObjectsWithTag("DataManager");
        if (1 < obj.Length)
        {
            // ���ɑ��݂��Ă���Ȃ�폜
            Destroy(gameObject);
        }
        else
        {
            // �V�[���J�ڂł͔j�������Ȃ�
            DontDestroyOnLoad(gameObject);
        }
    }
}
