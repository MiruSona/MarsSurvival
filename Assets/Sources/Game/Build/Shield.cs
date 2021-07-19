using UnityEngine;
using System.Collections;

public class Shield : BuildCommon {

    //참조
    private GameDAO.ShieldData public_data;

    #region 상속

    //빠른 초기화(한번)
    protected override void AwakeInit()
    {
        //참조
        public_data = GameDAO.instance.shield_data;

        //my_data 초기화
        my_data = new GameDAO.ShieldData();
        my_data.pool = GameDAO.BuildData.Pool.Shield;
    }

    //초기화
    protected override void ChildInit()
    {
        //초기화
        my_data.step = public_data.step;
        my_data.hp = my_data.hp_max[public_data.step];
        
        my_data.damage_timer.timer = 0f;

        my_data.state = GameDAO.BuildData.State.isAlive;
        my_data.movement = GameDAO.BuildData.Movement.isReady;

        //스프라이트 변화
        sprite_renderer.sprite = public_data.sprites[my_data.step];
    }

    //로드 초기화
    protected override void LoadChildInit()
    {
        //스프라이트 변화
        sprite_renderer.sprite = public_data.sprites[my_data.step];

        my_data.damage_timer.timer = 0f;

        my_data.state = GameDAO.BuildData.State.isAlive;
        my_data.movement = GameDAO.BuildData.Movement.isReady;
    }

    //공격(안함)
    protected override void Attack()
    {

    }

    #endregion
}
