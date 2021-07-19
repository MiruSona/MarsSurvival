using UnityEngine;
using System.Collections;

public class Regenerator : BuildCommon {

    //참조
    private GameDAO.RegeneratorData public_data;
    private GameDAO.PlayerData player_data;
    
    #region 상속

    //빠른 초기화(한번)
    protected override void AwakeInit() {
        //참조
        public_data = GameDAO.instance.regenerator_data;
        player_data = GameDAO.instance.player_data;

        //my_data 초기화
        my_data = new GameDAO.RegeneratorData();
        my_data.pool = GameDAO.BuildData.Pool.Regenerator;
    }

    //초기화
    protected override void ChildInit() {
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
    protected override void Attack() {

    }

    #endregion
    //Heal 처리
    void OnTriggerStay2D(Collider2D _col)
    {
        if (_col.CompareTag("Player"))
        {
            Heal();
        }
    }

    //플레이어 나가면 타이머 초기화
    void OnTriggerExit2D(Collider2D _col)
    {
        if (_col.CompareTag("Player"))
        {
            my_data.atk_timer.timer = 0;
        }
    }

    //이거 실행
    protected void Heal()
    {
        //살아있는 상태가 아니면 실행 X
        if (my_data.state != GameDAO.BuildData.State.isAlive)
            return;

        //주인공 피가 꽉차있어도 실행 X
        if (player_data.hp == player_data.max_hp)
            return;

        if (my_data.atk_timer.timer < my_data.atk_timer.term)
            my_data.atk_timer.timer += Time.deltaTime;
        else
        {
            //공격력 만큼 회복
            player_data.AddHP(my_data.atk[my_data.step]);

            //체력감소
            //HP가 0보다 작거나 같으면
            if (my_data.hp - my_data.atk[my_data.step] <= 0)
            {
                //HP를 0으로 만들고 죽음상태 표시
                my_data.hp = 0;
                my_data.state = GameDAO.BuildData.State.isDead;
            }
            else //HP가 0보다 많으면
            {
                //값만큼 빼준다
                my_data.hp -= my_data.atk[my_data.step];
            }

            my_data.atk_timer.timer = 0f;
        }

    }

    
}
