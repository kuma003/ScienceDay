using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    public enum LaunchState
    {
        prepare,
        launch,
        pause,
        touchdown,
    }

    [SerializeField] private GameObject[]  syncronizedObjects = new GameObject[0];


    private List<SynchronizeData> synchronizeDataRefs;

    private float _Time = 0;
    public float time
    {
        private set { _Time = value; }
        get { return _Time; }
    }
    private float timeMax = 0;
    private float timeApogee = 0;

    public float timeRate = 1f; // ${timeRate}倍速

    private LaunchState _LaunchState;
    public LaunchState launchState
    {
        private set { _LaunchState = value; }
        get { return _LaunchState; }
    }



    // Start is called before the first frame update
    void Start()
    {
        timeMax = (DataManager.Instance.trajectory.time.Count > 0) ?
            DataManager.Instance.trajectory.time.Last() : 0;
        var idx = DataManager.Instance.events.FindIndex(e => e.name == "apogee");
        timeApogee = (idx > 0) ? DataManager.Instance.events[idx].time : timeMax;
        synchronizeDataRefs = new List<SynchronizeData>();
        for (int i = 0; i < syncronizedObjects.Length; i++)
            if (syncronizedObjects[i] != null)
                synchronizeDataRefs.Add(syncronizedObjects[i].GetComponent<SynchronizeData>());
        launchState = LaunchState.prepare;

        // すべての同期データを初期化
        foreach (var compRef in synchronizeDataRefs) compRef.Reflesh();
    }

    // Update is called once per frame
    void Update()
    {
        if (syncronizedObjects.Length == 0) return;

        // 打ち上げ中
        switch(_LaunchState)
        {
            case LaunchState.prepare:
                time = -5;
                if (Input.GetKeyDown(KeyCode.Return))
                    launchState = LaunchState.launch;
                break;
            case LaunchState.launch:
                time += Time.deltaTime * (time < 0 || time > timeApogee + 1 ? 1 : timeRate); // 打ち上げから頂点1秒後まではゆっくり
                if (time > timeMax)
                    launchState = LaunchState.touchdown;
                if (Input.GetKeyDown(KeyCode.Space)) 
                    launchState = LaunchState.pause;
                break;
            case LaunchState.pause:
                if (Input.GetKeyDown(KeyCode.Space))  
                    launchState = LaunchState.launch;
                break;
            case LaunchState.touchdown:
                if (Input.GetKeyDown(KeyCode.Space))
                    launchState = LaunchState.prepare;
                break;
            default:
                break;
        }


        if (SynchronizeData.SetTime(time))
        {
            foreach (var compRef in synchronizeDataRefs) compRef.Reflesh();
        }
    }
}
