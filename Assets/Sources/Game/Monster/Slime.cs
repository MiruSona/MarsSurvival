using UnityEngine;
using System.Collections;

//슬라임 종류
public class Slime : MonsterCommon
{
    //초기화
    protected override void Init()
    {
        switch (intensity)
        {
            //약한애
            case Intensity.Weak:
                //스탯
                monster_data.hp = 360f;
                switch (attack_style)
                {
                    case AttackStyle.Melee:
                        monster_data.pool = GameDAO.MonsterData.Pool.Slime_Weak_Melee;
                        monster_data.atk_melee = 50f * 1.3f;
                        monster_data.speed = 0.055f;
                        if (file_manager.hard_mod)
                        {
                            monster_data.atk_melee *= Define.hard_slime_w;
                        }
                        break;

                    case AttackStyle.Range:
                        monster_data.pool = GameDAO.MonsterData.Pool.Slime_Weak_Range;
                        monster_data.atk_melee = 30f * 1.3f;
                        monster_data.atk_range = 50f * 1.3f;
                        monster_data.atk_range_timer.term = 2f;
                        monster_data.atk_range_timer.timer = monster_data.atk_range_timer.term;
                        monster_data.speed = 0.045f;
                        if (file_manager.hard_mod)
                        {
                            monster_data.atk_melee *= Define.hard_slime_w;
                            monster_data.atk_range *= Define.hard_slime_w;
                        }
                        break;
                }
                //자원 양 및 확률 초기화
                bio_range.min = 5; bio_range.max = 7;
                metal_range.min = 1; metal_range.max = 3;

                bio_drop_rate = 4000;
                metal_drop_rate = 4000;
                
                break;

            //보통애
            case Intensity.Normal:
                monster_data.hp = 600f;
                switch (attack_style)
                {
                    case AttackStyle.Melee:
                        monster_data.pool = GameDAO.MonsterData.Pool.Slime_Normal_Melee;
                        monster_data.atk_melee = 104f * 1.3f;
                        monster_data.speed = 0.055f;
                        if (file_manager.hard_mod)
                        {
                            monster_data.atk_melee *= Define.hard_slime_n;
                        }
                        break;

                    case AttackStyle.Range:
                        monster_data.pool = GameDAO.MonsterData.Pool.Slime_Normal_Range;
                        monster_data.atk_melee = 65f * 1.3f;
                        monster_data.atk_range = 104f * 1.3f;
                        monster_data.atk_range_timer.term = 2f;
                        monster_data.atk_range_timer.timer = monster_data.atk_range_timer.term;
                        monster_data.speed = 0.045f;
                        if (file_manager.hard_mod)
                        {
                            monster_data.atk_melee *= Define.hard_slime_n;
                            monster_data.atk_range *= Define.hard_slime_n;
                        }
                        break;
                }
                //자원 양 및 확률 초기화
                bio_range.min = 8; bio_range.max = 11;
                metal_range.min = 2; metal_range.max = 4;

                bio_drop_rate = 4000;
                metal_drop_rate = 4000;

                break;
            //강한애
            case Intensity.Strong:
                monster_data.hp = 800f;
                switch (attack_style)
                {
                    case AttackStyle.Melee:
                        monster_data.pool = GameDAO.MonsterData.Pool.Slime_Strong_Melee;
                        monster_data.atk_melee = 156f * 1.5f;
                        monster_data.speed = 0.055f;
                        if (file_manager.hard_mod)
                        {
                            monster_data.atk_melee *= Define.hard_slime_s;
                        }
                        break;

                    case AttackStyle.Range:
                        monster_data.pool = GameDAO.MonsterData.Pool.Slime_Strong_Range;
                        monster_data.atk_melee = 104f * 1.5f;
                        monster_data.atk_range = 156f * 1.5f;
                        monster_data.atk_range_timer.term = 2f;
                        monster_data.atk_range_timer.timer = monster_data.atk_range_timer.term;
                        monster_data.speed = 0.045f;
                        if (file_manager.hard_mod)
                        {
                            monster_data.atk_melee *= Define.hard_slime_s;
                            monster_data.atk_range *= Define.hard_slime_s;
                        }
                        break;
                }
                //자원 양 및 확률 초기화
                bio_range.min = 14; bio_range.max = 19;
                metal_range.min = 3; metal_range.max = 5;

                bio_drop_rate = 4000;
                metal_drop_rate = 4000;

                break;
        }

        //자원 생성
        if(intensity != Intensity.Strong)
            CreateSubstance(bio_range, metal_range, bio_drop_rate, metal_drop_rate, 0);
        else
            CreateSubstance(bio_range, metal_range, bio_drop_rate, metal_drop_rate, 10);

        //총알 관련 초기화
        if (attack_style == AttackStyle.Range)
        {
            //사운드 초기화
            bullet_sound = Resources.Load("Sound/SoundEffect/Slime_Attack") as AudioClip;
            //이펙트 초기화
            bullet_effect = transform.FindChild("Slime_Effect").GetComponent<ParticleSystem>();
            //총알 가져옴
            bullet_prefab = Resources.Load<GameObject>("Prefab/Game/Monster/Bullet/Slime_Bullet");
            //총알 생성(갯수, 속도)
            CreateBullets(3, 700f);
        }

        //죽는 이펙트 초기화
        DieEffectInit("Slime_Die_Effect");
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
        //총알 이펙트
        //방향 바꿔주기
        Vector3 bullet_angle = bullet_effect.transform.localEulerAngles;
        if (monster_data.facing == 1)
            bullet_angle.y = 90f;
        else
            bullet_angle.y = 270f;
        bullet_effect.transform.localEulerAngles = bullet_angle;
        //이펙트 발싸!
        bullet_effect.Emit(20);

        //총알 발사!
        ShootBullet(0f, bullet_effect.transform.localPosition.y);
    }
}
