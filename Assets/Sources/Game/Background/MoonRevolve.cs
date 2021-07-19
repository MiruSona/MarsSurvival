using UnityEngine;
using System.Collections;

public class MoonRevolve : MonoBehaviour {
    
    //참조
    private Transform moon_transform;

    //각도 주기
    private float angle_cycle = 0f;

    //초기화
    void Start () {
        moon_transform = transform.FindChild("Moon");

        angle_cycle = 360.0f / Define.DayCount_Cycle * 2.0f;
    }
	
	// 회전
	void Update () {
        //돌아가도록
        Vector3 time_angle = new Vector3(0, Time.time * angle_cycle, 0);
        transform.localEulerAngles = time_angle;

        moon_transform.localEulerAngles = -time_angle;
    }
}
