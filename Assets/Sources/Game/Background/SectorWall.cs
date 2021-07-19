using UnityEngine;
using System.Collections;

public class SectorWall : MonoBehaviour {

    //참조
    private SystemDAO.GlobalState global_state;

    //구역
    public int sector = 0;

    //초기화
	void Start () {
        global_state = SystemDAO.instance.global_state;
    }
	
	//자기 자신 구역과 현재 레벨 확인
	void Update () {
        if (sector <= global_state.level)
            gameObject.SetActive(false);
	}
}
