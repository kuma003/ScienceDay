using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SynchronizeData : MonoBehaviour
{
    public static float _time { get; private set; }
    protected static Vector3 _coord;
    protected static float _vel;
    protected static float _azimuth;
    protected static float _zenith;

    public static bool SetTime(float time)
    {
        var traj = DataManager.Instance.trajectory;

        if (traj.coord.Count == 0) return false;

        if (_time == time) return false;
        _time = time;

        // 先頭の時刻の前後のインデックス
        int idx2 = traj.time.FindIndex(t => t > time);
        if (idx2 == -1)
            idx2 = traj.time.Count - 1; // out of index
        int idx1 = (idx2 == 0) ? 0 : idx2 - 1;
        float t = Mathf.InverseLerp(traj.time[idx1], traj.time[idx2], time); // 内挿用パラメタ

        _coord = Vector3.Lerp(traj.coord[idx1], traj.coord[idx2], t);
        _vel = Mathf.Lerp(traj.vel[idx1], traj.vel[idx2], t);
        _azimuth = Mathf.Lerp(traj.azimuth[idx1], traj.azimuth[idx2], t);
        _zenith = Mathf.Lerp(traj.zenith[idx1], traj.zenith[idx2], t);

        return true;
    }
    public abstract void Reflesh();
}
