using UnityEngine;
using System.Collections;

public class Mine : BuildCommon {

    //참조
    private AudioClip beep_sound;

    #region 상속

    //빠른 초기화(한번)
    protected override void AwakeInit()
    {
        //my_data 초기화
        my_data = new GameDAO.MineData();
        my_data.pool = GameDAO.BuildData.Pool.Mine;

        //죽는 이펙트 초기화
        die_effect = Resources.Load<GameObject>("Prefab/Game/Effects/Mine_Effect");

        //참조 초기화
        beep_sound = Resources.Load<AudioClip>("Sound/SoundEffect/MineBeep");
    }

    //초기화
    protected override void ChildInit()
    {
        //초기화
        sprite_renderer.color = Color.white;

        my_data.damage_timer.timer = 0f;

        my_data.state = GameDAO.BuildData.State.isAlive;
        my_data.movement = GameDAO.BuildData.Movement.isReady;
    }

    //로드 초기화
    protected override void LoadChildInit()
    {
        my_data.damage_timer.timer = 0f;

        my_data.state = GameDAO.BuildData.State.isAlive;
        my_data.movement = GameDAO.BuildData.Movement.isReady;
    }

    //공격
    protected override void Attack()
    {
        //타이머 체크
        if (my_data.atk_timer.timer < my_data.atk_timer.term)
        {
            my_data.atk_timer.timer += Time.deltaTime;
            if (sprite_renderer.color != Color.white)
                sprite_renderer.color = Color.white;
            else
                sprite_renderer.color = Color.red;

            //사운드
            audio_source.PlayOneShot(beep_sound);
        }
        else
        {
            //공격&파괴
            if(monster_target.ToArray().Length != 0)
            {
                for(int i = 0; i < monster_target.ToArray().Length; i++)
                    monster_target[i].SubHP(my_data.atk[0]);

                my_data.atk_timer.timer = 0;
                my_data.state = GameDAO.BuildData.State.isDead;
            }

            if (boss_target.ToArray().Length != 0)
            {
                for (int i = 0; i < boss_target.ToArray().Length; i++)
                    boss_target[i].SubHP(my_data.atk[0]);

                my_data.atk_timer.timer = 0;
                my_data.state = GameDAO.BuildData.State.isDead;
            }

            if (sp_boss_target.ToArray().Length != 0)
            {
                for (int i = 0; i < sp_boss_target.ToArray().Length; i++)
                    sp_boss_target[i].SubHP(my_data.atk[0]);

                my_data.atk_timer.timer = 0;
                my_data.state = GameDAO.BuildData.State.isDead;
            }
        }
    }

    //인식할 경우
    protected override void RecognizeEnter()
    {
        my_data.movement = GameDAO.BuildData.Movement.isAttack;
    }

    //죽을때
    protected override void DieChild()
    {
        monster_target.Clear();
        boss_target.Clear();
        sp_boss_target.Clear();
        my_data.atk_timer.timer = 0f;
    }

    #endregion
}
