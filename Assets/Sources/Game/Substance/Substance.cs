using UnityEngine;
using System.Collections;


public class Substance : MonoBehaviour {

    //인덱스
    public int index = 0;
    public int sector = 0;

    //파괴 여부
    public bool destroyed = false;

    //자원 데이터
    public GameDAO.SubstanceData subdata = new GameDAO.SubstanceData();         //실제 남은 자원
    public GameDAO.SubstanceData init_subdata = new GameDAO.SubstanceData();    //초기 자원괎
    private int left_subdata = 0;   //총 남은 자원(크리스탈 제외)

    //참조
    private GameDAO.PlayerData player_data;
    private SystemDAO.MapManager map_manager;
    private SpriteRenderer sprite_renderer;
    //변화될 모습 -> 0 = 처음 / 1 = 중간 / 2 = 끝
    public Sprite[] sprite_frame = new Sprite[3];
    public float sprite_hieght = 0f;

    //자원 종류
    public enum Type
    {
        Rock,
        Tree
    }
    public Type type = Type.Rock;

    //크기
    public enum Size
    {
        Small,
        Medium,
        Large
    }
    public Size size = Size.Small;

    //크기에 따른 자원 값
    private readonly int small_value = 10;
    private readonly int medium_value = 16;      
    private readonly int large_value = 24;
    private int substance_value = 0;    
    
    //빠른 초기화
    void Awake()
    {
        //참조 초기화
        player_data = GameDAO.instance.player_data;
        map_manager = SystemDAO.instance.map_manager;
        sprite_renderer = GetComponent<SpriteRenderer>();
        //랜덤 좌우 반전
        int random = Random.Range(0, 2);
        if (random == 0)
            sprite_renderer.flipX = false;
        else
            sprite_renderer.flipX = true;

        //스프라이트 높이
        sprite_hieght = Define.ground_position + ((sprite_renderer.sprite.bounds.size.y / 2) * transform.localScale.y);

        //초기화 후 비활성화
        gameObject.SetActive(false);
    }

    //초기화
    void Start () {

        //크기에 따른 값 초기화
        switch (size)
        {
            case Size.Small:
                substance_value = small_value;
                break;

            case Size.Medium:
                substance_value = medium_value;
                break;

            case Size.Large:
                substance_value = large_value;
                break;
        }

        //종류에 따른 값 초기화
        switch (type)
        {
            case Type.Rock:
                subdata.metal = substance_value;                
                subdata.bio = 0;
                left_subdata = subdata.metal;
                init_subdata.metal = subdata.metal;
                break;

            case Type.Tree:
                subdata.bio = substance_value;
                subdata.metal = 0;
                left_subdata = subdata.bio;
                init_subdata.bio = subdata.bio;
                break;
        }
    }

    //항상 자기 자신 자원이 남았는지 확인 -> 파괴용
    void Update()
    {
        //플레이어가 채취 초기화 상태일 때 실행
        //프레임 변화
        int frame_length = substance_value / sprite_frame.Length;

        for (int i = 1; i <= sprite_frame.Length; i++)
        {
            if (left_subdata / frame_length == i)
                sprite_renderer.sprite = sprite_frame[i - 1];
        }

        //자원이 없으면 파괴
        if (subdata.CheckZero())
        {
            player_data.target_substance = null;
            player_data.movement = GameDAO.PlayerData.Movement.isReady;
            DestroySubstance();
        }
    }

    //자원 리젠
    public void RegenSubstance()
    {
        //자원 초기화
        if (type == Type.Rock)
            subdata.metal = init_subdata.metal;
        else
            subdata.bio = init_subdata.bio;

        //위치 초기화
        map_manager.SetRandomSubstancePoint(index, sector);
        transform.position = map_manager.GetSubstanceRealPoint(sector, index, sprite_hieght);

        //스프라이트 초기화
        sprite_renderer.sprite = sprite_frame[sprite_frame.Length - 1];

        //파괴여부 false
        destroyed = false;
    }

    //자원 파괴
    private void DestroySubstance()
    {
        //위치 변경
        if (transform.localPosition.x >= 0)
            transform.localPosition = Define.init_pos_plus;
        else
            transform.localPosition = Define.init_pos_minus;

        map_manager.SetDestroyedSubstancePoint(index, sector, transform.localPosition.x);

        //파괴여부 true
        destroyed = true;

        //active false
        gameObject.SetActive(false);
    }

    //자원 채취
    public GameDAO.SubstanceData SubSubstanceData(int _amount)
    {
        //보낼 데이터
        GameDAO.SubstanceData send_subdata = new GameDAO.SubstanceData();

        //돌인지 나무인지 확인
        switch (type)
        {
            case Type.Rock:                
                //돌이면 자신 metal값 빼고 temp의 metal값 더해준다.
                if((subdata.metal - _amount) >= 0)
                {
                    subdata.metal -= _amount;
                    send_subdata.metal += _amount;
                }                    
                else
                {
                    subdata.metal = 0;
                }

                //남은 자원
                left_subdata = subdata.metal;

                break;

            case Type.Tree:
                //나무면 자신 bio 빼고 temp의 bio값 더해준다.
                if ((subdata.bio - _amount) >= 0)
                {
                    subdata.bio -= _amount;
                    send_subdata.bio += _amount;
                }                    
                else
                {
                    subdata.bio = 0;
                }

                //남은 자원
                left_subdata = subdata.bio;

                break;
        }        

        //금속이나 바이오 자원이 다 떨어지면 크리스탈을 주도록
        if(subdata.CheckMetalBioZero())
        {
            send_subdata.crystal = subdata.crystal;
            subdata.crystal = 0;
        }

        return send_subdata;
    }
}
