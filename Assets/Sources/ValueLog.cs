using UnityEngine;
using System.Collections.Generic;

public class ValueLog : SingleTon<ValueLog> {

    private List<string> contents = new List<string>();
    private List<Rect> position = new List<Rect>();
    private float init_pos_x = 10f;
    private float init_pos_y = 60f;
    private float pos_y_interval = 40f;
    private GUIStyle style = new GUIStyle();

    void Start () {
        //정렬
        style.alignment = TextAnchor.UpperLeft;
        //폰트 크기
        style.fontSize = Screen.height * 4 / 100;
        //폰트 색
        style.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    void OnGUI()
    {
        //라벨
        for(int i = 0; i < contents.ToArray().Length; i++)
            GUI.Label(position[i], contents[i], style);        
    }

    public void ShowLog(string _log)
    {
        //내용 추가
        contents.Add(_log);

        //위치 추가
        int length = contents.ToArray().Length;
        //처음으로 넣는 거일 경우
        if (length == 1)
            position.Add(new Rect(init_pos_x, init_pos_y, Screen.width, Screen.height * 2 / 100));
        //아닐 경우
        else
            position.Add(new Rect(init_pos_x, init_pos_y + (pos_y_interval * (length - 1)), Screen.width, Screen.height * 2 / 100));
    }

    public void ClearLog()
    {
        contents.Clear();
        position.Clear();
    }
}
