using UnityEngine;
using System.Collections;
using System;

public class JellyfishBoss : SPBossCommon {

    //y좌표 움직임값(sin(x)의 x(각도)값) -> 위아래움직임
    private float degree_y = 0f;
    private const float degree_add = 0.08f;
    private const float degree_max = 0.2f;
    private const float pos_y_offset = -1.40f;

    protected override void Init()
    {
        RegenInit();

        //사운드 초기화
        die_sound = Resources.Load<AudioClip>("Sound/SoundEffect/Boss_Explode");
        melee_attack_sound = Resources.Load<AudioClip>("Sound/SoundEffect/Electric_Shock");

        //공격 이펙트
        melee_attack_effect = transform.FindChild("Melee_Attack").gameObject;
    }

    protected override void RegenInit()
    {
        //랜덤 위치 추가
        map_manager.RandomSPBossPoint();
        //위치 초기화
        transform.localPosition = map_manager.GetSPBossRealPoint(pos_y_offset);

        //스탯 초기화
        sp_boss_data.hp_max = 1500f;
        sp_boss_data.hp = 1500f;
        sp_boss_data.speed = 0.065f;

        sp_boss_data.atk = 150f;
        sp_boss_data.melee_atk_timer.term = 0.1f;
        if (file_manager.hard_mod)
            sp_boss_data.atk *= Define.hard_jellyfish_spboss;

        //자원 양 및 확률 초기화
        bio_range.min = 300; bio_range.max = 302;
        metal_range.min = 300; metal_range.max = 302;
        crystal_drop_rate = 10000;

        //자원 생성
        CreateSubstance(bio_range, metal_range, crystal_drop_rate);
    }

    protected override void LoadChild()
    {
        //움직임 초기화
        sp_boss_data.movement = GameDAO.SPBossData.Movement.isMove;
        //위치 초기화
        transform.localPosition = map_manager.GetSPBossRealPoint(pos_y_offset);
    }

    protected override void MeleeAttack()
    {
        //애니메이션
        animator.SetBool("Move", false);
        animator.SetBool("Attack", true);

        //공격
        if (sp_boss_data.melee_atk_timer.timer < sp_boss_data.melee_atk_timer.term)
            sp_boss_data.melee_atk_timer.timer += Time.deltaTime;
        else
        {
            //플레이어 공격중이면
            if (attack_player)
                PushPlayer();
            else
            {
                //타겟이 죽으면 null로 초기화 및 움직임으로 변경
                if (attakc_target.state == GameDAO.BuildData.State.isDead)
                {
                    attakc_target = null;
                    sp_boss_data.movement = GameDAO.SPBossData.Movement.isMove;
                }
            }

            //이펙트 있으면 실행
            if (melee_attack_effect != null)
                melee_attack_effect.SetActive(true);

            //공격 사운드
            if (melee_attack_sound != null)
                audio_source.PlayOneShot(melee_attack_sound);

            //공격!
            if (attack_player)
                player_data.SubHP(sp_boss_data.atk);
            else
            {
                if (attakc_target != null)
                    attakc_target.SubHP(sp_boss_data.atk);
                else
                    sp_boss_data.movement = GameDAO.SPBossData.Movement.isMove;
            }

            sp_boss_data.melee_atk_timer.timer = 0;
        }
    }

    protected override void Move()
    {
        //애니메이션
        animator.SetBool("Move", true);
        animator.SetBool("Attack", false);

        //y값 구하기
        if (degree_y >= 359.0f)
            degree_y = 0f;
        else
            degree_y += degree_add;

        //이동
        transform.Translate(sp_boss_data.speed * sp_boss_data.facing, 0, 0);
        Vector3 pos = transform.localPosition;
        pos.y = pos_y_offset + (Mathf.Sin(degree_y) * degree_max);
        transform.localPosition = pos;
    }

    protected override void TriggerChild(Collider2D _col)
    {
        //플레이어 에게 닿으면
        if (_col.CompareTag("Player"))
        {
            attack_player = true;
            //공격 상태로 변경
            sp_boss_data.movement = GameDAO.SPBossData.Movement.isMeleeAttack;
        }

        //건물 에게 닿으면
        if (_col.CompareTag("Build"))
        {
            //지뢰면 공격X - 지뢰
            if (_col.name != "MineCollider")
            {
                attack_player = false;
                //타겟 지정
                attakc_target = _col.GetComponent<BuildCommon>().my_data;
                //공격 상태로 변경
                sp_boss_data.movement = GameDAO.SPBossData.Movement.isMeleeAttack;
            }
        }
    }
}
