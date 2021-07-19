using UnityEngine;
using System.Collections;

public class TurretBullet : MonoBehaviour {

    //참조
    private Rigidbody2D rb2d;
    private Turret turret;
    private Transform turret_transform;

    //위치값
    private Vector3 reload_pos;
    private float atk;
    private float speed;
    private float distance_max = 11f;

    //범위 나갔는지 채크
    void Update()
    {
        float distance = Mathf.Abs(transform.localPosition.x - turret_transform.localPosition.x);

        if (distance >= distance_max)
            Reload();
    }

    //초기화
    public void Init(Turret _turret, float _atk, float _speed)
    {
        turret = _turret;
        turret_transform = turret.transform;
        atk = _atk;
        speed = _speed;
        rb2d = GetComponent<Rigidbody2D>();
        //방향 전환
        Vector3 local_scale = transform.localScale;
        local_scale.x *= turret.my_data.facing;
        transform.localScale = local_scale;
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
        //액티브
        gameObject.SetActive(true);
        //발사!
        rb2d.AddForce(Vector2.right * speed * _direction);
    }

    //파괴 시
    private void Reload()
    {
        //멈추기
        rb2d.velocity = Vector2.zero;
        //장전
        turret.ReloadBullet(GetComponent<TurretBullet>());
        //대기상태로
        turret.my_data.movement = GameDAO.BuildData.Movement.isReady;
        //disable
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        GameDAO.MonsterData monster_data = null;
        //몬스터에게 닿을 시
        if (col.CompareTag("Monster"))
        {
            monster_data = col.GetComponent<MonsterCommon>().monster_data;
            //몬스터가 살아있는 상태면 데미지!
            if (monster_data.state == GameDAO.MonsterData.State.isAlive)
                monster_data.SubHP(atk);
            //장전
            Reload();
        }

        //경계에 닿으면 리로드
        if (col.CompareTag("Boundary"))
            Reload();
    }
}
