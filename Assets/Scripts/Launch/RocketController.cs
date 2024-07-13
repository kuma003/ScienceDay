using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RocketController : SynchronizeData
{
    public override void Reflesh()
    {
        gameObject.transform.position = _coord;

        gameObject.GetComponent<Rocket>().azimuth = _azimuth;
        gameObject.GetComponent<Rocket>().zenith =_zenith;
    }
}
