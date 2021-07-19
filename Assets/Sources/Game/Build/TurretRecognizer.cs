using UnityEngine;
using System.Collections;

public class TurretRecognizer : MonoBehaviour {

    private GameDAO.BuildData turret_data;

    //초기화
    void Start()
    {
        turret_data = GetComponentInParent<Turret>().my_data;
    }

    void OnTriggerStay2D(Collider2D _col)
    {
        //몬스터가 들어오면
        if (_col.CompareTag("Monster"))
            turret_data.movement = GameDAO.BuildData.Movement.isAttack;

        //보스가 들어오면
        if (_col.CompareTag("Boss"))
            turret_data.movement = GameDAO.BuildData.Movement.isAttack;

        //외계 보스가 들어오면
        if (_col.CompareTag("SPBoss"))
            turret_data.movement = GameDAO.BuildData.Movement.isAttack;
    }

    void OnTriggerExit2D(Collider2D _col)
    {
        //몬스터가 벗어났으면
        if (_col.CompareTag("Monster"))
            turret_data.movement = GameDAO.BuildData.Movement.isReady;

        //보스가 들어오면
        if (_col.CompareTag("Boss"))
            turret_data.movement = GameDAO.BuildData.Movement.isReady;

        //외계 보스가 들어오면
        if (_col.CompareTag("SPBoss"))
            turret_data.movement = GameDAO.BuildData.Movement.isReady;
    }
}
