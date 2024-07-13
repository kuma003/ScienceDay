using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

public class LookAt : SynchronizeData
{


    private float distance = 100; // 対象までのオフセット (これを基に回転させる.)

    private float launchrod_time;

    private void Start()
    {
        launchrod_time = DataManager.Instance.events.Find(e => e.name == "liftoff").time;
    }

    public override void Reflesh()
    {
/*        if (this.time == time) return;
        this.time = time;


        FlightData traj = DataManager.Instance.trajectory;
        if (traj.coord.Count == 0) return; // データがない

        // 先頭の時刻の前後のインデックス
        int idx2 = traj.time.FindIndex(t => t > Mathf.Max(time, launchrod_time));
        if (idx2 == -1)
            idx2 = traj.time.Count - 1; // out of index
        int idx1 = (idx2 == 0) ? 0 : idx2 - 1;
        float t = Mathf.InverseLerp(traj.time[idx1], traj.time[idx2], time); // 内挿用パラメタ
*/
        /*Vector3 offset = Vector3.Normalize(Vector3.Cross(Vector3.up, traj.coord[idx2] - traj.coord[idx1])) * distance;
        if (offset.magnitude == 0) offset = Vector3.back * distance;*/

        gameObject.transform.position = _coord + Vector3.back * distance;

        /*gameObject.transform.rotation = Quaternion.AngleAxis(Vector3.SignedAngle(Vector3.forward, -offset, Vector3.up), Vector3.up);*/
    }
}
