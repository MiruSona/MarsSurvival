using UnityEngine;
using System.Collections;

public class SandwormBoss : SPBossCommon {

    //컴포넌트
    private Collider2D my_collider;
    private Collider2D recognizer;

    //이펙트
    private GameObject dust_effect;

    //공격 횟수
    private int attack_num = 0;
    private const int attack_limit = 3;

    //딜레이
    private GameDAO.Timer spawn_delay_timer = new GameDAO.Timer(0, 3f);
    private GameDAO.Timer move_pos_delay_timer = new GameDAO.Timer(0, 5f);
    private GameDAO.Timer move_timer = new GameDAO.Timer(0, 2f);

    //움직임 관련
    private const float pos_y_offset = -1.25f;

    //총알 관련
    private const float shoot_limit = 3;
    private float shoot_num = 0;
    private GameDAO.Timer shoot_timer = new GameDAO.Timer(0, 0.5f);

    protected override void Init()
    {
        RegenInit();

        //컬리더
        my_collider = GetComponent<Collider2D>();
        recognizer = transform.FindChild("Recognizer").GetComponent<Collider2D>();

        //총알 관련 초기화
        //총알 가져옴
        bullet_prefab = Resources.Load<GameObject>("Prefab/Game/SPBoss/Bullet/Sandworm_Bullet");
        fire_pos = transform.FindChild("FirePos");
        //총알 생성(갯수, 속도)
        CreateBullets(5, 1000f);

        //사운드 초기화
        die_sound = Resources.Load<AudioClip>("Sound/SoundEffect/Sandworm_Die");
        move_sound = Resources.Load<AudioClip>("Sound/SoundEffect/Sandworm_Move");
        range_attack_sound = Resources.Load<AudioClip>("Sound/SoundEffect/Sandworm_Acid_atk");

        //공격 이펙트
        range_attack_effect = transform.FindChild("Range_Attack").gameObject;

        //땅 이펙트
        dust_effect = transform.FindChild("Dust_Effect").gameObject;
    }

    protected override void RegenInit()
    {
        //움직임 초기화
        sp_boss_data.movement = GameDAO.SPBossData.Movement.isReady;
        move_pos_delay_timer.timer += Time.deltaTime;

        //랜덤 위치 추가
        map_manager.RandomSPBossPoint();
        //위치 초기화
        transform.localPosition = map_manager.GetSPBossRealPoint(-1.25f);

        //스탯 초기화
        sp_boss_data.hp_max = 3500f;
        sp_boss_data.hp = 3500f;
        sp_boss_data.speed = 0.13f;

        sp_boss_data.atk = 300f;
        sp_boss_data.melee_atk_timer.term = 0.1f;
        sp_boss_data.range_atk_timer.term = 1.0f;
        if(file_manager.hard_mod)
            sp_boss_data.atk *= Define.hard_sandworm_spboss;

        //자원 양 및 확률 초기화
        bio_range.min = 600; bio_range.max = 602;
        metal_range.min = 600; metal_range.max = 602;
        crystal_drop_rate = 10000;

        //자원 생성
        CreateSubstance(bio_range, metal_range, crystal_drop_rate);
    }

    protected override void LoadChild()
    {
        //움직임 초기화
        sp_boss_data.movement = GameDAO.SPBossData.Movement.isReady;
        move_pos_delay_timer.timer += Time.deltaTime;

        //위치 초기화
        transform.localPosition = map_manager.GetSPBossRealPoint(pos_y_offset);

        //움직임 초기화
        sp_boss_data.movement = GameDAO.SPBossData.Movement.isMove;
        move_timer.timer = 0;
    }

