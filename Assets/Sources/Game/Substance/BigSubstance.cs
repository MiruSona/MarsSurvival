using UnityEngine;
using System.Collections;

public class BigSubstance : MonoBehaviour {

    //데이터
    public int index = 0;
    [System.NonSerialized]
    public GameDAO.SubstanceData my_data = new GameDAO.SubstanceData(0,0,0);
    private GameDAO.SubstanceData init_data = new GameDAO.SubstanceData(200,200,0);

    //참조
    private GameDAO.PlayerData player_data;
    private SystemDAO.MapManager map_manager;
    private GameManager game_manager;

    //스프라이트
    private SpriteRenderer sprite_renderer;
    private Sprite[] sprites = new Sprite[9];
    public float sprite_height = 0f;    

    //자원 관련 데이터
    private int total_sub = 0;  //총 수량
    private int left_sub = 0;   //남은 수량

    //초기화
    void Awake () {
        player_data = GameDAO.instance.player_data;
        map_manager = SystemDAO.instance.map_manager;
        game_manager = GameManager.instance;

        sprite_renderer = GetComponent<SpriteRenderer>();
        sprite_height = Define.ground_position + ((sprite_renderer.sprite.bounds.size.y / 2) * transform.localScale.y);

        //스프라이트 초기화
        for (int i = 0; i < sprites.Length; i++)
            sprites[i] = Resources.Load<Sprite>("Sprite/Game/Substance/Big_Tree_Metal_" + (sprites.Length - i - 1));

        //전체 수량 초기화
        total_sub = init_data.bio + init_data.metal;
        
        //내 자원 초기화
        my_data.CopyAll(init_data);

        //남은 자원 초기화
        left_sub = my_data.metal + my_data.bio;

        //초기화 후 비활성화
        gameObject.SetActive(false);
    }
    
	void Update () {

        //스프라이트 길이
        float sprite_index = (sprites.Length - 1.0f) * ((float)left_sub / (float)total_sub);

        //스프라이트 변화
        sprite_renderer.sprite = sprites[(int)sprite_index];

        //남은 자원 있나 체크
        if (my_data.CheckZero())
        {
            player_data.target_bigsub = null;
            player_data.movement = GameDAO.PlayerData.Movement.isReady;
            DestroyMe();
        }

        //화면 밖으로 나갔나 체크
        if (map_manager.CheckDisableBigSubstance(index))
        {
            gameObject.SetActive(false);
        }
    }

    //리젠
    public void Regen()
    {
        //자원 초기화
        my_data.CopyAll(init_data);

        //남은 자원 초기화
        left_sub = my_data.metal + my_data.bio;

        //위치 초기화
        transform.position = map_manager.GetBigSubstaceRealPoint(index, sprite_height);

        //스프라이트 초기화
        sprite_renderer.sprite = sprites[sprites.Length - 1];
    }

    //데이터 로드
    public void LoadRegen()
    {
        //위치 초기화
        transform.position = map_manager.GetBigSubstaceRealPoint(index, sprite_height);

        //남은 자원
        left_sub = my_data.metal + my_data.bio;
    }

    //파괴
    public void DestroyMe()
    {
        //위치 변경
        if (transform.localPosition.x >= 0)
            transform.localPosition = Define.init_pos_plus;
        else
            transform.localPosition = Define.init_pos_minus;

        //가상 위치 제거
        map_manager.DestroyBigSubstancePoint(index);
        //게임메니저에서 제거
        game_manager.DestroyBigSubstance(gameObject);

        //엑티브 false로
        gameObject.SetActive(false);
    }
    
    //자원 채취
    public GameDAO.SubstanceData GetSubstance(int _amount)
    {
        //보낼 데이터
        GameDAO.SubstanceData send_data = new GameDAO.SubstanceData(_amount, _amount, 0);
        
        //내 자원에서 보낼 데이터 만큼 빼준다.
        my_data.SubAll(send_data);

        left_sub = my_data.metal + my_data.bio;

        return send_data;
    }
}
