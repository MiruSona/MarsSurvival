using UnityEngine;
using System.Collections;

public class BGManager : MonoBehaviour {

    //참조
    private SystemDAO.GlobalState global_state;
    private MeshRenderer bg_renderer;

    //밝기 관련
    private const float bg_bright_max = 1f;  //최대
    private const float bg_bright_min = 0f;  //최소
    //max - min
    private const float bg_bright_depth = bg_bright_max - bg_bright_min;
    //밝기 주기  
    private const float bg_bright_cycle = bg_bright_depth / (Define.Day_Cycle - (Define.Day_1D3_Cycle * 2));
    //현재 밝기
    private float bg_bright = bg_bright_min;

    //초기화
    void Start () {
        global_state = SystemDAO.instance.global_state;
        bg_renderer = transform.FindChild("Background").GetComponent<MeshRenderer>();
    }
	
	void Update () {
        //라이트 조절
        switch (global_state.state)
        {
            case Define.Day:
            case Define.DayStorm:
                
                //현재 게임 시간
                float game_time = global_state.game_time;

                #region 밝기 조절
                //하루의 0/3 ~ 1/3 까지 밝아짐
                if (0 <= game_time && game_time < Define.Day_1D3_Cycle)
                {
                    //밝기 구하기
                    bg_bright = bg_bright_min + game_time * bg_bright_cycle;
                    
                    bg_renderer.material.color = new Color(bg_bright, bg_bright, bg_bright);
                }

                //하루의 1/3 ~ 2/3 까지 최대치
                if (Define.Day_1D3_Cycle <= game_time && game_time <= (Define.Day_Cycle - Define.Day_1D3_Cycle))
                {
                    bg_renderer.material.color = new Color(bg_bright_max, bg_bright_max, bg_bright_max);
                }

                //하루의 2/3 ~ 3/3 까지 어두워짐
                if ((Define.Day_Cycle - Define.Day_1D3_Cycle) < game_time && game_time <= Define.Day_Cycle)
                {
                    //밝기 구하기
                    bg_bright = bg_bright_min + ((Define.Day_Cycle - game_time) * bg_bright_cycle);
                    
                    bg_renderer.material.color = new Color(bg_bright, bg_bright, bg_bright);
                }
                #endregion

                break;

            case Define.Night:
            case Define.NightStorm:

                //밝기 최소로
                bg_renderer.material.color = new Color(bg_bright_min, bg_bright_min, bg_bright_min);
                break;
        }

    }
}