    protected override void Ready()
    {
        //이동
        if(move_pos_delay_timer.timer == 0)
        {
            //랜덤 위치 추가
            map_manager.RandomAreaSPBossPoint();
            //위치 초기화
            transform.localPosition = map_manager.GetSPBossRealPoint(pos_y_offset);
            //hp바 & collider true로
            hp_bar.enabled = true;
            my_collider.enabled = true;
            recognizer.enabled = true;
            //움직이는 타이머 초기화
            move_timer.timer = 0;
        }

        //움직이기까지 딜레이(여기서 움직임 초기화)
        if (move_pos_delay_timer.timer < move_pos_delay_timer.term)
            move_pos_delay_timer.timer += Time.deltaTime;
        else
        {
            //이펙트On
            dust_effect.SetActive(true);

            //원거리공격 off
            recognizer.enabled = false;

            //애니메이션
            animator.SetBool("Move", true);
            animator.SetBool("Idle", false);

            //사운드
            audio_source.PlayOneShot(move_sound);

            sp_boss_data.movement = GameDAO.SPBossData.Movement.isMove;

            move_pos_delay_timer.timer = 0;
        }
    }
    
    protected override void Move()
    {
        //일정시간 움직임(근접공격)
        if (move_timer.timer < move_timer.term)
        {
            move_timer.timer += Time.deltaTime;
            transform.Translate(sp_boss_data.speed * sp_boss_data.facing, 0, 0);
        }
        else
        {
            //hp바 & collider false로
            hp_bar.enabled = false;
            my_collider.enabled = false;
        }

        //Idle(가만히 있는 상태)까지 딜레이
        if (spawn_delay_timer.timer < spawn_delay_timer.term)
            spawn_delay_timer.timer += Time.deltaTime;
        else
        {
            //이펙트On
            dust_effect.SetActive(true);

            //애니메이션
            animator.SetBool("Move", false);
            animator.SetBool("Idle", true);

            animator.speed = 1;

            //사운드
            audio_source.PlayOneShot(move_sound);

            sp_boss_data.movement = GameDAO.SPBossData.Movement.isReady;

            spawn_delay_timer.timer = 0;
        }
    }

    protected override void RangeAttack()
    {
        //타이머 체크
        if (sp_boss_data.range_atk_timer.timer < sp_boss_data.range_atk_timer.term)
            sp_boss_data.range_atk_timer.timer += Time.deltaTime;
        else
        {
            if (shoot_num < shoot_limit)
            {
                //총알 이펙트
                if (!range_attack_effect.activeSelf && range_attack_effect != null)
                {
                    range_attack_effect.transform.localScale = new Vector3(sp_boss_data.facing, 1f, 1f);
                    range_attack_effect.SetActive(true);
                }

                //공격 사운드
                if (range_attack_sound != null)
                    audio_source.PlayOneShot(range_attack_sound);

                //총알 발사!
                ShootBullet();
                ShootBullet();
                ShootBullet();

                //발사수++
                shoot_num++;
            }
            else
            {
                shoot_num = 0;
                recognizer.enabled = false;
                sp_boss_data.movement = GameDAO.SPBossData.Movement.isReady;
            }

            sp_boss_data.range_atk_timer.timer = 0;
        }
    }

    protected override void MeleeAttack()
    {
        //공격
        if (sp_boss_data.melee_atk_timer.timer < sp_boss_data.melee_atk_timer.term)
            sp_boss_data.melee_atk_timer.timer += Time.deltaTime;
        else
        {
            //플레이어 공격중이면
            if (!attack_player)
            {
                //타겟이 죽으면 null로 초기화 및 움직임으로 변경
                if (attakc_target.state == GameDAO.BuildData.State.isDead)
                {
                    attakc_target = null;
                    sp_boss_data.movement = GameDAO.SPBossData.Movement.isMove;
                }
            }

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

    protected override void PushBoss()
    {

    }

    protected override void TriggerChild(Collider2D _col)
    {
        if (sp_boss_data.movement == GameDAO.SPBossData.Movement.isMove)
        {
            //플레이어 에게 닿으면
            if (_col.CompareTag("Player"))
            {
                attack_player = true;
                //공격 상태로 변경
                MeleeAttack();
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
                    MeleeAttack();
                }
            }
        }
    }
}
