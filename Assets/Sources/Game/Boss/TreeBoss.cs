using UnityEngine;
using System.Collections;

public class TreeBoss : BossCommon {

    //초기화
    protected override void Init()
    {
        //스탯
        boss_data.hp_max = 1700f;
        boss_data.hp = boss_data.hp_max;
        boss_data.atk = 150f;
        boss_data.speed_x = 0.055f;

        //자원 양 및 확률 초기화
        bio_range.min = 70; bio_range.max = 100;
        metal_range.min = 0; metal_range.max = 0;
        crystal_range.min = 5; crystal_range.max = 10;

        crystal_drop_rate = 1000;

        //자원 생성
        CreateSubstance(bio_range, metal_range, crystal_range, crystal_drop_rate);

        //죽는 이펙트 초기화
        DieEffectInit("Tree_Die_Effect");

        //땅에 닿는 이펙트
        ground_effect = transform.FindChild("Dust_Effect").GetComponent<ParticleSystem>();
    }

    //움직임
    protected override void Move()
    {
        //움직이기
        transform.Translate(boss_data.speed_x * boss_data.facing, 0, 0);
    }

    //공격 상속
    protected override void AttackChild()
    {
        //이펙트 있으면 실행
        if (ground_effect != null)
            ground_effect.gameObject.SetActive(true);
    }
}
