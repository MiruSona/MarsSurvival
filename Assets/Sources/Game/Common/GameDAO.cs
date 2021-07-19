using UnityEngine;
using System.Collections.Generic;


public class GameDAO : SingleTon<GameDAO> {
    
    #region 플레이어

    //플레이어 데이터
    public class PlayerData
    {
        #region 변수

        //스탯
        public float hp = 1200f;        //체력
        public float max_hp = 1200f;    //최대 체력
        public float speed = 50f;       //속도
        public float max_speed = 70f;   //최대 속도
        public int facing = 1;          //방향(1, -1)
        public bool grounded = false;    //땅에 있는지 여부

        //자원
        public SubstanceData my_subdata = new SubstanceData();
        public Substance target_substance = null;
        public BigSubstance target_bigsub = null;

        //도구 관련
        public ToolData tool_data = new ToolData();
        public Timer tool_timer = new Timer(0f, 1.5f);

        //무기
        public WeaponData weapon_data = new WeaponData();
        public bool attacking = false;  //공격 중 여부
        public Timer attack_timer;
        public Timer reload_timer = new Timer(0f, 0.7f);

        //피격 타이머
        public Timer damaged_timer = new Timer(0f, 1.5f);

        //중독 타이머
        public Timer poisoned_timer = new Timer(0f, 90.0f);

        //워프 올라가는거 표시
        public bool warp_up = false;

        //움직임
        public enum Movement
        {
            isReady,            //대기
            isMove,             //움직임 초기화
            isMoveSchedule,     //움직임
            isGather,           //채취 초기화
            isGatherSchedule,   //채취
            isRepair,           //수리
            isRepairSchedule,   //수리중
            isAttack,           //공격 초기화
            isAttackSchedule    //공격
        }
        public Movement movement = Movement.isReady;

        //상태
        public enum State
        {
            isAlive,
            isDamaged,
            isPoisoned,
            isDead
        }
        public State state = State.isAlive;
        #endregion

        #region 함수

        //생성자
        public PlayerData()
        {
            //공격 타이머
            attack_timer = new Timer(weapon_data.shoot_speed, weapon_data.shoot_speed);
        }

        //다이어리 아티펙트
        public void ArtifactDiary(float _value)
        {
            //작동시 체력 * 1.2
            hp *= _value;
            max_hp *= _value;
        }

        //발자국 아티펙트
        public void ArtifactFootPrint(float _value)
        {
            //작동시 이속 * 1.2
            speed *= _value;
            max_speed *= _value;
        }

        //방향 전환
        public void FacingRight()
        {
            facing = 1;
        }

        public void FacingLeft()
        {
            facing = -1;
        }

        //체력 처리 함수
        public void SubHP(float _amount)
        {
            //살아있는 상태가 아니면 실행 X
            if (state != State.isAlive)
                return;

            //방어구 처리
            SystemDAO.FileManager file_manager = SystemDAO.instance.file_manager;
            float damage = _amount;
            switch (file_manager.cloth_buy)
            {
                case Define.NORMAL_CLOTH:
                    damage *= (1.0f - Define.normal_cloth);
                    break;

                case Define.SPECIAL_CLOTH:
                    damage *= (1.0f - Define.special_cloth);
                    break;
            }

            //HP가 0보다 작거나 같으면
            if (hp - damage <= 0)
            {
                //HP를 0으로 만들고 죽음상태 표시
                hp = 0;
                state = State.isDead;
            }
            else //HP가 0보다 많으면
            {
                //값만큼 빼준다
                hp -= damage;
                state = State.isDamaged;
            }
        }

        //체력 처리 함수
        public void AddHP(float _amount)
        {
            //죽은 상태에선 실행 X
            if (state == State.isDead)
                return;

            //HP가 최대치보다 많거나 같으면
            if (hp + _amount >= max_hp)
            {
                //HP를 최대치로
                hp = max_hp;
            }
            else //HP가 최대치 보다 적으면
            {
                //값만큼 더해준다
                hp += _amount;
            }
        }
        #endregion
    }
    public PlayerData player_data = new PlayerData();

