using UnityEngine;
using System.Collections;

public class BuildSlot : MonoBehaviour {

    //참조
    private GameManager game_manager;
    private GameDAO.PlayerData player_data;
    private Transform ui_slot1;
    private Transform ui_slot2;
    
    private Transform col_slot1;
    private Transform col_slot2;

    //데이터
    private GameDAO.SubstanceData build_need;
    private GameDAO.BuildData.Pool pool;

    //빌드 가능 여부(다른곳에서 체크)
    [System.NonSerialized]
    public bool slot1_bool = true;
    [System.NonSerialized]
    public bool slot2_bool = true;

    //오프셋
    private float offset_x = 0f;
    private float offset_y = 0f;

    //초기화
    void Start ()
    {
        game_manager = GameManager.instance;
        player_data = GameDAO.instance.player_data;

        ui_slot1 = transform.FindChild("SlotCanvas").FindChild("Slot1");
        ui_slot2 = transform.FindChild("SlotCanvas").FindChild("Slot2");

        col_slot1 = transform.FindChild("SlotCheck").FindChild("Slot1");
        col_slot2 = transform.FindChild("SlotCheck").FindChild("Slot2");
    }

    //컬리더 위치 변화
    void Update()
    {
        col_slot1.transform.position = ui_slot1.position;
        col_slot2.transform.position = ui_slot2.position;
    }

    public void Init(GameDAO.SubstanceData _subdata, GameDAO.BuildData.Pool _pool, float _offset_x, float _offset_y)
    {
        build_need = _subdata;
        pool = _pool;

        offset_x = _offset_x;
        offset_y = _offset_y;

        gameObject.SetActive(true);
    }
	
    //빌드
    public void BuildSlot1()
    {
        bool spawn_bool = false;

        //해당위치에 무언가 있다면 실행X
        if (!slot1_bool)
            return;

        //빌드 시 필요량의 자원이 있는지 확인
        if (player_data.my_subdata.CheckMore(build_need))
        {
            //있다면 빌드!
            spawn_bool = game_manager.SpawnBuild(pool, ui_slot1, offset_x, offset_y);

            //자원 빼기
            if (spawn_bool)
                player_data.my_subdata.SubAll(build_need);
        }

        //슬롯 비활성화
        gameObject.SetActive(false);
    }

    public void BuildSlot2()
    {
        bool spawn_bool = false;

        //해당위치에 무언가 있다면 실행X
        if (!slot2_bool)
            return;

        //빌드 시 필요량의 자원이 있는지 확인
        if (player_data.my_subdata.CheckMore(build_need))
        {
            //있다면 빌드!
            spawn_bool = game_manager.SpawnBuild(pool, ui_slot2, offset_x, offset_y);

            //자원 빼기
            if (spawn_bool)
                player_data.my_subdata.SubAll(build_need);
        }

        //슬롯 비활성화
        gameObject.SetActive(false);
    }

    //빌드 취소
    public void ExitBuild()
    {
        gameObject.SetActive(false);
    }
}
