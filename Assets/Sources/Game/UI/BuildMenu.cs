using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BuildMenu : MonoBehaviour {

    //참조
    private SystemDAO.GlobalState global_state;
    private GameManager game_manager;
    private GameDAO.PlayerData player_data;
    private BuildSlot build_slot;

    private GameDAO.TurretData turret_data;
    private GameDAO.ShieldData shield_data;
    private GameDAO.RegeneratorData regenerator_data;
    private GameDAO.MealData meal_data;
    private GameDAO.LenternData lentern_data;

    //이미지 / 텍스트
    private Button turret_btn;
    private Image turret_image;
    private Text[] turret_text = new Text[3];

    private Button shield_btn;
    private Image shield_image;
    private Text[] shield_text = new Text[3];

    private Button regenerator_btn;
    private Image regenerator_image;
    private Text[] regenerator_text = new Text[3];
    
    private Text[] meal_text = new Text[2];

    private Button lentern_btn;
    private Text[] lentern_text = new Text[2];

    //초기화
    void Awake () {
        global_state = SystemDAO.instance.global_state;
        game_manager = GameManager.instance;
        player_data = GameDAO.instance.player_data;
        build_slot = Player.instance.transform.FindChild("BuildSlot").GetComponent<BuildSlot>();

        turret_data = GameDAO.instance.turret_data;
        shield_data = GameDAO.instance.shield_data;
        regenerator_data = GameDAO.instance.regenerator_data;
        meal_data = GameDAO.instance.meal_data;
        lentern_data = GameDAO.instance.lentern_data;

        turret_btn = LoadButtonComponent("Turret");
        turret_image = LoadImageComponent("Turret");
        turret_text = LoadText3Component("Turret");

        shield_btn = LoadButtonComponent("Shield");
        shield_image = LoadImageComponent("Shield");
        shield_text = LoadText3Component("Shield");

        regenerator_btn = LoadButtonComponent("Regenerator");
        regenerator_image = LoadImageComponent("Regenerator");
        regenerator_text = LoadText3Component("Regenerator");
        
        meal_text = LoadText2Component("Meal");

        lentern_btn = LoadButtonComponent("Lentern");
        lentern_text = LoadText2Component("Lentern");
    }

    //Active시
    void OnEnable()
    {
        //UI 변경
        SetUIBuild(turret_image, turret_text, turret_data);
        SetUIBuild(shield_image, shield_text, shield_data);
        SetUIBuild(regenerator_image, regenerator_text, regenerator_data);

        SetUIConsume(meal_text, meal_data);
        SetUIConsume(lentern_text, lentern_data);

        //풀이 비어있는지 체크
        if (game_manager.turret_empty)
            turret_btn.interactable = false;
        else
            turret_btn.interactable = true;

        if (game_manager.shield_empty)
            shield_btn.interactable = false;
        else
            shield_btn.interactable = true;

        if (game_manager.regenerator_empty)
            regenerator_btn.interactable = false;
        else
            regenerator_btn.interactable = true;

        //랜턴 처리
        switch (global_state.state)
        {
            case Define.Day:
            case Define.DayStorm:
                lentern_btn.interactable = false;
                break;

            case Define.Night:
            case Define.NightStorm:
                lentern_btn.interactable = true;
                break;
        }
    }

    //일시정지 풀기
    public void Resume()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }

    //터렛 빌드 선택 시
    public void ChooseTurret()
    {
        int turret_step = turret_data.step;

        //빌드 시 필요량의 자원이 있는지 확인
        if (player_data.my_subdata.CheckMore(turret_data.build_need[turret_step]))
        {
            //슬롯 선택 활성화
            build_slot.Init(turret_data.build_need[turret_step], GameDAO.BuildData.Pool.Turret, 0f, 0f);
            Resume();
        }
    }

    //실드 빌드 선택 시
    public void ChooseShield()
    {
        int shield_step = shield_data.step;

        //빌드 시 필요량의 자원이 있는지 확인
        if (player_data.my_subdata.CheckMore(shield_data.build_need[shield_step]))
        {
            //슬롯 선택 활성화
            build_slot.Init(shield_data.build_need[shield_step], GameDAO.BuildData.Pool.Shield, 0f, 1f);
            Resume();
        }
    }

    //회복기 빌드 선택 시
    public void ChooseRegenerator()
    {
        int regenerator_step = regenerator_data.step;

        //빌드 시 필요량의 자원이 있는지 확인
        if (player_data.my_subdata.CheckMore(regenerator_data.build_need[regenerator_step]))
        {
            //슬롯 선택 활성화
            build_slot.Init(regenerator_data.build_need[regenerator_step], GameDAO.BuildData.Pool.Regenerator, 0f, 0f);
            Resume();
        }
    }

    //식량 선택 시
    public void ChooseMeal()
    {
        //HP꽉차있다면 실행 X
        if (player_data.hp == player_data.max_hp)
            return;

        //사용 시 필요량의 자원이 있는지 확인
        if (player_data.my_subdata.CheckMore(meal_data.buy_need))
        {
            //회복
            player_data.AddHP(meal_data.recovery);
            //자원 소모
            player_data.my_subdata.SubAll(meal_data.buy_need);
            Resume();
        }
    }

    //랜턴 선택 시
    public void ChooseLentern()
    {
        if (global_state.state == Define.Day || global_state.state == Define.DayStorm)
            return;

        //사용 시 필요량의 자원이 있는지 확인
        if (player_data.my_subdata.CheckMore(lentern_data.buy_need))
        {
            //시야 ON
            lentern_data.light_on = true;

            //자원 소모
            player_data.my_subdata.SubAll(lentern_data.buy_need);
            Resume();
        }
    }

    #region 공용

    //UI 가져오기
    private Button LoadButtonComponent(string _name)
    {
        Button send_btn = null;

        send_btn = transform.FindChild(_name).GetComponent<Button>();

        return send_btn;
    }

    private Image LoadImageComponent(string _name)
    {
        Image send_Image = null;

        send_Image = transform.FindChild(_name).FindChild("MainImg").GetComponent<Image>();

        return send_Image;
    }

    private Image LoadImageComponent(string _name, string _sub_name)
    {
        Image send_Image = null;

        send_Image = transform.FindChild(_name).FindChild(_sub_name).GetComponent<Image>();

        return send_Image;
    }

    private Text[] LoadText3Component(string _name)
    {
        Text[] send_text = new Text[3];

        send_text[0] = transform.FindChild(_name).FindChild("Level").GetComponent<Text>();
        send_text[1] = transform.FindChild(_name).FindChild("MetalText").GetComponent<Text>();
        send_text[2] = transform.FindChild(_name).FindChild("BioText").GetComponent<Text>();

        return send_text;
    }

    private Text[] LoadText2Component(string _name)
    {
        Text[] send_text = new Text[2];

        send_text[0] = transform.FindChild(_name).FindChild("MetalText").GetComponent<Text>();
        send_text[1] = transform.FindChild(_name).FindChild("BioText").GetComponent<Text>();

        return send_text;
    }

    #endregion

    #region UI변경

    //빌드
    private void SetUIBuild(Image _image, Text[] _text, GameDAO.BuildData _build_data)
    {
        //이미지
        _image.sprite = _build_data.sprites[_build_data.step];

        //레벨
        _text[0].text = "LV . " + (_build_data.step + 1);

        //필요자원
        _text[1].text = "x" + _build_data.build_need[_build_data.step].metal;
        _text[2].text = "x" + _build_data.build_need[_build_data.step].bio;
    }

    //소모
    private void SetUIConsume(Text[] _text, GameDAO.ConsumeData _consume_data)
    {
        //필요자원
        _text[0].text = "x" + _consume_data.buy_need.metal;
        _text[1].text = "x" + _consume_data.buy_need.bio;
    }

    #endregion
}