    //도구 데이터
    public class ToolData
    {
        public int amount = 2;      //채취 양
        public int step = 0;        //진화 정도
        public float[] speed =      //채취 속도
            new float[Define.tool_step_max] 
            {
                1f,
                1.5f,
                2.5f
            };    
        public Vector3 target_position = Vector3.zero;  //대상 위치

        //업글 시
        public SubstanceData[] upgrade_need = 
            new SubstanceData[Define.tool_step_max]
            {
                new SubstanceData(50,200,0),
                new SubstanceData(150,750,0),
                new SubstanceData(0,0,0)
            };

        //스탭 별 변화
        public Sprite[] sprites = new Sprite[Define.tool_step_max];
        public Color[] colors = 
            new Color[Define.tool_step_max] {
                new Color(0.0f, 0.8f, 0.5f, 0.7f),
                new Color(1.0f, 1.0f, 0.0f, 0.8f),
                new Color(0.7f, 0.1f, 0.0f, 0.9f)
            };
        public float[] line_width =
            new float[Define.tool_step_max]
            {
                0.05f,
                0.07f,
                0.12f
            };

        //파편 아티펙트
        public void ArtifactFragment(float _value)
        {
            for (int i = 0; i < speed.Length; i++)
                speed[i] *= _value;
        }
    }

    //무기 데이터
    public class WeaponData
    {
        //공격력
        public float[,] atk =
            new float[Define.weapon_step_max, Define.weapon_level_max] {
                    { 40f, 45f, 52f },
                    { 60f, 67f, 75f },
                    { 86f, 94f, 103f }
                };
        public int level = 0;                //강화 정도
        public int step = 0;                 //진화 정도
        public float shoot_speed = 0.4f;     //공격 속도
        public float[] reload_speed =        //장전 속도
            new float[Define.weapon_step_max]
            {
                1f, 1.3f, 1.6f
            };

        public float bullet_num = 0;        //장전된 총알 수
        public float bullet_max = 10;       //총알 최대치
        public float bullet_speed = 1000f;  //공격 속도

        //업글 시
        public SubstanceData[,] upgrade_need = 
            new SubstanceData[Define.weapon_step_max, Define.weapon_level_max] {
                    { new SubstanceData(45,0,0), new SubstanceData(54,0,0), new SubstanceData(76,0,0) },
                    { new SubstanceData(108,0,0), new SubstanceData(150,0,0), new SubstanceData(240,0,0) },
                    { new SubstanceData(384,0,0), new SubstanceData(616,0,0), new SubstanceData(0,0,0) }
                };

        //스탭 별 변화
        public Sprite[] sprites = new Sprite[Define.weapon_step_max];
        public Color[] colors =
            new Color[Define.weapon_step_max] {
                new Color(1.0f, 1.0f, 0.0f, 0.9f),
                new Color(1.0f, 0.5f, 0.0f, 0.9f),
                new Color(1.0f, 0.1f, 0.0f, 0.9f)
            };

        //묘지 아티펙트
        public void ArtifactGraveStone(float _value)
        {
            for (int i = 0; i < Define.weapon_step_max; i++)
                for (int j = 0; j < Define.weapon_level_max; j++)
                    atk[i, j] *= _value;
        }
    }
    #endregion
    
    #region 빌드

    //빌드 공통
    [System.Serializable]
    public class BuildData
    {
        #region 변수

        //인덱스
        public int index = -1;

        //방향
        public int facing = 1;

        //기본 정보
        public int step = 0;                    //진화 정도

        public float hp = 0f;                   //현재 hp
        public float[] hp_max;                  //최대 hp

        public float[] atk;                     //공격력

        //타이머
        public Timer atk_timer;                 //공격 타이머
        public Timer damage_timer;              //데미지 타이머

        //상태
        public enum State
        {
            isAlive,
            isDamaged,
            isDead
        }
        public State state = State.isAlive;

