using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShadowRotateEarth : MonoBehaviour {

    //각도 변경 주기
    private float angle_cycle = 0f;     //각도
    private float time = 0f;

    private Vector3 time_angle;

    private bool fliped = false;
    private float plus = 120f;
    // Use this for initialization
    void Start() {
        angle_cycle = 360.0f / Define.DayCount_Cycle / 2.0f;
        //GetComponent<Image>().CrossFadeAlpha(0f, 0f, false);
        transform.localEulerAngles += new Vector3(0, 0, 30f);
    }

    // Update is called once per frame
    void Update() {
        time += Time.deltaTime;
        time_angle = new Vector3(0, 0, time * angle_cycle + plus);
        //time_angle = new Vector3(0, 0, time * angle_cycle);
        gameObject.transform.localEulerAngles = -time_angle;

        if (transform.localEulerAngles.z < 180 & !fliped) {
            plus += 180f;
            fliped = true;
        }
        if (transform.localEulerAngles.z < 245) {
            GetComponent<Image>().CrossFadeAlpha(0f, 1f, false);
        } else if (transform.localEulerAngles.z < 330 & transform.localEulerAngles.z > 245) {
            GetComponent<Image>().CrossFadeAlpha(1f, 1f, false);
            fliped = false;
        }
    }
}
