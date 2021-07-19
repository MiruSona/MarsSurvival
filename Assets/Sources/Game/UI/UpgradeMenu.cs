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
    private GameDAO.RegeneratorData regenerator_data;

    private Weapon weapon;
    private Tool tool;

    //이미지 / 텍스트
    private Image weapon_image;
    private Image weapon_progress;
    private Sprite[] progress_sprite = new Sprite[Define.weapon_level_max];
    private Text[] weapon_text = new Text[3];

    private Image tool_image;
    private Text[] tool_text = new Text[3];

    private Image turret_image;
    private Text[] turret_text = new Text[3];

    private Image shield_image;
    private Text[] shield_text = new Text[3];

    private Image regenerator_image;
    private Text[] regenerator_text = new Text[3];

    //초기화
    void Start () {
        //참조
        player_data = GameDAO.instance.player_data;
        tool_data = player_data.tool_data;
        weapon_data = player_data.weapon_data;
        turret_data = GameDAO.instance.turret_data;
        shield_data = GameDAO.instance.shield_data;
        regenerator_data = GameDAO.instance.regenerator_data;

        weapon = Player.instance.weapon.GetComponent<Weapon>();
        tool = Player.instance.tool.GetComponent<Tool>();

        //받아오기
        weapon_image = LoadImageComponent("Weapon");
        weapon_text = LoadTextComponent("Weapon");
        weapon_progress = LoadImageComponent("Weapon", "Progress");
        for (int i = 0; i < Define.weapon_level_max; i++)
            progress_sprite[i] = Resources.Load<Sprite>("Sprite/Game/UI/Progress_" + i);

        tool_image = LoadImageComponent("Tool");
        tool_text = LoadTextComponent("Tool");

        turret_image = LoadImageComponent("Turret");
        turret_text = LoadTextComponent("Turret");

        shield_image = LoadImageComponent("Shield");
        shield_text = LoadTextComponent("Shield");

        regenerator_image = LoadImageComponent("Regenerator");
        regenerator_text = LoadTextComponent("Regenerator");

        //UI 초기화
        SetUI(weapon_image, weapon_progress, weapon_text, weapon_data);
        SetUI(tool_image, tool_text, tool_data);
        SetUI(turret_image, turret_text, turret_data);
        SetUI(shield_image, shield_text, shield_data);
        SetUI(regenerator_image, regenerator_text, regenerator_data);
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
        //업글 다하면 실행 X - 테스트
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

            //UI text 변화 - 테스트
            SetUI(weapon_image, weapon_progress, weapon_text, weapon_data);

            //외형 변화
            weapon.UpdateShape();
        }
    }

    //도구 업글
    public void ToolUpgrade()
    {
        //업글 다하면 실행 X - 테스트
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

            //UI text 변화 - 테스트
            SetUI(tool_image, tool_text, tool_data);

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

            //UI text 변화 - 테스트
            SetUI(turret_image, turret_text, turret_data);
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

            //UI text 변화 - 테스트
            SetUI(shield_image, shield_text, shield_data);
        }
    }

    //회복기 업글
    public void RegeneratorUpgrade()
    {
        //업글 다하면 실행 X
        if (regenerator_data.step == Define.build_step_max - 1)
            return;

        int step = regenerator_data.step;

        //플레이어가 가진 자원이 요구량 보다 많으면 업글!
        if (player_data.my_subdata.CheckMore(regenerator_data.upgrade_need[step]))
        {
            //자원--
            player_data.my_subdata.SubAll(regenerator_data.upgrade_need[step]);

            //업글!
            regenerator_data.step++;

            //UI text 변화 - 테스트
            SetUI(regenerator_image, regenerator_text, regenerator_data);
        }
    }
    #endregion

    #region 공용

    //UI 가져오기
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

    private Text[] LoadTextComponent(string _name)
    {
        Text[] send_text = new Text[3];

        send_text[0] = transform.FindChild(_name).FindChild("Level").GetComponent<Text>();
        send_text[1] = transform.FindChild(_name).FindChild("MetalText").GetComponent<Text>();
        send_text[2] = transform.FindChild(_name).FindChild("BioText").GetComponent<Text>();

        return send_text;
    }

    #endregion

    #region UI변경

    //무기
    private void SetUI(Image _image, Image _progress, Text[] _text, GameDAO.WeaponData _weapon_data)
    {
        //이미지
        _image.sprite = _weapon_data.sprites[_weapon_data.step];
        _progress.sprite = progress_sprite[_weapon_data.level];

        //레벨
        _text[0].text = "LV . " + (_weapon_data.step + 1);

        //필요자원

        if (!(_weapon_data.step == Define.weapon_step_max - 1 && _weapon_data.level == Define.weapon_level_max - 1))
        {
            _text[1].text = "x" + _weapon_data.upgrade_need[_weapon_data.step, _weapon_data.level].metal;
            _text[2].text = "x" + _weapon_data.upgrade_need[_weapon_data.step, _weapon_data.level].bio;
        }
        else
        {
            _text[1].text = "xM";
            _text[2].text = "xM";
        }
    }

    //도구
    private void SetUI(Image _image, Text[] _text, GameDAO.ToolData _tool_data)
    {
        //이미지
        _image.sprite = _tool_data.sprites[_tool_data.step];

        //레벨
        _text[0].text = "LV . " + (_tool_data.step + 1);

        //필요자원
        if (_tool_data.step < Define.tool_step_max - 1)
        {
            _text[1].text = "x" + _tool_data.upgrade_need[_tool_data.step].metal;
            _text[2].text = "x" + _tool_data.upgrade_need[_tool_data.step].bio;
        }
        else
        {
            _text[1].text = "xM";
            _text[2].text = "xM";
        }
    }

    //빌드
    private void SetUI(Image _image, Text[] _text, GameDAO.BuildData _build_data)
    {
        //이미지
        _image.sprite = _build_data.sprites[_build_data.step];

        //레벨
        _text[0].text = "LV . " + (_build_data.step + 1);

        //필요자원
        if (_build_data.step < Define.build_step_max - 1)
        {
            _text[1].text = "x" + _build_data.upgrade_need[_build_data.step].metal;
            _text[2].text = "x" + _build_data.upgrade_need[_build_data.step].bio;
        }
        else
        {
            _text[1].text = "xM";
            _text[2].text = "xM";
        }
    } 
    
    #endregion

}