        //움직임
        public enum Movement
        {
            isReady,
            isAttack
        }
        public Movement movement = Movement.isReady;

        //풀
        public enum Pool
        {
            Turret,
            Shield,
            Mine
        }
        public Pool pool = Pool.Turret;

        //자원 필요량
        //빌드 시
        public SubstanceData[] build_need;

        //업글 시
        public SubstanceData[] upgrade_need;

        //참조
        public Sprite[] sprites = new Sprite[Define.build_step_max];

        #endregion

        //체력 감소
        public void SubHP(float _amount)
        {
            //살아있는 상태가 아니면 실행 X
            if (state != State.isAlive)
                return;

            //HP가 0보다 작거나 같으면
            if (hp - _amount <= 0)
            {
                //HP를 0으로 만들고 죽음상태 표시
                hp = 0;
                state = State.isDead;
            }
            else //HP가 0보다 많으면
            {
                //값만큼 빼준다
                hp -= _amount;
                state = State.isDamaged;
            }
        }
    }
    public BuildData build_data;

    //터렛
    [System.Serializable]
    public class TurretData : BuildData
    {
        //초기화
        public TurretData()
        {
            atk = new float[Define.build_step_max] { 80f, 115f, 165f };
            hp_max = new float[Define.build_step_max] { 400f, 600f, 800f };
            hp = hp_max[step];

            atk_timer = new Timer(1.8f, 1.8f);
            damage_timer = new Timer(0, 1.0f);

            build_need = new SubstanceData[Define.build_step_max]
            {
                new SubstanceData(40,0,0),
                new SubstanceData(55,0,0),
                new SubstanceData(75,0,0)
            };

            upgrade_need = new SubstanceData[Define.build_step_max]
            {
                new SubstanceData(95,0,0),
                new SubstanceData(235,0,0),
                new SubstanceData(0,0,0)
            };
        }

    }
    public TurretData turret_data = new TurretData();

    //실드
    [System.Serializable]
    public class ShieldData : BuildData
    {
        //초기화
        public ShieldData()
        {
            hp_max = new float[Define.build_step_max] { 500f, 850f, 1300f };
            hp = hp_max[step];

            damage_timer = new Timer(0, 1.0f);

            build_need = new SubstanceData[Define.build_step_max]
            {
                new SubstanceData(10,30,0),
                new SubstanceData(15,40,0),
                new SubstanceData(25,60,0)
            };

            upgrade_need = new SubstanceData[Define.build_step_max]
            {
                new SubstanceData(95,120,0),
                new SubstanceData(235,300,0),
                new SubstanceData(0,0,0)
            };
        }
    }
    public ShieldData shield_data = new ShieldData();

    //지뢰
    [System.Serializable]
    public class MineData : BuildData
    {
        public MineData()
        {
            atk = new float[1] { 1300f };

            atk_timer = new Timer(0f, 1f);

            build_need = 
                new SubstanceData[1] {
                    new SubstanceData(0, 0, 1)
                };
        }
    }
    public MineData mine_data = new MineData();

    #endregion

    #region 소모품

    //소모품 공통
    public class ConsumeData
    {
        public float recovery;
        public float atk;
        public Timer atk_timer;
        public SubstanceData buy_need;
    }

    //식량
    public class MealData : ConsumeData
    {
        public MealData()
        {
            recovery = 400f;
            buy_need = new SubstanceData(0, 31, 0);
        }

        //아티펙트 체크
        public void ArtifactAmber(float _value)
        {
            recovery += _value;
        }
    }
    public MealData meal_data = new MealData(); 

    //워프
    public class WarpData : ConsumeData
    {
        public WarpData()
        {
            buy_need = new SubstanceData(0, 0, 1);
        }
    }
    public WarpData warp_data = new WarpData();

    #endregion

    #region 버프
    //버프 데이터
    public class BuffData
    {      
        //버프 종류
        public int buff_kind = Define.BuffAttack;

