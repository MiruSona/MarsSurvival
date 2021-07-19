using UnityEngine;
using System.Collections;

public class MonsterBullet : MonoBehaviour {

    //참조
    private Rigidbody2D rb2d;
    private GameDAO.PlayerData player_data;
    private MonsterCommon monster_common;

    //위치값
    private Vector3 reload_pos;
    private float atk;
    private float speed;

    //초기화
    public void Init(MonsterCommon _monster_common, float _atk, float _speed)
    {
        monster_common = _monster_common;
        atk = _atk;
        speed = _speed;
        rb2d = GetComponent<Rigidbody2D>();
        player_data = GameDAO.instance.player_data;
    }

    //위치 지정
    public void SetReloadPos(Vector3 _pos)
    {
        reload_pos = _pos;
    }
	
    //발사
    public void Shoot(float _direction)
    {
        //초기화 위치로 이동
        transform.localPosition = reload_pos;
        gameObject.SetActive(true);
        rb2d.AddForce(Vector2.right * speed * _direction);
    }

    //파괴 시
    private void Reload()
    {
        //멈추기
        rb2d.velocity = Vector2.zero;
        //장전
        monster_common.ReloadBullet(GetComponent<MonsterBullet>());
        //disable
        gameObject.SetActive(false);
    }
    
    void OnTriggerEnter2D(Collider2D col)
    {
        //주인공에게 닿을 시
        if (col.CompareTag("Player"))
        {
            //주인공이 살아 있을 경우에만 데미지 주고 밀기
            if(player_data.state == GameDAO.PlayerData.State.isAlive)
            {
                player_data.SubHP(atk);
                monster_common.PushPlayer();
            }
            Reload();
        }

        //건물에게 닿을 시
        if (col.CompareTag("Build"))
        {
            //지뢰면 공격X - 지뢰
            if (col.name != "MineCollider")
            {
                BuildCommon build = col.GetComponent<BuildCommon>();
                //건물이 살아 있을 경우에만 데미지 주기
                if (build.my_data.state == GameDAO.BuildData.State.isAlive)
                {
                    build.my_data.SubHP(atk);
                }
            }
            Reload();
        }

        //땅에 닿을 시
        if (col.CompareTag("Ground"))
            Reload();
    }
}
