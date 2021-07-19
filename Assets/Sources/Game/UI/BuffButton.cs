using UnityEngine;
using System.Collections;

public class BuffButton : MonoBehaviour {

    void OnEnable() {
        iTween2.ScaleTo(gameObject, iTween2.Hash("scale", new Vector3(1.2f, 1.2f, 1.0f), "time", 0.6f, "looptype", "pingPong", "easetype", "easeInSine"));
    }
}