        //아이콘
        public Sprite[] icons = new Sprite[Define.buff_kind_num];

        //효과량
        public float value = 1.3f;

        //지속 시간(타이머)
        public Timer buff_timer = new Timer(0, 120f);

        //버프 작동 중인지
        public bool enable = false;

        //적용 여부
        public bool apply = false;
        
        //초기화 여부
        public bool init = false;

        //버프 이미 적용했는지 여부
        public bool already_clicked;
    }
    public BuffData buff_data = new BuffData();
    #endregion

    #region 몬스터

    //몬스터 데이터 클래스
    [System.Serializable]
    public class MonsterData
    {
        #region 변수
        //인덱스
        public int index = -1;

        //스탯
        public float hp = 0;
        public float speed = 0;
        public int facing = 1;
        public SubstanceData give_subdata = new SubstanceData();

        //공격관련
        public float atk_melee = 0;     //근거리 공격력
        public float atk_range = 0;      //원거리 공격력
        public Timer atk_range_timer = new Timer();

        //피격 타이머
        public Timer damage_timer = new Timer(0f, 0.1f);

        //몬스터 상태
        public enum State
        {
            isAlive,
            isDamaged,
            isDead
        }
        public State state = State.isAlive;

        //몬스터 움직임
        public enum Movement
        {
            isReady,
            isMove,
            isAttackMelee,    //근거리 공격
            isAttackRange      //원거리 공격
        }
        public Movement movement = Movement.isMove;

        //오브젝트 풀
        public enum Pool
        {
            Slime_Weak_Melee,
            Slime_Weak_Range,
            Slime_Normal_Melee,
            Slime_Normal_Range,
            Slime_Strong_Melee,
            Slime_Strong_Range,
            Larva_Weak,
            Larva_Normal,
            SandMonster_Weak,
            SandMonster_Normal
        }
        public Pool pool = Pool.Slime_Weak_Melee;
        #endregion

        #region 함수
        //방향 전환
        public void FacingRight()
        {
            facing = 1;
        }

        public void FacingLeft()
        {
            facing = -1;
        }

        //체력 감소
        public void SubHP(float _amount)
        {
            //살아있는 상태가 아니면 실행 X
            if (state != State.isAlive)
                return;

            //HP가 0보다 작거나 같으면
            if (hp - _amount <= 0)
            {
                //HP를 0으로 만들고 죽음상태 표시
                hp = 0;
                state = State.isDead;
            }
            else //HP가 0보다 많으면
            {
                //값만큼 빼준다
                hp -= _amount;
                state = State.isDamaged;
            }
        }
        #endregion
    }

    //몬스터 소환확률 
    public class MonsterSpawnRate
    {
        #region 변수
        public struct SectorRate
        {
            public int[] sector;

            public SectorRate(int _sector0, int _sector1, int _sector2)
            {
                sector = new int[Define.sector_total_num]
                {
                    _sector0,
                    _sector1,
                    _sector2
                };
            }
        }

        public SectorRate[] global_state = new SectorRate[4]
        {
            new SectorRate(0,0,0),
            new SectorRate(0,0,0),
            new SectorRate(0,0,0),
            new SectorRate(0,0,0)
        };
        #endregion

        #region 함수
        //확률 설정 -> 0,1,2 구역 / 현재 날씨+밤낮상태
        public void SetRate(int _sector0, int _sector1, int _sector2, int _global_state)
        {
            global_state[_global_state].sector[0] = _sector0;
            global_state[_global_state].sector[1] = _sector1;
            global_state[_global_state].sector[2] = _sector2;
        }

        //모든값에 받아온값++
        public void AddAllRate(SectorRate[] _global_state)
        {
            for (int state = 0; state < 4; state++)
                for (int sector = 0; sector < 3; sector++)
                    global_state[state].sector[sector] += _global_state[state].sector[sector];
        }

