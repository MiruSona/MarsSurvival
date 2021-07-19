using UnityEngine;
using System.Collections;

public class UIMap : MonoBehaviour {

    //참조
    private SystemDAO.MapManager map_manager;
    private RectTransform rect_transform;

    //기준값
    private float max_pos = 180.0f / Define.virtual_point.max;

    //초기화
    void Start () {
        map_manager = SystemDAO.instance.map_manager;
        rect_transform = GetComponent<RectTransform>();
    }
	
	void Update () {
        rect_transform.anchoredPosition = new Vector2(map_manager.player_point * max_pos, 0);
    }
}
