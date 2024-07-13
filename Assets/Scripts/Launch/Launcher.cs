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

    [SerializeField] private GameObject camera = null;
    [SerializeField] private GameObject rocket = null;
    [SerializeField] private GameObject lines = null;

    private float _Time = 0;
    public float time
    {
        private set { _Time = value; }
        get { return _Time; }
    }
    private float timeMax = 0;

    public float timeRate = 1f; // ${timeRate}”{‘¬

    private LaunchState _LaunchState;
    public LaunchState launchState
    {
        private set { _LaunchState = value; }
        get { return _LaunchState; }
    }



    // Start is called before the first frame update
    void Start()
    {
        if (rocket == null || rocket.GetComponent<Rocket>() == null) return;
        timeMax = (DataManager.Instance.trajectory.time.Count > 0) ?
            DataManager.Instance.trajectory.time.Last() : 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (rocket == null || rocket.GetComponent<Rocket>() == null) return;

        // ‘Å‚¿ã‚°’†
        switch(_LaunchState)
        {
            case LaunchState.prepare:
                time = -5;
                if (Input.GetKeyDown(KeyCode.Return))
                    launchState = LaunchState.launch;
                break;
            case LaunchState.launch:
                time += timeRate * Time.deltaTime;
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

        camera.GetComponent<LookAt>().SetTime(time);
        rocket.GetComponent<RocketController>().SetTime(time);
        lines.GetComponent<TrajectoryDrawer>().SetTime(time);

    }
}
