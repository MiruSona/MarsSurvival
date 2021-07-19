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

    //어떤 대사를 할지
    private enum TalkKind
    {
        Player,
        OpenSector,
        Tip
    }
    private TalkKind talk_kind = TalkKind.Player;

    // 평소 대사
    private string[] sentences = null;

    //팁
    private string[] tips = null;

    //섹터 오픈 시 하는 대사
    private string[] open_sentences = new string[2]
    {
        "모험 영역이 확장되었습니다.",
        "모험 영역이 확장되었습니다."
    };
    
    //초기화
    void Start() {
        //대사 가져오기
        TextAsset txt_file = Resources.Load<TextAsset>("Etc/PlayerSentences");
        sentences = txt_file.text.Split('\n');
        txt_file = Resources.Load<TextAsset>("Etc/Tips");
        tips = txt_file.text.Split('\n');

        player_data = GameDAO.instance.player_data;
        player_talk = transform.FindChild("TalkCanvas").FindChild("TalkText").GetComponent<Text>();

        global_state = SystemDAO.instance.global_state;
        gm_data = SystemDAO.instance.gm_data;

        init_pos = player_talk.transform.localPosition;
    }

    void FixedUpdate()
    {
        //플레이 상태 아니면 실행X
        if (gm_data.state != SystemDAO.GameManagerData.State.isPlay)
            return;

        //좌우 반전 방지
        player_talk.rectTransform.localScale = new Vector3(player_data.facing, 1, 1);

        //시간에 따라 대화 다르게 초기화
        float game_time = global_state.game_time;
        int day_num = global_state.day_num;

        if (game_time < 0.1f)
        {
            //낮 시작부분에는 팁 Or 섹터 오픈
            talk_timer.timer = talk_timer.term;
            if (day_num == Define.Secotr1_Open_Day || day_num == Define.Secotr2_Open_Day)
                Init(TalkKind.OpenSector);
            else if (day_num > 1)
                Init(TalkKind.Tip);
            else
                Init(TalkKind.Player);
        }
        else if (game_time < Define.Day_Cycle)
        {
            //잡소리
            if (!player_talk.gameObject.activeSelf)
                Init(TalkKind.Player);
        }
        else if (game_time < Define.Day_Cycle + 0.1f)
        {
            //밤 시작부분에는 팁
            talk_timer.timer = talk_timer.term;
            Init(TalkKind.Tip);
        }
        else
        {
            //잡소리
            if (!player_talk.gameObject.activeSelf)
                Init(TalkKind.Player);
        }
        
        //타이머 체크
        if (talk_timer.timer < talk_timer.term)
            talk_timer.timer += Time.deltaTime;
        else
        {
            //false -> true
            if (!player_talk.gameObject.activeSelf)
                player_talk.gameObject.SetActive(true);
            //이미 true면 말하기 실행
            else
                Talking();
        }
    }

    //초기화
    private void Init(TalkKind _talk_kind)
    {
        //어떤 말이냐에 따라 초기화
        switch (_talk_kind)
        {
            case TalkKind.Player:
                player_talk.color = new Color(1f, 1f, 0, alpha_add_value);
                player_talk.text = sentences[Random.Range(0, sentences.Length)];
                break;

            case TalkKind.OpenSector:
                player_talk.color = new Color(1f, 0, 0, alpha_add_value);
                player_talk.text = open_sentences[global_state.level - 1];
                break;

            case TalkKind.Tip:
                player_talk.color = new Color(0.5f, 1f, 0, alpha_add_value);
                player_talk.text = tips[(global_state.day_num - 1) % tips.Length];
                break;
        }
        //위치 초기화
        player_talk.transform.localPosition = init_pos;
        alpha_add = true;
    }

    //말하기
    private void Talking()
    {
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
