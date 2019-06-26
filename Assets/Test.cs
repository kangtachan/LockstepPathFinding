using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lockstep.Math;
using Debug = Lockstep.Logging.Debug;

public class Test : MonoBehaviour {
    public LVector2 srcPoint;
    public LVector2 dstPoint;
    // Start is called before the first frame update
    void Start(){
        UnityEngine.Debug.Log("srcPoint " + srcPoint + " srcPoint" + dstPoint);
    }
}
