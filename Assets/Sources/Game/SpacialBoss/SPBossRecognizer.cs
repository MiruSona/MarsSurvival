using UnityEngine;
using System.Collections;

public class SPBossRecognizer : MonoBehaviour {

    //참조
    private SPBossCommon sp_boss;

    //초기화
    void Start()
    {
        sp_boss = GetComponentInParent<SPBossCommon>();
    }

    //공격 인식
    void OnTriggerStay2D(Collider2D _col)
    {
        if (_col.CompareTag("Player") || _col.CompareTag("Build"))
        {
            sp_boss.sp_boss_data.movement = GameDAO.SPBossData.Movement.isRangeAttack;
        }
    }

    //공격 인식
    void OnTriggerExit2D(Collider2D _col)
    {
        if (_col.CompareTag("Player") || _col.CompareTag("Build"))
        {
            sp_boss.sp_boss_data.movement = GameDAO.SPBossData.Movement.isMove;
        }
    }
}
