using UnityEngine;
using System.Collections;

public class PlayerBullet : MonoBehaviour {

    //참조
    private Rigidbody2D rb2d;
    private SpriteRenderer sprite_renderer;
    private Material trail_material;
    private Player player;
    private Transform camera_transform;
    private float camera_width_half;

    private GameDAO.PlayerData player_data;
    private GameDAO.WeaponData weapon_data;

    //위치값
    private Vector3 reload_pos;

    //초기화
    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        sprite_renderer = GetComponent<SpriteRenderer>();

        TrailRenderer trail_renerer = GetComponent<TrailRenderer>();
        trail_renerer.sortingLayerName = "ToolWeapon";
        trail_renerer.sortingOrder = 0;
        trail_material = trail_renerer.material;

        player = Player.instance;
        camera_transform = Camera.main.transform;
        camera_width_half = Camera.main.orthographicSize * Camera.main.aspect;

        player_data = GameDAO.instance.player_data;
        weapon_data = player_data.weapon_data;
        
        gameObject.SetActive(false);
    }

    //Enable 시 모양 변경
    void OnEnable()
    {
        //스프라이트 변경
        sprite_renderer.color = weapon_data.colors[weapon_data.step];

        //트레일 랜더러 색변경
        trail_material.color = weapon_data.colors[weapon_data.step];
    }

    void Update()
    {
        //카메라 벗어나면 장전
        if(transform.localPosition.x >= camera_transform.localPosition.x + camera_width_half)
            Reload();

        if (transform.localPosition.x <= camera_transform.localPosition.x - camera_width_half)
            Reload();
    }

    //위치 지정
    public void SetReloadPos(Vector3 _pos)
    {
        reload_pos = _pos;
    }

    //발사
    public void Shoot()
    {
        //초기화 위치로 이동
        transform.localPosition = reload_pos;
        transform.localScale = new Vector3(player_data.facing, 1f, 1f);

        //엑티브
        gameObject.SetActive(true);

        //발사!
        rb2d.AddForce(Vector2.right * weapon_data.bullet_speed * player_data.facing);
    }

    //파괴 시
    private void Reload()
    {
        //멈추기
        rb2d.velocity = Vector2.zero;
        //장전
        player.ReloadBullet(GetComponent<PlayerBullet>());
        //disable
        gameObject.SetActive(false);
    }

    //충돌 판정
    void OnTriggerEnter2D(Collider2D col)
    {
        GameDAO.MonsterData monster_data = null;
        //몬스터에게 닿을 시
        if (col.CompareTag("Monster"))
        {
            monster_data = col.GetComponent<MonsterCommon>().monster_data;
            //몬스터가 살아있는 상태면 데미지!
            if (monster_data.state == GameDAO.MonsterData.State.isAlive)
                monster_data.SubHP(weapon_data.atk[weapon_data.step, weapon_data.level]);
            //장전
            Reload();
        }

        //경계에 닿으면 리로드
        if (col.CompareTag("Boundary"))
            Reload();
    }
}
