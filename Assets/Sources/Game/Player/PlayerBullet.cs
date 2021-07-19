using UnityEngine;
using System.Collections;

public class PlayerBullet : MonoBehaviour {

    //참조
    private Rigidbody2D rb2d;
    private SpriteRenderer sprite_renderer;
    private TrailRenderer[] trail_renderer = new TrailRenderer[Define.weapon_step_max];
    private Player player;
    private Transform camera_transform;

    private GameDAO.PlayerData player_data;
    private GameDAO.WeaponData weapon_data;

    //위치값
    private Vector3 reload_pos;

    //크기값
    private Vector3 scale = new Vector3(1f, 1f, 1f);

    //초기화
    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        sprite_renderer = GetComponent<SpriteRenderer>();

        //trail renderer 초기화
        trail_renderer[0] = transform.FindChild("YellowTrail").GetComponent<TrailRenderer>();
        trail_renderer[1] = transform.FindChild("OrangeTrail").GetComponent<TrailRenderer>();
        trail_renderer[2] = transform.FindChild("RedTrail").GetComponent<TrailRenderer>();
        for(int i = 0; i < trail_renderer.Length; i++)
        {
            trail_renderer[i].sortingLayerName = "ToolWeapon";
            trail_renderer[i].sortingOrder = 0;
        }

        player = Player.instance;
        camera_transform = Camera.main.transform;
        
        player_data = GameDAO.instance.player_data;
        weapon_data = player_data.weapon_data;
        
        gameObject.SetActive(false);
    }

    //Enable 시 모양 변경
    void OnEnable()
    {
        //스프라이트 변경
        sprite_renderer.color = weapon_data.colors[weapon_data.step];

        //크기변경
        switch (weapon_data.step)
        {
            case 0:
                scale.x = 1f * player_data.facing;
                scale.y = 1f;
                break;
            case 1:
                scale.x = 1.3f * player_data.facing;
                scale.y = 1.3f;
                break;
            case 2:
                scale.x = 2f * player_data.facing;
                scale.y = 2f;
                break;
        }
        transform.localScale = scale;

        //트레일 랜더러 색변경
        for (int i = 0; i < trail_renderer.Length; i++)
            trail_renderer[i].gameObject.SetActive(false);

        trail_renderer[weapon_data.step].gameObject.SetActive(true);
        trail_renderer[weapon_data.step].time = 0;
        trail_renderer[weapon_data.step].time = 0.1f;
    }

    void Update()
    {
        //카메라 벗어나면 장전
        if(transform.localPosition.x >= camera_transform.localPosition.x + Define.camera_width_half)
            Reload();

        if (transform.localPosition.x <= camera_transform.localPosition.x - Define.camera_width_half)
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
        
        //몬스터에게 닿을 시
        if (col.CompareTag("Monster"))
        {
            GameDAO.MonsterData monster_data = null;
            monster_data = col.GetComponent<MonsterCommon>().monster_data;
            //몬스터가 살아있는 상태면 데미지!
            if (monster_data.state == GameDAO.MonsterData.State.isAlive)
                monster_data.SubHP(weapon_data.atk[weapon_data.step, weapon_data.level]);
            //장전
            Reload();
        }

        //보스에게 닿을 시
        if (col.CompareTag("Boss"))
        {
            GameDAO.BossData boss_data = null;
            boss_data = col.GetComponent<BossCommon>().boss_data;
            //보스가 살아있는 상태면 데미지!
            if (boss_data.state == GameDAO.BossData.State.isAlive)
                boss_data.SubHP(weapon_data.atk[weapon_data.step, weapon_data.level]);
            //장전
            Reload();
        }

        //경계에 닿으면 리로드
        if (col.CompareTag("Boundary"))
            Reload();
    }
}
