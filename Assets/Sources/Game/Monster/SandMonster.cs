using UnityEngine;
using System.Collections;

public class SandMonster : MonsterCommon {

    //초기화
    protected override void Init()
    {
        switch (intensity)
        {
            //약한애
            case Intensity.Weak:
                //스탯
                monster_data.hp = 720f;
                switch (attack_style)
                {
                    case AttackStyle.Melee:
                        monster_data.pool = GameDAO.MonsterData.Pool.SandMonster_Weak;
                        monster_data.atk_melee = 130f * 1.3f;
                        monster_data.speed = 0.065f;
                        if (file_manager.hard_mod)
                            monster_data.atk_melee *= Define.hard_sandmonster_w;
                        break;
                }
                //자원 양 및 확률 초기화
                bio_range.min = 15; bio_range.max = 21;
                metal_range.min = 15; metal_range.max = 21;

                bio_drop_rate = 5000;
                metal_drop_rate = 5000;

                break;

            //보통애
            case Intensity.Normal:
                monster_data.hp = 960f;
                switch (attack_style)
                {
                    case AttackStyle.Melee:
                        monster_data.pool = GameDAO.MonsterData.Pool.SandMonster_Normal;
                        monster_data.atk_melee = 182f * 1.3f;
                        monster_data.speed = 0.075f;
                        if (file_manager.hard_mod)
                            monster_data.atk_melee *= Define.hard_sandmonster_n;
                        break;
                }
                //자원 양 및 확률 초기화
                bio_range.min = 25; bio_range.max = 33;
                metal_range.min = 25; metal_range.max = 33;

                bio_drop_rate = 5000;
                metal_drop_rate = 5000;

                break;
        }

        //자원 생성
        CreateSubstance(bio_range, metal_range, bio_drop_rate, metal_drop_rate, 500);

        //죽는 이펙트 & 사운드 초기화
        DieEffectInit("SandMonster_Die_Effect");  

        //몬스터 이펙트 초기화
        monster_effect = transform.FindChild("Particle").GetComponent<ParticleSystem>().GetComponent<Renderer>();
    }

    //움직임
    protected override void Move()
    {
        //움직이기
        transform.Translate(monster_data.speed * monster_data.facing, 0, 0);
    }

    //원거리 공격
    protected override void AttackRange()
    {
        //안씀
    }

    //모레폭풍 꺼지면 죽게
    protected override void StormDie()
    {
        if (global_state.state == Define.Day || global_state.state == Define.Night)
            DieWithDisable();
    }
}
