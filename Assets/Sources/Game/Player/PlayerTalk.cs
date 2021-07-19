using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerTalk : MonoBehaviour {

    //참조
    private Text player_talk;
    private GameDAO.PlayerData player_data;
    private SystemDAO.GlobalState global_state;
    private SystemDAO.GameManagerData gm_data;

    //타이머
    private GameDAO.Timer talk_timer = new GameDAO.Timer(0f, Define.Day_1D3_Cycle);
    private GameDAO.Timer stay_timer = new GameDAO.Timer(0f, 3f);

    //알파값 관련
    private bool alpha_add = true;

    //delta 값(위치 / 알파값)
    private const float alpha_add_value = 0.02f;
    private const float pos_add_value = 0.008f;

    //초기화 위치
    private Vector3 init_pos = Vector3.zero;

    //대사 여부
    private bool sector1_open = false;
    private bool sector2_open = false;

    //평소 대사
    private string[] sentences = new string[19] {
        "I wanna go home.",
        "CHICKEN!!",
        "I can endure this time, \nI can do all things, \nand I can escape! ",
        "Be positive, \nI'm having a marvelous time, \nthat nobody can experience",
        "Only trees, rocks, and....um... \n\"Monsters\".",
        "Yeah, I see. \nEarth is pretty much...pretty. \nThis place is so desolate.",
        "It's over. I'll gonna die.",
        "If I have a baby in future, \nI'll tell this 'Unbelievable' saga.",
        "*Sniff* \nUrrgh, horrible smells... ",
        "Oh please... help...",
        "Did you hear about the boy \nwhose whole left side was cut off? \nHe's all right now.",
        "Too hot...",
        "Almond is die... \nIt becomes Di-A-mond. *laugh*",
        "Da da dum da dum da da da..",
        "PIZZA!!",
        "I want to watch Tv",
        "What have I done to deserve this?",
        "Darn it.",
        "I hate this planet." };

    //섹터 오픈 시 하는 대사
    private string[] open_sentences = new string[2]
    {
        "The adventure Area1 is expanded",
        "The adventure Area2 is expande"
    };
    
    //초기화
    void Start() {
        player_data = GameDAO.instance.player_data;
        player_talk = transform.FindChild("TalkCanvas").FindChild("TalkText").GetComponent<Text>();

        global_state = SystemDAO.instance.global_state;
        gm_data = SystemDAO.instance.gm_data;

        init_pos = player_talk.transform.localPosition;
    }

    void FixedUpdate() {
        //플레이 상태 아니면 실행X
        if (gm_data.state != SystemDAO.GameManagerData.State.isPlay)
            return;

        //좌우 반전 방지
        player_talk.rectTransform.localScale = new Vector3(player_data.facing, 1, 1);

        //Day 체크
        //이미 날짜가 지났으면 실행X
        if (global_state.day_num > Define.Secotr1_Open_Day)
            sector1_open = true;
        if (global_state.day_num > Define.Secotr2_Open_Day)
            sector2_open = true;

        //구역 오픈시 진행
        switch (global_state.day_num)
        {
            case Define.Secotr1_Open_Day:
                if (!sector1_open)
                {
                    if (!player_talk.gameObject.activeSelf)
                    {
                        player_talk.gameObject.SetActive(true);
                        Init(true);
                    }
                    else
                        Talking();

                    if (!player_talk.IsActive())
                        sector1_open = true;
                }
                break;

            case Define.Secotr2_Open_Day:
                if (!sector2_open)
                {
                    if (!player_talk.gameObject.activeSelf)
                    {
                        player_talk.gameObject.SetActive(true);
                        Init(true);
                    }
                    else
                        Talking();

                    if (!player_talk.IsActive())
                        sector2_open = true;
                }
                break;
        }

        //타이머 체크
        if (talk_timer.timer < talk_timer.term)
            talk_timer.timer += Time.deltaTime;
        else
        {
            //false -> true
            if (!player_talk.gameObject.activeSelf)
            {
                player_talk.gameObject.SetActive(true);
                Init(false);
            }
            else
                Talking();
        }
    }

    //초기화
    private void Init(bool _sector_open)
    {
        if (_sector_open)
        {
            player_talk.color = new Color(1f, 0, 0, alpha_add_value);
            player_talk.text = open_sentences[global_state.level - 1];
        }
        else
        {
            player_talk.color = new Color(1f, 1f, 0, alpha_add_value);
            player_talk.text = "";
        }
        player_talk.transform.localPosition = init_pos;
        alpha_add = true;
    }

    //말하기
    private void Talking()
    {
        //비어있다면 문장 넣어줌
        if (player_talk.text == "")
            player_talk.text = sentences[Random.Range(0, sentences.Length)];

        //알파값 & 위치 조절
        Color color = player_talk.color;

        //알파값이 0보다 작거나 같으면 talk 종료.
        if (color.a < 0f)
        {   
            color.a = 0f;
            player_talk.color = color;
            player_talk.gameObject.SetActive(false);
            talk_timer.timer = 0f;
        } 

        //알파값이 1보다 작은 동안 알파값 및 위치 ++ or --
        if (color.a < 1f)
        {
            if (alpha_add)
            {
                color.a += alpha_add_value;
                player_talk.transform.Translate(new Vector3(0, pos_add_value, 0));
            }
            else
            {
                color.a -= alpha_add_value;
                player_talk.transform.Translate(new Vector3(0, -pos_add_value, 0));
            }
        }
        else
        {
            //잠깐 대기
            if (stay_timer.timer < stay_timer.term)
                stay_timer.timer += Time.deltaTime;
            else
            {
                //대기 시간 끝나면 다시--
                color.a -= alpha_add_value;
                alpha_add = false;
                stay_timer.timer = 0f;
            }
        }

        //색 적용
        player_talk.color = color;
    }
}
