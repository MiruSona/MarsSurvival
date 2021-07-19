using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ValueLog : MonoBehaviour {
    
	void Start () {
	
	}

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        //x , y , width , height
        Rect rect1 = new Rect(10, 60, w, h * 2 / 100);
        Rect rect2 = new Rect(10, 90, w, h * 2 / 100);

        //정렬
        style.alignment = TextAnchor.UpperLeft;
        //폰트 크기
        style.fontSize = h * 4 / 100;
        //폰트 색
        style.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        //내용
        string text1 = "point : " + SystemDAO.instance.file_manager.build_data.build_points[0];
        string text2 = "step : " + SystemDAO.instance.file_manager.build_data.turret_step;

        //라벨
        GUI.Label(rect1, text1, style);
        GUI.Label(rect2, text2, style);
    }
}
