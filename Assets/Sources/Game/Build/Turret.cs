using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Turret : BuildCommon {

    //참조
    private GameDAO.TurretData public_data;

    //총알 관련
    public Stack<TurretBullet> bullet_stack = new Stack<TurretBullet>();
    protected GameObject bullet_prefab;
    protected Vector3 fire_pos;

    private int bullet_num = 10;
    private float bullet_speed = 700f;

    #region 상속

    //빠른 초기화(한번)
    protected override void AwakeInit()
    {
        //참조
        public_data = GameDAO.instance.turret_data;
        bullet_prefab = Resources.Load("Prefab/Game/Build/Bullet/TurretBullet") as GameObject;

        //my_data 초기화
        my_data = new GameDAO.TurretData();
        my_data.pool = GameDAO.BuildData.Pool.Turret;
    }

    //초기화
    protected override void ChildInit()
    {        
        //초기화
        my_data.step = public_data.step;
        my_data.hp = my_data.hp_max[public_data.step];

        my_data.atk_timer.timer = my_data.atk_timer.term;
        my_data.damage_timer.timer = 0f;

        my_data.state = GameDAO.BuildData.State.isAlive;
        my_data.movement = GameDAO.BuildData.Movement.isReady;

        //스프라이트 변화
        sprite_renderer.sprite = public_data.sprites[my_data.step];

        //총알 생성
        CreateBullets();
    }

    //로드 초기화
    protected override void LoadChildInit()
    {
        //스프라이트 변화
        sprite_renderer.sprite = public_data.sprites[my_data.step];

        my_data.atk_timer.timer = my_data.atk_timer.term;
        my_data.damage_timer.timer = 0f;

        my_data.state = GameDAO.BuildData.State.isAlive;
        my_data.movement = GameDAO.BuildData.Movement.isReady;

        //총알 생성
        CreateBullets();
    }

    //공격
    protected override void Attack()
    {
        //타이머 체크
        if (my_data.atk_timer.timer < my_data.atk_timer.term)
            my_data.atk_timer.timer += Time.deltaTime;
        else
        {
            //총알 발사
            ShootBullet(0.8f * my_data.facing, 0.47f);

            my_data.atk_timer.timer = 0;
        }
    } 
    
    #endregion

    #region 총알 관련

    //총알 생성
    private void CreateBullets()
    {
        //이미 생성되있으면 생성X
        if (bullet_stack.ToArray().Length >= bullet_num)
            return;

        //총알 생성
        TurretBullet bullet;
        for (int i = 0; i < bullet_num; i++)
        {
            bullet = Instantiate(bullet_prefab).GetComponent<TurretBullet>();
            bullet.Init(GetComponent<Turret>(), my_data.atk[my_data.step], bullet_speed);
            bullet_stack.Push(bullet);
        }
    }

    //총알 회수
    public void ReloadBullet(TurretBullet _bullet)
    {
        bullet_stack.Push(_bullet);
    }

    //총알 발사
    private void ShootBullet(float _offset_x, float _offset_y)
    {
        //총알
        TurretBullet bullet;
        //위치 초기화
        fire_pos = transform.localPosition;
        fire_pos.x += _offset_x;
        fire_pos.y += _offset_y;

        //총알 남았는지 확인
        if (bullet_stack.ToArray().Length != 0)
        {
            //총알 가져옴
            bullet = bullet_stack.Pop();
            //재장전 위치 설정
            bullet.SetReloadPos(fire_pos);
            //발사!
            bullet.Shoot(my_data.facing);
        }
    }
    #endregion
}
