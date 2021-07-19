using UnityEngine;
using System.Collections;

public class BuildSlot : MonoBehaviour {

    //참조
    private GameManager game_manager;
    private GameDAO.PlayerData player_data;
    private GameObject player_recognize;

    //슬롯
    private Transform slot1_trans;
    private Transform slot2_trans;
    private Vector2 slot1_size = Vector2.zero;
    private Vector2 slot2_size = Vector2.zero;

    //데이터
    private GameDAO.SubstanceData build_need;
    private GameDAO.BuildData.Pool pool;

    //오프셋
    private float offset_x = 0f;
    private float offset_y = 0f;

    //초기화
    void Start ()
    {
        game_manager = GameManager.instance;
        player_data = GameDAO.instance.player_data;
        player_recognize = Player.instance.transform.FindChild("Player_Recognize").gameObject;

        slot1_trans = transform.FindChild("SlotCanvas").FindChild("Slot1");
        slot2_trans = transform.FindChild("SlotCanvas").FindChild("Slot2");

        Canvas canvas = transform.FindChild("SlotCanvas").GetComponent<Canvas>();
        slot1_size = new Vector2(canvas.scaleFactor, canvas.scaleFactor);
        slot2_size = new Vector2(canvas.scaleFactor, canvas.scaleFactor);
    }
    
    //초기화
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
        if (CheckSlot(slot1_trans.position, slot1_size))
            return;

        //빌드 시 필요량의 자원이 있는지 확인
        if (player_data.my_subdata.CheckMore(build_need))
        {
            //있다면 빌드!
            spawn_bool = game_manager.SpawnBuild(pool, slot1_trans, offset_x, offset_y);

            //자원 빼기
            if (spawn_bool)
                player_data.my_subdata.SubAll(build_need);
        }

        //Player_Recognize 활성화
        player_recognize.SetActive(true);

        //슬롯 비활성화
        gameObject.SetActive(false);
    }

    public void BuildSlot2()
    {
        bool spawn_bool = false;
        
        //해당위치에 무언가 있다면 실행X
        if (CheckSlot(slot2_trans.position, slot2_size))
            return;

        //빌드 시 필요량의 자원이 있는지 확인
        if (player_data.my_subdata.CheckMore(build_need))
        {
            //있다면 빌드!
            spawn_bool = game_manager.SpawnBuild(pool, slot2_trans, offset_x, offset_y);

            //자원 빼기
            if (spawn_bool)
                player_data.my_subdata.SubAll(build_need);
        }

        //Player_Recognize 활성화
        player_recognize.SetActive(true);

        //슬롯 비활성화
        gameObject.SetActive(false);
    }

    //빌드 취소
    public void ExitBuild()
    {
        //Player_Recognize 활성화
        player_recognize.SetActive(true);

        gameObject.SetActive(false);
    }

    //위치 체크
    public bool CheckSlot(Vector2 _position, Vector2 _size)
    {
        //보낼 bool - 기본 false(해당 위치에 아무것도 없음)
        bool send_bool = false;

        //레이 캐스팅
        Ray2D ray = new Ray2D(_position, Vector2.zero);
        RaycastHit2D[] hit = Physics2D.BoxCastAll(ray.origin, _size, 0f, ray.direction);

        //해당위치에 무언가 있다면 true
        for(int i = 0; i < hit.Length; i++)
        {
            if (hit[i].collider != null)
            {
                if (hit[i].collider.CompareTag("Build"))
                    send_bool = true;
            }
        }

        return send_bool;
    }
}
