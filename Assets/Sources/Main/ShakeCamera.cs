using UnityEngine;
using System.Collections;

public class ShakeCamera : MonoBehaviour {

    //타이머
    public float timer = 3f;
    public float term = 3f;

    //흔들기 관련
    public float amount = 0.05f;
    public float time = 1f;
    

	void Update () {
        //카메라 흔들기
        if (timer < term)
            timer += Time.deltaTime;
        else
        {
            //카메라 흔들기
            iTween2.ShakePosition(gameObject, new Vector3(amount, amount, 0), time);
            timer = 0;
        }
    }
}