        //체크값이 확률 안에 포함되나 체크
        public bool CheckRate(int _min, int _check_value, int _player_sector, int _global_state)
        {
            bool send_bool = false;
            //최소 <= 체크할값 <= 최소 + 상태에 따른 최대 값  --> 이럴 경우 true
            if (_min <= _check_value && _check_value < _min + global_state[_global_state].sector[_player_sector])
            {
                send_bool = true;
            }
            return send_bool;
        }
        #endregion
    }
    #endregion

    #region 보스

    [System.Serializable]
    public class BossData
    {
        #region 변수

        //스탯
        public float hp_max = 100f;
        public float hp = 0;
        public float speed_x = 0;
        public float speed_y = 0;
        public int facing = 1;
        public SubstanceData give_subdata = new SubstanceData();

        //공격관련
        public float atk = 0;
        public Timer atk_timer = new Timer(0f, 0f);

        //피격 타이머
        public Timer damage_timer = new Timer(0f, 0.2f);

        //움직임 타이머
        public Timer move_timer = new Timer(0f, 0f);

        //보스 상태
        public enum State
        {
            isAlive,
            isDamaged,
            isDead
        }
        public State state = State.isAlive;

        //보스 움직임
        public enum Movement
        {
            isReady,
            isMove,
            isAttack,
        }
        public Movement movement = Movement.isMove;

        //보스 풀
        public enum Pool
        {
            Rock_Boss,
            Tree_Boss
        }
        public Pool pool = Pool.Rock_Boss;
        #endregion

        #region 함수
        //방향 전환
        public void FacingRight()
        {
            facing = 1;
        }

        public void FacingLeft()
        {
            facing = -1;
        }

        //체력 감소
        public void SubHP(float _amount)
        {
            //살아있는 상태가 아니면 실행 X
            if (state != State.isAlive)
                return;

            //HP가 0보다 작거나 같으면
            if (hp - _amount <= 0)
            {
                //HP를 0으로 만들고 죽음상태 표시
                hp = 0;
                state = State.isDead;
            }
            else //HP가 0보다 많으면
            {
                //값만큼 빼준다
                hp -= _amount;
                state = State.isDamaged;
            }
        }
        #endregion
    }
    #endregion
    
    #region 특수 보스
    [System.Serializable]
    public class SPBossData
    {
        #region 변수
        //스탯
        public float hp_max = 100f;
        public float hp = 100f;
        public float speed = 0.001f;
        public int facing = 1;
        public SubstanceData give_subdata = new SubstanceData();

        //공격관련
        public float atk = 1f;
        public Timer melee_atk_timer = new Timer(0f, 0.1f);
        public Timer range_atk_timer = new Timer(0f, 1f);

        //피격 타이머
        public Timer damage_timer = new Timer(0f, 0.2f);

        //보스 상태
        public enum State
        {
            isAlive,
            isDamaged,
            isDead
        }
        public State state = State.isAlive;

        //보스 움직임
        public enum Movement
        {
            isReady,
            isMove,
            isMeleeAttack,
            isRangeAttack
        }
        public Movement movement = Movement.isMove;
        #endregion

        #region 함수
        //방향 전환
        public void FacingRight()
        {
            facing = 1;
        }

        public void FacingLeft()
        {
            facing = -1;
        }

        //체력 감소
        public void SubHP(float _amount)
        {
            //살아있는 상태가 아니면 실행 X
            if (state != State.isAlive)
                return;

            //HP가 0보다 작거나 같으면
            if (hp - _amount <= 0)
            {
                //HP를 0으로 만들고 죽음상태 표시
                hp = 0;
                state = State.isDead;
            }
            else //HP가 0보다 많으면
            {
                //값만큼 빼준다
                hp -= _amount;
                state = State.isDamaged;
            }
        }
        #endregion
    }
    #endregion

    #region 자원
    //자원 데이터 구조체
    [System.Serializable]
    public class SubstanceData
    {
        public int metal = 0;       //금속
        public int bio = 0;         //바이오
        public int crystal = 0;     //크리스탈

