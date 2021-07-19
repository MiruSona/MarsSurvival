using UnityEngine;
using System.Collections;

public class PlayerRecognize : MonoBehaviour {

    //참조
    private GameDAO.PlayerData player_data;
    private GameDAO.SpaceShipData spaceship_data;

    //초기화
    void Start() {
        player_data = GameDAO.instance.player_data;
        spaceship_data = GameDAO.instance.spaceship_data;
    }

    //인식 범위 안에 들어온 것들 확인
    void OnTriggerStay2D(Collider2D _col) {
        //대기 상태 / 땅에있을때/ 공격 외에는 모두 실행X
        if (!player_data.grounded || player_data.attacking)
            return;

        if (player_data.movement != GameDAO.PlayerData.Movement.isReady)
        {
            if(player_data.movement != GameDAO.PlayerData.Movement.isAttackSchedule)
                return;
        }  
        
        //자원인지 확인
        if (_col.CompareTag("Substance")) {
            //자원 채취로 상태 변경
            player_data.movement = GameDAO.PlayerData.Movement.isGather;
            //타겟 자원 설정
            player_data.target_substance = _col.GetComponent<Substance>();
            //타겟 위치 설정
            player_data.tool_data.target_position = _col.transform.position;
        }

        //우주선인지 확인
        if (_col.CompareTag("SpaceShip")) {
            //고치기 실행
            if(player_data.my_subdata.CheckMore(spaceship_data.repair_need))
            player_data.movement = GameDAO.PlayerData.Movement.isRepair;

            player_data.tool_data.target_position = _col.transform.position;
        }

    }

    //인식 범위 나간 것들 확인
    void OnTriggerExit2D(Collider2D _col) {
        //공격 중이거나 움직이는 중이면 실행 X
        if (player_data.movement == GameDAO.PlayerData.Movement.isAttackSchedule) 
            return;
        
        if (player_data.movement == GameDAO.PlayerData.Movement.isMoveSchedule) 
            return;
        
        //자원 인지 확인
        if (_col.CompareTag("Substance")) {
            //대기 상태로 변경
            player_data.movement = GameDAO.PlayerData.Movement.isReady;

            //타겟 자원 초기화
            player_data.target_substance = null;
        }

        //우주선인지 확인
        if (_col.CompareTag("SpaceShip")) {
            //고치기 중지
            player_data.movement = GameDAO.PlayerData.Movement.isReady;

            player_data.tool_data.target_position = Vector3.zero;
        }

    }
}
