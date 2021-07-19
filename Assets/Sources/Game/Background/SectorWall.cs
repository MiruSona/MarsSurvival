using UnityEngine;
using System.Collections;

public class SectorWall : MonoBehaviour {

    //참조
    private SystemDAO.GlobalState global_state;
    private SpriteRenderer left_renderer;
    private SpriteRenderer right_renderer;

    //구역
    public int sector = 0;

    //알파값
    private Color alpha = Color.white;

    //초기화
	void Start () {
        global_state = SystemDAO.instance.global_state;
        left_renderer = transform.FindChild("Left").GetComponent<SpriteRenderer>();
        right_renderer = transform.FindChild("Right").GetComponent<SpriteRenderer>();
    }
	
	//자기 자신 구역과 현재 레벨 확인
	void Update () {
        switch (sector)
        {
            case 1:
                if (global_state.day_num > Define.Secotr1_Open_Day)
                    gameObject.SetActive(false);

                if(global_state.day_num == Define.Secotr1_Open_Day)
                {
                    if (alpha.a > 0f)
                    {
                        alpha.a -= 0.01f;
                        left_renderer.color = alpha;
                        right_renderer.color = alpha;
                    } else {
                        gameObject.SetActive(false);
                        //if (Social.localUser.authenticated)
                        //    Social.ReportProgress(GPGSSstatics.achievement_where_is_it_going, 100.0f, (bool success) => {
                        //        if (success) {
                        //            Debug.Log("Reported");
                        //        } else {
                        //        }
                        //    });
                    }
                        
                }
                break;

            case 2:
                if (global_state.day_num > Define.Secotr2_Open_Day)
                    gameObject.SetActive(false);

                if (global_state.day_num == Define.Secotr2_Open_Day)
                {
                    if (alpha.a > 0f)
                    {
                        alpha.a -= 0.01f;
                        left_renderer.color = alpha;
                        right_renderer.color = alpha;
                    }
                    else {
                        gameObject.SetActive(false);
                        //if (Social.localUser.authenticated)
                        //    Social.ReportProgress(GPGSSstatics.achievement_far_far_away, 100.0f, (bool success) => {
                        //        if (success) {
                        //            Debug.Log("Reported");
                        //        } else {
                        //        }
                        //    });
                    }
                }
                break;
        }
	}
}
