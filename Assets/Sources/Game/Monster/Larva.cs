using UnityEngine;
using System.Collections;

public class Larva : MonsterCommon {

    //초기화
    protected override void Init()
    {
        switch (intensity)
        {
            //약한애
            case Intensity.Weak:
                //스탯
                monster_data.hp = 700f;
                switch (attack_style)
                {
                    case AttackStyle.Melee:
                        monster_data.pool = GameDAO.MonsterData.Pool.Larva_Weak;
                        monster_data.atk_melee = 90f;
                        monster_data.speed = 0.065f;
                        break;
                }
                //자원 양 및 확률 초기화
                bio_range.min = 12; bio_range.max = 17;
                metal_range.min = 21; metal_range.max = 26;
                crystal_range.min = 2; crystal_range.max = 4;

                bio_drop_rate = 7000;
                metal_drop_rate = 7000;

                break;

            //보통애
            case Intensity.Normal:
                monster_data.hp = 1100f;
                switch (attack_style)
                {
                    case AttackStyle.Melee:
                        monster_data.pool = GameDAO.MonsterData.Pool.Larva_Normal;
                        monster_data.atk_melee = 130f;
                        monster_data.speed = 0.075f;
                        break;
                }
                //자원 양 및 확률 초기화
                bio_range.min = 21; bio_range.max = 29;
                metal_range.min = 26; metal_range.max = 33;
                crystal_range.min = 2; crystal_range.max = 4;

                bio_drop_rate = 7000;
                metal_drop_rate = 7000;

                break;
        }

        //자원 생성
        CreateSubstance(bio_range, metal_range, crystal_range, bio_drop_rate, metal_drop_rate);

        //죽는 이펙트 초기화
        DieEffectInit("Bugs_Die_Effect");
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

    protected override void NightDie()
    {
        if(global_state.state == Define.Day || global_state.state == Define.DayStorm)
            DieWithDisable();
    }
}
