using UnityEngine;
using System.Collections;

public class Larva : MonsterCommon {

    //가스
    private GameObject gas_effect;
    private int rate = 5000;

    //초기화
    protected override void Init()
    {
        switch (intensity)
        {
            //약한애
            case Intensity.Weak:
                //스탯
                monster_data.hp = 560f;
                switch (attack_style)
                {
                    case AttackStyle.Melee:
                        monster_data.pool = GameDAO.MonsterData.Pool.Larva_Weak;
                        monster_data.atk_melee = 117f * 1.3f;
                        monster_data.speed = 0.065f;
                        break;
                }
                //자원 양 및 확률 초기화
                bio_range.min = 3; bio_range.max = 8;
                metal_range.min = 21; metal_range.max = 26;

                bio_drop_rate = 5000;
                metal_drop_rate = 5000;

                break;

            //보통애
            case Intensity.Normal:
                monster_data.hp = 900f;
                switch (attack_style)
                {
                    case AttackStyle.Melee:
                        monster_data.pool = GameDAO.MonsterData.Pool.Larva_Normal;
                        monster_data.atk_melee = 169f * 1.3f;
                        monster_data.speed = 0.075f;
                        break;
                }
                //자원 양 및 확률 초기화
                bio_range.min = 10; bio_range.max = 18;
                metal_range.min = 26; metal_range.max = 33;

                bio_drop_rate = 5000;
                metal_drop_rate = 5000;

                break;
        }

        //자원 생성
        CreateSubstance(bio_range, metal_range, bio_drop_rate, metal_drop_rate, 100);

        //죽는 이펙트 초기화
        DieEffectInit("Bugs_Die_Effect");

        //가스 이펙트 초기화
        gas_effect = Resources.Load<GameObject>("Prefab/Game/Effects/PoisonGas");
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

    //밤에 죽도록
    protected override void NightDie()
    {
        if(global_state.state == Define.Day || global_state.state == Define.DayStorm)
            DieWithDisable();
    }

    //죽을때 확률로 가스뿜게
    protected override void DieChild()
    {
        //확률
        int random = Random.Range(0, 10000);

        //rate / 10000 확률로 가스 발생
        if(random < rate)
        {
            //이펙트 생성
            GameObject effect = Instantiate(gas_effect);
            effect.transform.localPosition = transform.localPosition;
        }
    }
}
