using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RocketController : SynchronizeData
{
    private bool isIgnite = false;
    private AudioSource audioSourceBuff;

    private Rocket rocket;

    public void Start()
    {
        rocket = gameObject.GetComponent<Rocket>();
        audioSourceBuff = gameObject.GetComponent<AudioSource>();
    }

    public void Update()
    {
        if (audioSourceBuff.isPlaying)
        {
            rocket.SetThrust(1f);
        }
        else
        {
            rocket.SetThrust(0);
        }
    }

    public override void Reflesh()
    {
        if (_time >= 0 && !isIgnite)
        {
            print(_time);
            print(!isIgnite);
            isIgnite = true;
            audioSourceBuff.Play();
            Debug.Log("Rocket Ignition");
            
        }
        else if (_time < 0)
        {
            isIgnite = false;
        }



        gameObject.transform.position = _coord;

        gameObject.GetComponent<Rocket>().azimuth = _azimuth;
        gameObject.GetComponent<Rocket>().zenith =_zenith;
    }
}
