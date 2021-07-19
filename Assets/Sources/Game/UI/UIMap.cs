using UnityEngine;
using System.Collections;

public class UIMap : MonoBehaviour {

    //참조
    private SystemDAO.MapManager map_manager;
    private SystemDAO.GlobalState global_state;
    private RectTransform arrow_transform;
    private GameObject[] sector_sign = new GameObject[Define.sector_total_num - 1];

    //기준값
    private float max_pos = 180.0f / Define.virtual_point.max;

    //초기화
    void Start () {
        map_manager = SystemDAO.instance.map_manager;
        global_state = SystemDAO.instance.global_state;
        arrow_transform = transform.FindChild("MapArrow").GetComponent<RectTransform>();
        for(int i = 0; i < sector_sign.Length; i++)
            sector_sign[i] = transform.Find("MapSign" + (i + 1)).gameObject;
    }
	
	void Update () {
        //자기 위치 표시
        arrow_transform.anchoredPosition = new Vector2(map_manager.player_point * max_pos, 0);

        //미니맵에서 지우기
        switch (global_state.level)
        {
            case 2:
                sector_sign[0].SetActive(false);
                sector_sign[1].SetActive(false);
                break;
            case 1:
                sector_sign[0].SetActive(false);
                break;
        }
    }
}
