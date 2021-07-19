using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        //주인공에게 닿을 시
        if (col.CompareTag("Build"))
        {
            Debug.Log("trigger_enter");
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
            //주인공에게 닿을 시
            if (col.CompareTag("Build"))
            {
                Debug.Log("trigger_exit");
            }
     }

}
