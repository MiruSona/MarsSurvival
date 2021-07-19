using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GodMode : SingleTon<GodMode> {

    //참조
    private GameDAO.PlayerData player_data;
    private MonsterCommon monster_common;

    //오브젝트
    public Text atkText;
    public Text toolText;
    public Text regenText;
    public GameObject gModePanel;
    public GameObject upgradePanel;

    //수치 조절
    //플레이어
    public float weapon_atk = 100f;
    public float tool_speed = 1.5f;

    //몬스터
    public float monster_rengen_cycle = 20f;

    // 초기화
    void Start() {
        player_data = GameDAO.instance.player_data;
    }

    public void openGM() {
        gModePanel.SetActive(true);
        upgradePanel.SetActive(false);
        Time.timeScale = 0f;
    }

    public void closdBtn() {
        gModePanel.SetActive(false);
        Time.timeScale = 1f;
    }
    
    public void regenp1() {
        monster_rengen_cycle++;
    }

    public void regenm1() {
        if (monster_rengen_cycle > 0)
            monster_rengen_cycle--;
    }


    public void atkp1() {
            weapon_atk += 1f;
    }

    public void atkp10() {
            weapon_atk += 10f;
    }

    public void atkm1() {
        if (weapon_atk > 0)
            weapon_atk -= 1f;
    }

    public void atkm10() {
        if (weapon_atk > 10)
            weapon_atk -= 10f;
    }


    public void spdp01() {
            tool_speed += 0.1f;
    }

    public void spdp1() {
            tool_speed += 1f;
    }

    public void spdm01() {
        if (tool_speed > 1)
            tool_speed -= 0.1f;
    }

    public void spdm1() {
        if (tool_speed > 1)
            tool_speed -= 1f;
    }

    public void Init() {
        weapon_atk = 100;
        tool_speed = 1.5f;
        monster_rengen_cycle = 20f;
    }

    public void metal100() {
        player_data.my_subdata.metal += 100;
    }

    public void bio100() {
        player_data.my_subdata.bio += 100;
    }
    
    // 업데이트
    void Update() {
        player_data.weapon_data.atk[0, 0] = weapon_atk;
        player_data.tool_data.speed[0] = tool_speed;

        atkText.text = weapon_atk.ToString();
        toolText.text = tool_speed.ToString();
        regenText.text = monster_rengen_cycle.ToString();

        //Define.monster_regen_term = monster_rengen_cycle;
    }

    //수치 조절 함수만들기

}