        #region 함수
        //생성자
        public SubstanceData()
        {
            Init();
        }

        public SubstanceData(int _metal, int _bio, int _crystal)
        {
            metal = _metal;
            bio = _bio;
            crystal = _crystal;
        }

        //초기화
        public void Init()
        {
            metal = 0;       //금속
            bio = 0;         //바이오
            crystal = 0;     //크리스탈
        }

        //-------------------- 데이터 --------------------
        //단순히 받아온 값을 빼주는거
        public void SubAll(SubstanceData _sub)
        {
            if (metal - _sub.metal < 0)
                metal = 0;
            else
                metal -= _sub.metal;

            if (bio - _sub.bio < 0)
                bio = 0;
            else
                bio -= _sub.bio;

            if (crystal - _sub.crystal < 0)
                crystal = 0;
            else
                crystal -= _sub.crystal;
        }

        //단순히 받아온 값을 더해주는거
        public void AddAll(SubstanceData _sub)
        {
            if (!CheckFull(_sub.metal, 0))
                metal += _sub.metal;
            else
                metal = 9999;

            if (!CheckFull(_sub.bio, 1))
                bio += _sub.bio;
            else
                bio = 9999;
            
            if (!CheckFull(_sub.crystal, 2))
                crystal += _sub.crystal;
            else
                crystal = 9999;
        }

        //값 복사
        public void CopyAll(SubstanceData _sub)
        {
            metal = _sub.metal;
            bio = _sub.bio;
            crystal = _sub.crystal;
        }

        //-------------------- 체크문 --------------------
        //모든값이 0인지 체크
        public bool CheckZero()
        {
            bool check = false;

            if ((metal == 0) && (bio == 0) && (crystal == 0))
                check = true;

            return check;
        }

        //금속과 바이오만 0인지 체크
        public bool CheckMetalBioZero()
        {
            bool check = false;

            if ((metal == 0) && (bio == 0))
                check = true;

            return check;
        }

        //가지고 있는 자원이 더 많은지 비교
        public bool CheckMore(SubstanceData _sub_data)
        {
            bool send_bool = false;

            if (metal >= _sub_data.metal
                && bio >= _sub_data.bio
                && crystal >= _sub_data.crystal)
                send_bool = true;

            return send_bool;
        }

        //꽉찼는지 체크 -> 0 = metal / 1 = bio / 2 = crystal
        public bool CheckFull(int _value, int _kind)
        {
            bool send_bool = false;

            switch (_kind)
            {
                case 0:
                    if (metal + _value > Define.player_sub_max)
                        send_bool = true;
                    break;

                case 1:
                    if (bio + _value > Define.player_sub_max)
                        send_bool = true;
                    break;

                case 2:
                    if (crystal + _value > Define.player_sub_max)
                        send_bool = true;
                    break;
            }

            return send_bool;
        }
        #endregion
    }
    #endregion
    
    #region 우주선
    public class SpaceShipData
    {
        //스탯
        public int step = 0;
        public float current_hp = 0f;
        public float current_max_hp = 100f;
        public float step_hp = 0f;
        public float step_max_hp = 100f * Define.spaceship_step;

        //필요 자원
        public SubstanceData[] repair_need =
            new SubstanceData[Define.spaceship_step]
            {
                new SubstanceData( 1, 1, 0 ),
                new SubstanceData( 3, 3, 0 ),
                new SubstanceData( 10, 10, 0 ),
                new SubstanceData( 15, 15, 0 ),
                new SubstanceData( 20, 20, 0 )
            };

        //참조
        public Sprite[] sprites = new Sprite[Define.spaceship_step + 1];

        //스프라이트 색
        public Color repair_color = new Color(0.6f, 0.6f, 1f);

        public SpaceShipData()
        {
            current_max_hp = step_max_hp / Define.spaceship_step;
        }

