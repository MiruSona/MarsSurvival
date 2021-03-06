using UnityEngine;
using System.Collections;

public class LightManager : MonoBehaviour {

    //참조
    private SystemDAO.GlobalState global_state;
    private Transform player;
    private Transform player_light_transform;
    private Light player_light;
    private Light bg_light;

    //빛 세기 관련
    private const float light_power_max = 35.0f;  //빛 최대
    private const float light_power_min = 25.0f;  //빛 최소
    //max - min
    private const float light_power_depth = light_power_max - light_power_min;
    //빛 주기  
    private const float light_power_cycle = light_power_depth / (Define.Day_Cycle - (Define.Day_1D3_Cycle * 2));
    //현재 빛 세기
    private float light_power = light_power_min;

    //배경 빛
    private const float bg_light_max = 35.0f;
    private const float bg_light_min = 0f;
    private const float bg_light_depth = bg_light_max - bg_light_min;
    private const float bg_light_cycle = bg_light_depth / (Define.Day_Cycle - (Define.Day_1D3_Cycle * 2));
    private float bg_light_power = bg_light_min;

    //현재 빛 범위
    private float light_range = Define.light_range_min;

    //초기화
    void Start () {
        global_state = SystemDAO.instance.global_state;
        player = GameObject.FindWithTag("Player").transform;

        player_light_transform = transform.FindChild("PlayerLight");
        player_light = player_light_transform.GetComponent<Light>();
        bg_light = transform.FindChild("BGLight").GetComponent<Light>();
    }

    // 빛 관리
    void Update () {

        //플레이어 빛 위치 변경
        Vector3 player_pos = player.localPosition;
        player_pos.z = -10f;
        player_light_transform.localPosition = player_pos;

        //라이트 조절
        switch (global_state.state)
        {
            case Define.Day:
            case Define.DayStorm:

                //현재 게임 시간
                float game_time = global_state.game_time;

                #region 빛 세기 조절
                //하루의 0/3 ~ 1/3 까지 밝아짐
                if (0 <= game_time && game_time < Define.Day_1D3_Cycle)
                {
                    //빛 세기 구하기
                    light_power = light_power_min + game_time * light_power_cycle;
                    //빛 범위 구하기
                    light_range = Define.light_range_min + game_time * Define.light_range_cycle;

                    //플레이어 빛
                    player_light.range = light_power;
                    player_light.spotAngle = light_range;

                    //배경 빛
                    bg_light_power = bg_light_min + game_time * bg_light_cycle;
                    bg_light.range = bg_light_power;
                }

                //하루의 1/3 ~ 2/3 까지 최대치
                if (Define.Day_1D3_Cycle <= game_time && game_time <= (Define.Day_Cycle - Define.Day_1D3_Cycle))
                {
                    player_light.range = light_power_max;
                    player_light.spotAngle = Define.light_range_max;

                    bg_light.range = bg_light_max;
                }

                //하루의 2/3 ~ 3/3 까지 어두워짐
                if ((Define.Day_Cycle - Define.Day_1D3_Cycle) < game_time && game_time <= Define.Day_Cycle)
                {
                    //빛 세기 구하기
                    light_power = light_power_min + ((Define.Day_Cycle - game_time) * light_power_cycle);
                    //빛 범위 구하기
                    light_range = Define.light_range_min + ((Define.Day_Cycle - game_time) * Define.light_range_cycle);

                    player_light.range = light_power;
                    player_light.spotAngle = light_range;

                    //배경 빛
                    bg_light_power = bg_light_min + ((Define.Day_Cycle - game_time) * bg_light_cycle);
                    bg_light.range = bg_light_power;
                }
                #endregion

                break;

            case Define.Night:
            case Define.NightStorm:

                //빛 세기 최소로
                player_light.range = light_power_min;
                player_light.spotAngle = Define.light_range_min;
                bg_light.range = bg_light_min;

                break;
        }
    }
}
