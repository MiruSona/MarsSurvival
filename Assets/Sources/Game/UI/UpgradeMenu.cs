using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UpgradeMenu : MonoBehaviour {

    //참조
    private GameDAO.PlayerData player_data;
    private GameDAO.ToolData tool_data;
    private GameDAO.WeaponData weapon_data;
    private GameDAO.TurretData turret_data;
    private GameDAO.ShieldData shield_data;
    private GameDAO.SpaceShipData spaceship_data;

    private Weapon weapon;
    private Tool tool;

    //업그레이드 UI
    private struct UpgradeUI
    {
        public Button btn;
        public Image main_img, sub_img;
        public Text level, metal, bio;
        public Image repair_need;

        public UpgradeUI(Button _btn, Image _main_img, Image _sub_img, Text _level, Text _metal, Text _bio, Image _repair_need)
        {
            btn = _btn;
            main_img = _main_img; sub_img = _sub_img;
            level = _level; metal = _metal; bio = _bio;
            repair_need = _repair_need;
        }
    }

    private UpgradeUI weapon_ui;
    private Sprite[,] progress_sprite = new Sprite[Define.weapon_step_max, Define.weapon_level_max];

    private UpgradeUI tool_ui;

    private UpgradeUI turret_ui;

    private UpgradeUI shield_ui;

    //초기화
    void Awake () {
        //참조
        player_data = GameDAO.instance.player_data;
        tool_data = player_data.tool_data;
        weapon_data = player_data.weapon_data;
        turret_data = GameDAO.instance.turret_data;
        shield_data = GameDAO.instance.shield_data;
        spaceship_data = GameDAO.instance.spaceship_data;

        weapon = Player.instance.weapon.GetComponent<Weapon>();
        tool = Player.instance.tool.GetComponent<Tool>();

        //UI 초기화
        weapon_ui =
            new UpgradeUI(
                    LoadButtonComponent("Weapon"),
                    LoadImageComponent("Weapon"),
                    LoadImageComponent("Weapon", "Progress"),
                    LoadTextComponent("Weapon", "Level"),
                    LoadTextComponent("Weapon", "MetalText"),
                    null,
                    LoadImageComponent("Weapon", "UpgradeStop")
                );
        for (int i = 0; i < Define.weapon_step_max; i++)
            for(int j = 0; j < Define.weapon_level_max; j++)
                progress_sprite[i,j] = Resources.Load<Sprite>("Sprite/Game/UI/Progress_" + i + "_" + j);

        tool_ui =
            new UpgradeUI(
                    LoadButtonComponent("Tool"),
                    LoadImageComponent("Tool"),
                    null,
                    LoadTextComponent("Tool", "Level"),
                    LoadTextComponent("Tool", "MetalText"),
                    LoadTextComponent("Tool", "BioText"),
                    LoadImageComponent("Tool", "UpgradeStop")
                );

        turret_ui =
            new UpgradeUI(
                    LoadButtonComponent("Turret"),
                    LoadImageComponent("Turret"),
                    null,
                    LoadTextComponent("Turret", "Level"),
                    LoadTextComponent("Turret", "MetalText"),
                    null,
                    LoadImageComponent("Turret", "UpgradeStop")
                );

        shield_ui =
            new UpgradeUI(
                    LoadButtonComponent("Shield"),
                    LoadImageComponent("Shield"),
                    null,
                    LoadTextComponent("Shield", "Level"),
                    LoadTextComponent("Shield", "MetalText"),
                    LoadTextComponent("Shield", "BioText"),
                    LoadImageComponent("Shield", "UpgradeStop")
                );
    }

    //UI 갱신
    void OnEnable()
    {
        //UI 초기화
        SetUpgradeUIAll();
    }
    
    //일시정지 풀기
    public void Resume()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }

    #region 업그레이드

    //무기 업글
    public void WeaponUpgrade()
    {
        //업글 다하면 실행 X
        if (weapon_data.step == Define.weapon_step_max - 1 && weapon_data.level == Define.weapon_level_max - 1)
            return;

        int step = weapon_data.step;
        int level = weapon_data.level;

        //플레이어가 가진 자원이 요구량 보다 많으면 업글!
        if (player_data.my_subdata.CheckMore(weapon_data.upgrade_need[step, level]))
        {
            //자원--
            player_data.my_subdata.SubAll(weapon_data.upgrade_need[step, level]);

            //업글!
            if (weapon_data.level == Define.weapon_level_max - 1)
            {
                weapon_data.step++;
                weapon_data.level = 0;
            }
            else
            {
                weapon_data.level++;
            }

            //UI text 변화
            SetUpgradeUIAll();

            //외형 변화
            weapon.UpdateShape();
        }
    }

    //도구 업글
    public void ToolUpgrade()
    {
        //업글 다하면 실행 X
        if (player_data.tool_data.step == Define.tool_step_max - 1)
            return;

        int step = tool_data.step;

        //플레이어가 가진 자원이 요구량 보다 많으면 업글!
        if (player_data.my_subdata.CheckMore(tool_data.upgrade_need[step]))
        {
            //자원--
            player_data.my_subdata.SubAll(tool_data.upgrade_need[step]);

            //업글!
            tool_data.step++;

            //UI text 변화
            SetUpgradeUIAll();

            //외형 변화
            tool.UpdateShape();
        }
    }

    //터렛 업글
    public void TurretUpgrade()
    {
        //업글 다하면 실행 X
        if (turret_data.step == Define.build_step_max - 1)
            return;

        int step = turret_data.step;

        //플레이어가 가진 자원이 요구량 보다 많으면 업글!
        if (player_data.my_subdata.CheckMore(turret_data.upgrade_need[step]))
        {
            //자원--
            player_data.my_subdata.SubAll(turret_data.upgrade_need[step]);

            //업글!
            turret_data.step++;

            //UI text 변화
            SetUpgradeUIAll();
        }
    }

    //실드 업글
    public void ShieldUpgrade()
    {
        //업글 다하면 실행 X
        if (shield_data.step == Define.build_step_max - 1)
            return;

        int step = shield_data.step;

        //플레이어가 가진 자원이 요구량 보다 많으면 업글!
        if (player_data.my_subdata.CheckMore(shield_data.upgrade_need[step]))
        {
            //자원--
            player_data.my_subdata.SubAll(shield_data.upgrade_need[step]);

            //업글!
            shield_data.step++;

            //UI text 변화
            SetUpgradeUIAll();
        }
    }
    #endregion

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

    //무기
    private void SetUpgradeUI(UpgradeUI _ui, GameDAO.WeaponData _weapon_data)
    {
        //이미지
        _ui.main_img.sprite = _weapon_data.sprites[_weapon_data.step];
        _ui.sub_img.sprite = progress_sprite[_weapon_data.step, _weapon_data.level];

        //레벨
        _ui.level.text = "LV . " + (_weapon_data.step + 1);

        //필요자원
        GameDAO.SubstanceData need_sub = _weapon_data.upgrade_need[_weapon_data.step, _weapon_data.level];
        if (!(_weapon_data.step == Define.weapon_step_max - 1 && _weapon_data.level == Define.weapon_level_max - 1))
        {
            _ui.metal.text = "x" + need_sub.metal;

            if (player_data.my_subdata.CheckMore(need_sub))
                _ui.metal.color = Color.white;
            else
                _ui.metal.color = Color.red;
        }
        else
        {
            _ui.metal.text = "Max";

            _ui.metal.color = Color.white;
        }

        //자원 있고 풀이 있으면 true / 아님 false
        if (player_data.my_subdata.CheckMore(need_sub))
            _ui.btn.interactable = true;
        else
            _ui.btn.interactable = false;

        //업그레이드 체크
        CheckWeaponStep();
    }

    //도구
    private void SetUpgradeUI(UpgradeUI _ui, GameDAO.ToolData _tool_data)
    {
        //이미지
        _ui.main_img.sprite = _tool_data.sprites[_tool_data.step];

        //레벨
        _ui.level.text = "LV . " + (_tool_data.step + 1);

        //필요자원
        GameDAO.SubstanceData need_sub = _tool_data.upgrade_need[_tool_data.step];
        GameDAO.SubstanceData metal_sub = new GameDAO.SubstanceData(need_sub.metal, 0, 0);
        GameDAO.SubstanceData bio_sub = new GameDAO.SubstanceData(0, need_sub.bio, 0);
        if (_tool_data.step < Define.tool_step_max - 1)
        {
            _ui.metal.text = "x" + need_sub.metal;
            _ui.bio.text = "x" + need_sub.bio;

            if (player_data.my_subdata.CheckMore(metal_sub))
                _ui.metal.color = Color.white;
            else
                _ui.metal.color = Color.red;

            if (player_data.my_subdata.CheckMore(bio_sub))
                _ui.bio.color = Color.white;
            else
                _ui.bio.color = Color.red;
        }
        else
        {
            _ui.metal.text = "Max";
            _ui.bio.text = "Max";

            _ui.metal.color = Color.white;
            _ui.bio.color = Color.white;
        }

        //자원 있고 풀이 있으면 true / 아님 false
        if (player_data.my_subdata.CheckMore(need_sub))
            _ui.btn.interactable = true;
        else
            _ui.btn.interactable = false;

        //업그레이드 체크
        CheckToolStep();
    }

    //빌드
    private void SetUpgradeUI(UpgradeUI _ui, GameDAO.BuildData _build_data)
    {
        //이미지
        _ui.main_img.sprite = _build_data.sprites[_build_data.step];

        //레벨
        _ui.level.text = "LV . " + (_build_data.step + 1);

        //필요자원
        GameDAO.SubstanceData need_sub = _build_data.upgrade_need[_build_data.step];
        GameDAO.SubstanceData metal_sub = new GameDAO.SubstanceData(need_sub.metal, 0, 0);
        GameDAO.SubstanceData bio_sub = new GameDAO.SubstanceData(0, need_sub.bio, 0);
        if (_build_data.step < Define.build_step_max - 1)
        {
            if(_ui.metal != null)
            {
                _ui.metal.text = "x" + need_sub.metal;
                if(player_data.my_subdata.CheckMore(metal_sub))
                    _ui.metal.color = Color.white;
                else
                    _ui.metal.color = Color.red;
            }

            if(_ui.bio != null)
            {
                _ui.bio.text = "x" + need_sub.bio;
                if (player_data.my_subdata.CheckMore(bio_sub))
                    _ui.bio.color = Color.white;
                else
                    _ui.bio.color = Color.red;
            }
        }
        else
        {
            if (_ui.metal != null)
            {
                _ui.metal.text = "Max";
                _ui.metal.color = Color.white;
            }
               
            if (_ui.bio != null)
            {
                _ui.bio.text = "Max";
                _ui.bio.color = Color.white;
            }
        }

        //자원 있고 풀이 있으면 true / 아님 false
        if (player_data.my_subdata.CheckMore(need_sub))
            _ui.btn.interactable = true;
        else
            _ui.btn.interactable = false;

        //업그레이드 체크
        CheckBuildStep(_ui, _build_data);
    }

    //전체
    private void SetUpgradeUIAll()
    {
        SetUpgradeUI(weapon_ui, weapon_data);
        SetUpgradeUI(tool_ui, tool_data);
        SetUpgradeUI(turret_ui, turret_data);
        SetUpgradeUI(shield_ui, shield_data);
    }

    #endregion

    #region 도구 & 무기 체크
    
    //무기 체크
    private void CheckWeaponStep()
    {
        //글씨 기본 false
        weapon_ui.repair_need.gameObject.SetActive(false);

        //레벨 0 일때
        if (weapon_data.level == 0)
        {
            //수리단계 2단계 아래고 무기 단계 0이면 업글X
            if(spaceship_data.step < 1 && weapon_data.step == 1)
            {
                weapon_ui.btn.interactable = false;
                weapon_ui.repair_need.gameObject.SetActive(true);
            }
            //수리단계 4단계 아래고 무기 단계 1이면 업글X
            if (spaceship_data.step < 3 && weapon_data.step == 2)
            {
                weapon_ui.btn.interactable = false;
                weapon_ui.repair_need.gameObject.SetActive(true);
            }
        }
    }

    //도구 체크
    private void CheckToolStep()
    {
        //글씨 기본 false
        tool_ui.repair_need.gameObject.SetActive(false);

        //수리단계 2단계 아래고 도구 단계 0이면 업글X
        if (spaceship_data.step < 1 && tool_data.step == 0)
        {
            tool_ui.btn.interactable = false;
            tool_ui.repair_need.gameObject.SetActive(true);
        }
        //수리단계 4단계 아래고 도구 단계 1이면 업글X
        if (spaceship_data.step < 3 && tool_data.step == 1)
        {
            tool_ui.btn.interactable = false;
            tool_ui.repair_need.gameObject.SetActive(true);
        }
    }

    //빌드 체크
    private void CheckBuildStep(UpgradeUI _ui, GameDAO.BuildData _build_data)
    {
        //글씨 기본 false
        _ui.repair_need.gameObject.SetActive(false);

        //수리단계 2단계 아래고 업글 단계 0이면 업글X
        if (spaceship_data.step < 1 && _build_data.step == 0)
        {
            _ui.btn.interactable = false;
            _ui.repair_need.gameObject.SetActive(true);
        }
        //수리단계 4단계 아래고 업글 단계 1이면 업글X
        if (spaceship_data.step < 3 && _build_data.step == 1)
        {
            _ui.btn.interactable = false;
            _ui.repair_need.gameObject.SetActive(true);
        }
    }
    #endregion

}
