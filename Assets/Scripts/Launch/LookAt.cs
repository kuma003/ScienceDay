using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{


    private float distance = 100; // �Ώۂ܂ł̃I�t�Z�b�g (�������ɉ�]������.)

    private float time;

    private float launchrod_time;

    private void Start()
    {
        launchrod_time = DataManager.Instance.events.Find(e => e.name == "liftoff").time;
    }

    public void SetTime(float time)
    {
        if (this.time == time) return;
        this.time = time;


        FlightData traj = DataManager.Instance.trajectory;
        if (traj.coord.Count == 0) return; // �f�[�^���Ȃ�

        // �擪�̎����̑O��̃C���f�b�N�X
        int idx2 = traj.time.FindIndex(t => t > Mathf.Max(time, launchrod_time));
        if (idx2 == -1)
            idx2 = traj.time.Count - 1; // out of index
        int idx1 = (idx2 == 0) ? 0 : idx2 - 1;
        float t = Mathf.InverseLerp(traj.time[idx1], traj.time[idx2], time); // ���}�p�p�����^

        Vector3 rocketPos = Vector3.Lerp(traj.coord[idx1], traj.coord[idx2], t);
        /*Vector3 offset = Vector3.Normalize(Vector3.Cross(Vector3.up, traj.coord[idx2] - traj.coord[idx1])) * distance;
        if (offset.magnitude == 0) offset = Vector3.back * distance;*/

        gameObject.transform.position = rocketPos + Vector3.back * distance;

        /*gameObject.transform.rotation = Quaternion.AngleAxis(Vector3.SignedAngle(Vector3.forward, -offset, Vector3.up), Vector3.up);*/
    }
}