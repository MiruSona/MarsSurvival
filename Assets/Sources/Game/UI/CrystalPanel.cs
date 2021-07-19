using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class CrystalPanel : MonoBehaviour {

    GameDAO.PlayerData playerData;
    SystemDAO.FileManager file_manager;

    bool clicked = false;
    string purchaseCheck = "";
    string purchaseResult = "";

    private const int small_value = 3;
    private const int medium_value = 18;
    private const int large_value = 40;

    void OnEnable() {
        playerData = GameDAO.instance.player_data;
        file_manager = SystemDAO.instance.file_manager;
    }

    public void smallBuying() {
        if (playerData.my_subdata.CheckFull(small_value, 2))
            return;

        purchaseCheck = "smallBuying";
        CallAndroid();

        if (purchaseResult == "ok")
        {
            playerData.my_subdata.crystal += small_value;
            file_manager.SaveAll();
        } 
    }
    public void mediumBuying() {
        if (playerData.my_subdata.CheckFull(medium_value, 2))
            return;

        purchaseCheck = "mediumBuying";
        CallAndroid();

        if (purchaseResult == "ok")
        {
            playerData.my_subdata.crystal += medium_value;
            file_manager.SaveAll();
        }
    }
    public void largeBuying() {
        if (playerData.my_subdata.CheckFull(large_value, 2))
            return;

        purchaseCheck = "largeBuying";
        CallAndroid();

        if (purchaseResult == "ok")
        {
            playerData.my_subdata.crystal += large_value;
            file_manager.SaveAll();
        }  
    }

    public void Resume() {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }


    //IAB
    void CallAndroid() {
        if (clicked || Application.platform != RuntimePlatform.Android) {
            //Debug.Log("Do not Execute this method");
            return;
        }
        clicked = true;

#if UNITY_ANDROID
        // try, catch부분이 핵심인데...
        // 간단히 말해서 unity가 Android Class 중 com.unity3d.player.UnityPlayer 를 부른다.
        // 부른 그놈(jc)의 반(class)에가서 '너네 반에 currentActivity 란 애 나와!'라고 외친다.
        // 그때 튀어나온 애를 jo가 기억하게 만든다.
        // 그리고 jo에게 말한다.
        // unity 왈 "야 AnswerToUnity 해봐!"
        try {
            using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
                using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity")) {
                    //Debug.Log("CallAndroid");

                    jo.Call(purchaseCheck);

                }
            }
        } catch (Exception e) {
            //Debug.Log(e.StackTrace);
        }
#endif

    }

    /// <summery>
    /// Android Class가 Unity에 값을 전달 할 때 사용하는 Method
    /// </summery>
    void ResultFromAndroid(string result) {
        clicked = false;
        purchaseResult = result;
    }
}
