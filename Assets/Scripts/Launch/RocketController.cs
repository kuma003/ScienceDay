using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RocketController : MonoBehaviour
{
    private float time;


    public void SetTime(float time)
    {
        if (this.time == time) return;
        this.time = time;

        FlightData traj = DataManager.Instance.trajectory;
        if (traj.coord.Count == 0) return; // データがない


        // 先頭の時刻の前後のインデックス
        int idx2 = traj.time.FindIndex(t => t > time);
        if (idx2 == -1)
            idx2 = traj.time.Count - 1; // out of index
        int idx1 = (idx2 == 0) ? 0 : idx2 - 1;
        float t = Mathf.InverseLerp(traj.time[idx1], traj.time[idx2], time); // 内挿用パラメタ

        gameObject.transform.position = Vector3.Lerp(traj.coord[idx1], traj.coord[idx2], t);

        gameObject.GetComponent<Rocket>().azimuth = Mathf.Lerp(traj.azimuth[idx1], traj.azimuth[idx2], t);
        gameObject.GetComponent<Rocket>().zenith = Mathf.Lerp(traj.zenith[idx1], traj.zenith[idx2], t);
        Debug.Log(gameObject.GetComponent<Rocket>().azimuth);
    }
}
