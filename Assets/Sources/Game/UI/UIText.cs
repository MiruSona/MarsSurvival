using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIText : MonoBehaviour {

    //참조
    private GameDAO.PlayerData player_data;
    private SystemDAO.GlobalState global_state;
    private Text text;

    //UI 표시 종류
    public enum Kind
    {
        Metal,
        Bio,
        Crystal,
        Day
    }
    public Kind kind = Kind.Metal;

    //초기화
    void Start () {
        player_data = GameDAO.instance.player_data;
        global_state = SystemDAO.instance.global_state;
        text = GetComponent<Text>();
    }

    //표시
    void Update()
    {
        switch (kind)
        {
            case Kind.Metal:
                text.text = "" + player_data.my_subdata.metal;
                break;

            case Kind.Bio:
                text.text = "" + player_data.my_subdata.bio;
                break;

            case Kind.Crystal:
                text.text = "" + player_data.my_subdata.crystal;
                break;

            case Kind.Day:
                text.text = "DAY " + global_state.day_num;
                break;
        }
    }
}
