using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BuildMenu : MonoBehaviour {

    //참조
    private GameManager game_manager;
    private SystemDAO.GameManagerData gm_data;
    private GameDAO.PlayerData player_data;
    private GameObject player_recognize;
    private BuildSlot build_slot;

    //빌드
    private GameDAO.TurretData turret_data;
    private GameDAO.ShieldData shield_data;
    private GameDAO.MineData mine_data;

    //소비
    private GameDAO.MealData meal_data;
    private GameDAO.WarpData warp_data;

    private Animator player_anim;

    //빌드 UI
    private struct BuildUI
    {
        public Button btn;
        public Image img;
        public Text level, metal, bio;
        public Image limit;

        public BuildUI(Button _btn, Image _img, Text _level, Text _metal, Text _bio, Image _limit)
        {
            btn = _btn; img = _img;
            level = _level; metal = _metal; bio = _bio; limit = _limit;
        }
    }

    private BuildUI turret_ui;
    private BuildUI shield_ui;
    private BuildUI mine_ui;

    //소모품 UI
    private struct ConsumeUI
    {
        public Button btn;
        public Text text;

        public ConsumeUI(Button _btn, Text _text)
        {
            btn = _btn;
            text = _text;
        }
    }

    private ConsumeUI meal_ui;
    private ConsumeUI warp_ui;

    //초기화
    void Awake () {
        game_manager = GameManager.instance;
        gm_data = SystemDAO.instance.gm_data;
        player_data = GameDAO.instance.player_data;
        player_recognize = Player.instance.transform.FindChild("Player_Recognize").gameObject;
        build_slot = Player.instance.transform.FindChild("BuildSlot").GetComponent<BuildSlot>();

        turret_data = GameDAO.instance.turret_data;
        shield_data = GameDAO.instance.shield_data;
        mine_data = GameDAO.instance.mine_data;

        meal_data = GameDAO.instance.meal_data;
        warp_data = GameDAO.instance.warp_data;

        player_anim = Player.instance.GetComponent<Animator>();

        //빌드
        turret_ui =
            new BuildUI(
                LoadButtonComponent("Turret"),
                LoadImageComponent("Turret"),
                LoadTextComponent("Turret", "Level"),
                LoadTextComponent("Turret", "MetalText"),
                null,
                LoadImageComponent("Turret", "Limit")
                );

        shield_ui =
            new BuildUI(
                LoadButtonComponent("Shield"),
                LoadImageComponent("Shield"),
                LoadTextComponent("Shield", "Level"),
                LoadTextComponent("Shield", "MetalText"),
                LoadTextComponent("Shield", "BioText"),
                LoadImageComponent("Shield", "Limit")
                );

        mine_ui =
            new BuildUI(
                LoadButtonComponent("Mine"),
                null,
                null,
                LoadTextComponent("Mine", "MetalText"),
                LoadTextComponent("Mine", "BioText"),
                LoadImageComponent("Mine", "Limit")
                );

        //소비
        meal_ui = new ConsumeUI(LoadButtonComponent("Meal"), LoadTextComponent("Meal", "BioText"));
        warp_ui = new ConsumeUI(LoadButtonComponent("Warp"), LoadTextComponent("Warp", "CrystalText"));
    }

    //Active시
    void OnEnable()
    {
        //UI 변경
        SetBuildUI(turret_ui, turret_data, game_manager.turret_empty);
        SetBuildUI(shield_ui, shield_data, game_manager.shield_empty);
        SetBuildUI(mine_ui, mine_data, game_manager.mine_empty);

        SetConsumeUI(meal_ui, meal_data);
        SetConsumeUI(warp_ui, warp_data);
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
            //Player_Recognize 비활성화 및 하던 일 중지
            player_recognize.SetActive(false);
            player_data.movement = GameDAO.PlayerData.Movement.isReady;

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
            //Player_Recognize 비활성화 및 하던 일 중지
            player_recognize.SetActive(false);
            player_data.movement = GameDAO.PlayerData.Movement.isReady;

            //슬롯 선택 활성화
            build_slot.Init(shield_data.build_need[shield_step], GameDAO.BuildData.Pool.Shield, 0f, 1f);
            Resume();
        }
    }

    //지뢰 선택 시
    public void ChooseMine()
    {
        //Player_Recognize 비활성화 및 하던 일 중지
        player_recognize.SetActive(false);
        player_data.movement = GameDAO.PlayerData.Movement.isReady;

        //빌드 시 필요량의 자원이 있는지 확인
        if (player_data.my_subdata.CheckMore(mine_data.build_need[0]))
        {
            //Player_Recognize 비활성화 및 하던 일 중지
            player_recognize.SetActive(false);
            player_data.movement = GameDAO.PlayerData.Movement.isReady;

            //슬롯 선택 활성화
            build_slot.Init(mine_data.build_need[0], GameDAO.BuildData.Pool.Mine, 0f, -0.5f);
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

    //워프 선택 시
    public void ChooseWarp()
    {
        //사용 시 필요량의 자원이 있는지 확인
        if (player_data.my_subdata.CheckMore(warp_data.buy_need))
        {
            gm_data.state = SystemDAO.GameManagerData.State.isPause;
            player_anim.Play("Player_Warp_Start");

            //자원 소모
            player_data.my_subdata.SubAll(warp_data.buy_need);
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

    private Text LoadTextComponent(string _name, string _sub_name)
    {
        Text send_text = null;

        send_text = transform.FindChild(_name).FindChild(_sub_name).GetComponent<Text>();

        return send_text;
    }

    #endregion

    #region UI변경

    //빌드
    private void SetBuildUI(BuildUI _build_ui, GameDAO.BuildData _build_data, bool _empty)
    {
        //참조 변수
        int step = _build_data.step;
        GameDAO.SubstanceData need_sub = _build_data.build_need[step];

        //이미지
        if (_build_ui.img != null)
            _build_ui.img.sprite = _build_data.sprites[_build_data.step];

        //레벨
        if(_build_ui.level != null)
            _build_ui.level.text = "LV . " + (_build_data.step + 1);

        //필요자원
        if(_build_ui.metal != null && need_sub.metal != 0)
        {
            //자원 표시
            _build_ui.metal.text = "x" + need_sub.metal;
            ChangeText(_build_ui.metal, need_sub.metal, 0, 0);
        }
        if (_build_ui.bio != null && need_sub.bio != 0)
        {
            _build_ui.bio.text = "x" + need_sub.bio;
            ChangeText(_build_ui.bio, 0, need_sub.bio, 0);
        }

        //버튼 변경
        ChangeBuildBtn(_build_ui, need_sub, _empty);
    }
    private void ChangeBuildBtn(BuildUI _build_ui, GameDAO.SubstanceData _sub_data, bool _empty)
    {
        //자원 있고 풀이 있으면 true / 아님 false
        if (player_data.my_subdata.CheckMore(_sub_data) && !_empty)
            _build_ui.btn.interactable = true;
        else
            _build_ui.btn.interactable = false;

        //pool이 비어있으면 limit 표시 아니면 표시X
        if (_empty)
            _build_ui.limit.gameObject.SetActive(true);
        else
            _build_ui.limit.gameObject.SetActive(false);
    }
    
    //소모품
    private void SetConsumeUI(ConsumeUI _consume_ui, GameDAO.ConsumeData _consume_data)
    {
        //참조
        GameDAO.SubstanceData need_sub = _consume_data.buy_need;

        //필요자원
        if (_consume_ui.text != null)
        {
            //필요 자원에 따라 표시
            if(need_sub.metal != 0)
            {
                _consume_ui.text.text = "x" + need_sub.metal;
                ChangeText(_consume_ui.text, need_sub.metal, 0, 0);
            }
            if (need_sub.bio != 0)
            {
                _consume_ui.text.text = "x" + need_sub.bio;
                ChangeText(_consume_ui.text, 0, need_sub.bio, 0);
            }
            if (need_sub.crystal != 0)
            {
                _consume_ui.text.text = "x" + need_sub.crystal;
                ChangeText(_consume_ui.text, 0, 0, need_sub.crystal);
            }
        }

        //버튼 변경
        if (player_data.my_subdata.CheckMore(need_sub))
            _consume_ui.btn.interactable = true;
        else
            _consume_ui.btn.interactable = false;
    }

    //공통
    private void ChangeText(Text _text, int _metal, int _bio, int _crystal)
    {
        //빌드 시 필요량의 자원이 있는지 확인
        GameDAO.SubstanceData sub_data = new GameDAO.SubstanceData(_metal, _bio, _crystal);
        if (player_data.my_subdata.CheckMore(sub_data))
            _text.color = Color.white;
        else
            _text.color = Color.red;
    }
    #endregion
}
