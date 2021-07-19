using UnityEngine;
using System.Collections;

public class MonsterRecognize : MonoBehaviour {

    //참조
    private GameDAO.MonsterData monster_data;

    //타이머
    private GameDAO.Timer delay_timer = new GameDAO.Timer(0, 3f);

	//초기화
	void Start () {
        monster_data = GetComponentInParent<MonsterCommon>().monster_data;
    }

    void OnTriggerStay2D(Collider2D _col)
    {
        //만약 지뢰면 실행X
        if (_col.CompareTag("Build"))
        {
            if (_col.GetComponent<BuildCommon>().my_data.pool == GameDAO.BuildData.Pool.Mine)
                return;
        }

        //플레이어나 건물에게 닿으면
        if (_col.CompareTag("Player") || _col.CompareTag("Build"))
        {
            if (delay_timer.timer < delay_timer.term)
                delay_timer.timer += Time.fixedDeltaTime;
            else
            {
                //원거리 공격 상태로 변경
                monster_data.movement = GameDAO.MonsterData.Movement.isAttackRange;
            }
        }
    }

    void OnTriggerExit2D(Collider2D _col)
    {
        //플레이어에게서 벗어나면 공격 해제
        if (_col.CompareTag("Player") || _col.CompareTag("Build"))
        {
            monster_data.movement = GameDAO.MonsterData.Movement.isMove;
            delay_timer.timer = 0;
        }
    }
}