        public void SetCurrentHP(int step)
        {
            if (step < sprites.Length)
                current_hp = step_hp - (step * current_max_hp);
            else
                current_hp = step_hp - ((step - 1) * current_max_hp);
        }
    }
    public SpaceShipData spaceship_data = new SpaceShipData();
    #endregion

    #region 아티팩트
    [System.Serializable]
    public struct Artifact
    {
        public string name;
        public bool activate;
        public float value;
        public string description;

        public Artifact(string _name, float _value)
        {
            name = _name;
            activate = false;
            value = _value;
            description = "";
        }
    }

    public class ArtifactData
    {
        #region 변수
        public Artifact[] artifcat =
            new Artifact[Define.artifact_num]
            {                
                new Artifact("GraveStone", 1.5f),   //공격 50%
                new Artifact("FootPrint", 1.2f),    //이속 20%
                new Artifact("Fragment", 1.2f),     //도구 속도 20%
                new Artifact("Diary", 1.2f),        //체력 20%
                new Artifact("Amber", 100f)         //식량 +100
            };

        //얻은 아티펙트 리스트
        public List<Artifact> get_artifact_list = new List<Artifact>();

        //스프라이트
        public Sprite[] sprites = new Sprite[Define.artifact_num];
        #endregion

        #region 함수

        //생성자
        public ArtifactData()
        {
            artifcat[0].description = "공격력 50% 증가";
            artifcat[1].description = "움직이는 속도 20% 증가";
            artifcat[2].description = "도구 속도 20% 증가";
            artifcat[3].description = "체력 20% 증가";
            artifcat[4].description = "고구마 회복량 +100";
        }

        //아티펙트 설정
        public void SetArtifact(int _clear_num)
        {
            get_artifact_list.Clear();
            for (int i = 0; i < _clear_num; i++)
            {
                if (i < artifcat.Length)
                    get_artifact_list.Add(artifcat[i]);
                else
                    break;
            }
        } 

        #endregion
    }
    public ArtifactData artifact_data = new ArtifactData();
    #endregion

    //----------------------- 공용 -----------------------
    //타이머
    [System.Serializable]
    public struct Timer
    {
        public float term;      //간격
        public float timer;     //타이머

        public Timer(float _timer, float _term)
        {
            term = _term;
            timer = _timer;
        }
    }

    //초기화
    void Awake()
    {
        //파괴 안되게
        DontDestroyOnLoad(gameObject);        

        //조명 안꺼지게
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        
        //마켓 서비스 초기화
        MarketService.instance.playerData = instance.player_data;
        MarketService.instance.file_manager = SystemDAO.instance.file_manager;
        
        #region 스프라이트 초기화
        //우주선
        for (int i = 0; i < Define.spaceship_step + 1; i++)
            spaceship_data.sprites[i] = Resources.Load<Sprite>("Sprite/Game/Object/SS0" + i);

        //무기
        for (int i = 0; i < Define.weapon_step_max; i++)
            player_data.weapon_data.sprites[i] = Resources.Load<Sprite>("Sprite/Game/Player/LaserGun_" + i);

        //도구
        for (int i = 0; i < Define.tool_step_max; i++)
            player_data.tool_data.sprites[i] = Resources.Load<Sprite>("Sprite/Game/Player/Tool_" + i);

        //터렛
        for (int i = 0; i < Define.build_step_max; i++)
            turret_data.sprites[i] = Resources.Load<Sprite>("Sprite/Game/Build/Turret_" + i);

        //실드
        for (int i = 0; i < Define.build_step_max; i++)
            shield_data.sprites[i] = Resources.Load<Sprite>("Sprite/Game/Build/Shield_" + i);

        //아티펙트
        for (int i = 0; i < Define.artifact_num; i++)
            artifact_data.sprites[i] = Resources.Load<Sprite>("Sprite/Main/Artifacts/Artifact_" + i);

        //버프
        for (int i = 0; i < Define.buff_kind_num; i++)
            buff_data.icons[i] = Resources.Load<Sprite>("Sprite/Game/UI/BuffIcon_" + i);
        #endregion
    }
}
