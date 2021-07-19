using UnityEngine;
using System.Collections;

public class BGEffect : MonoBehaviour {

    //참조
    private SystemDAO.GlobalState global_state;
    private Transform[] ChildrenTransform = new Transform[4];

    //각도 변경 주기
    private float angle_cycle = 0f;     //각도

    // 초기화
    void Start()
    {
        global_state = SystemDAO.instance.global_state;

        ChildrenTransform[0] = transform.FindChild("Sun1").transform;
        ChildrenTransform[1] = transform.FindChild("Sun2").transform;
        ChildrenTransform[2] = transform.FindChild("Earth1").transform;
        ChildrenTransform[3] = transform.FindChild("Earth2").transform;
        
        //주기 구하기
        angle_cycle = 360.0f / Define.DayCount_Cycle / 2.0f;
    }

    void Update()
    {

        //돌아가도록
        Vector3 time_angle = new Vector3(0, 0, global_state.real_time * angle_cycle - 45f);
        transform.localEulerAngles = -time_angle;

        time_angle.z += 30f;

        for (int i = 0; i < ChildrenTransform.Length; i++)
            ChildrenTransform[i].localEulerAngles = time_angle;
    }
}
