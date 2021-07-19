using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public abstract class BossCommon : MonoBehaviour {

    //데이터
    public GameDAO.BossData boss_data = new GameDAO.BossData();
    protected bool attack_player = false;
    protected GameDAO.BuildData attakc_target = null;

    #region 참조
    protected GameDAO.PlayerData player_data;
    protected SystemDAO.MapManager map_manager;
    protected SystemDAO.GlobalState global_state;
    protected SystemDAO.FileManager file_manager;
    protected GameManager game_manager;
    private SystemDAO.GameManagerData gm_data;

    protected Transform player_transform;
    protected Rigidbody2D rb2d;
    protected Animator animator;
    protected AudioSource audio_source;
    protected SpriteRenderer sprite_renderer;
    protected Vector3 scale;

    private GameObject get_crystal_ui;
    #endregion

    //이펙트
    protected GameObject die_effect;
    protected ParticleSystem ground_effect;

    protected GameObject metal_effect;
    protected GameObject bio_effect;
    protected GameObject crystal_effect;

    //사운드
    protected AudioClip jump_up_sound = null;
    protected AudioClip jump_down_sound = null;
    protected AudioClip attack_sound = null;

    //HP바
    protected Image hp_bar;

    //플레이어와 몬스터 사이 거리
    protected Vector3 between_distance;

    //공격 타이머
    protected GameDAO.Timer atk_timer = new GameDAO.Timer(0, 0.1f);

    #region 자원
    //자원 양
    protected Define.Range bio_range = new Define.Range();
    protected Define.Range metal_range = new Define.Range();

    //자원 드랍 확률
    protected int crystal_drop_rate = 0;
    #endregion

    //초기화
    void Awake()
    {
        //참조
        player_data = GameDAO.instance.player_data;
        map_manager = SystemDAO.instance.map_manager;
        global_state = SystemDAO.instance.global_state;
        file_manager = SystemDAO.instance.file_manager;
        game_manager = GameManager.instance;
        gm_data = SystemDAO.instance.gm_data;

        player_transform = GameObject.FindGameObjectWithTag("Player").transform;
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audio_source = GetComponent<AudioSource>();
        sprite_renderer = GetComponent<SpriteRenderer>();
        scale = transform.localScale;

        get_crystal_ui = player_transform.FindChild("GetCrystal").gameObject;

        //이펙트
        bio_effect = Resources.Load("Prefab/Game/Effects/Bio_Effect") as GameObject;
        metal_effect = Resources.Load("Prefab/Game/Effects/Metal_Effect") as GameObject;
        crystal_effect = Resources.Load("Prefab/Game/Effects/Crystal_Effect") as GameObject;

        //HP바
        hp_bar = transform.FindChild("HPCanvas").FindChild("HPBar").GetComponent<Image>();

        gameObject.SetActive(false);
    }

    //Enable 될때마다 초기화
    void OnEnable()
    {
        //초기화
        Init();
    }

    //업데이트
    void FixedUpdate()
    {

        //플레이 상태 아니면 실행X
        if (gm_data.state != SystemDAO.GameManagerData.State.isPlay)
            return;

        #region 죽음/Disable/Damage 체크
        //죽으면 실행 X
        if (boss_data.state == GameDAO.BossData.State.isDead)
        {
            Die();
            return;
        }

        //가상 위치 갱신
        map_manager.SetBossPoint(transform);
        //diable point 넘어가면 쥬금!
        if (map_manager.CheckDisableBoss())
        {
            DieWithDisable();
            return;
        }

        //데미지 받으면
        if (boss_data.state == GameDAO.BossData.State.isDamaged)
        {
            Damaged();
        }
        #endregion

        #region HP바 처리
        hp_bar.fillAmount = boss_data.hp / boss_data.hp_max;
        #endregion

        #region 거리 계산 및 방향 변경

        //주인공과 몬스터 사이 거리 계산
        between_distance = transform.localPosition - player_transform.localPosition;

        //바라보는 방향 변경
        if (Vector3.Normalize(between_distance).x >= 0)
        {
            boss_data.FacingLeft();
            transform.localScale = new Vector3(boss_data.facing * scale.x, scale.y, scale.z);
        }
        else
        {
            boss_data.FacingRight();
            transform.localScale = new Vector3(boss_data.facing * scale.x, scale.y, scale.z);
        }

        #endregion

        #region 행동/패턴
        switch (boss_data.movement)
        {
            //대기
            case GameDAO.BossData.Movement.isReady:

                break;
            //움직일때
            case GameDAO.BossData.Movement.isMove:
                //애니메이션
                animator.SetBool("Move", true);
                animator.SetBool("Attack", false);

                //움직임
                if (boss_data.state == GameDAO.BossData.State.isAlive)
                    Move();

                break;
            //근접공격
            case GameDAO.BossData.Movement.isAttack:
                //플레이어가 생존 상태 아니면 진행X
                if (player_data.state != GameDAO.PlayerData.State.isAlive)
                    break;

                //애니메이션
                animator.SetBool("Move", false);
                animator.SetBool("Attack", true);

                //공격
                if (atk_timer.timer < atk_timer.term)
                    atk_timer.timer += Time.deltaTime;
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
                            boss_data.movement = GameDAO.BossData.Movement.isMove;
                        }
                    }

                    Attack();

                    atk_timer.timer = 0;
                }
                break;
        }
        #endregion
    }

    #region 공통
    //자원 랜덤 생성
    protected void CreateSubstance(Define.Range bio, Define.Range metal, int _crystal_drop_rate)
    {
        //0 ~ 9999 까지 랜덤 생성
        int chance = Random.Range(0, 10000);

        boss_data.give_subdata.bio = Random.Range(bio.min, bio.max);

        boss_data.give_subdata.metal = Random.Range(metal.min, metal.max);

        if (chance < _crystal_drop_rate)
            boss_data.give_subdata.crystal = 1;
        else
            boss_data.give_subdata.crystal = 0;
    }

    //피격
    protected void Damaged()
    {
        //밀리기(한번만 실행)
        if (boss_data.damage_timer.timer == 0)
        {
            sprite_renderer.color = new Color(1.0f, 1.0f, 1.0f, 0.6f);
            PushBoss();
        }

        if (boss_data.damage_timer.timer < boss_data.damage_timer.term)
            boss_data.damage_timer.timer += Time.deltaTime;
        else
        {
            //원래 색으로
            sprite_renderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            //상태 되돌림
            boss_data.state = GameDAO.BossData.State.isAlive;
            //시간 초기화
            boss_data.damage_timer.timer = 0f;
        }
    }

    //죽음
    protected void Die()
    {
        //상속용
        DieChild();

        //죽는 이펙트 생성
        GameObject effect = Instantiate(die_effect);
        effect.transform.localPosition = transform.localPosition;

        //플레이어에게 자원++
        player_data.my_subdata.AddAll(boss_data.give_subdata);
        //플레이어 크리스탈 획득 이펙트
        if (boss_data.give_subdata.crystal > 0 && !get_crystal_ui.activeSelf)
            get_crystal_ui.SetActive(true);

        //자원 이펙트 생성
        if (boss_data.give_subdata.bio > 0)
        {
            GameObject sub_effect = Instantiate(bio_effect);
            sub_effect.transform.localPosition = transform.localPosition;

            int substance = boss_data.give_subdata.bio;
            if (substance > 0)
                sub_effect.GetComponent<ParticleSystem>().Emit(substance);
        }
        if (boss_data.give_subdata.metal > 0)
        {
            GameObject sub_effect = Instantiate(metal_effect);
            sub_effect.transform.localPosition = transform.localPosition;

            int substance = boss_data.give_subdata.metal;
            if (substance > 0)
                sub_effect.GetComponent<ParticleSystem>().Emit(substance);
        }
        if (boss_data.give_subdata.crystal > 0)
        {
            GameObject sub_effect = Instantiate(crystal_effect);
            sub_effect.transform.localPosition = transform.localPosition;

            int substance = boss_data.give_subdata.crystal;
            if (substance > 0)
                sub_effect.GetComponent<ParticleSystem>().Emit(substance);
        }

        //위치 변경
        if (transform.localPosition.x >= 0)
            transform.localPosition = Define.init_pos_plus;
        else
            transform.localPosition = Define.init_pos_minus;


        //가상 위치 제거
        map_manager.DestroyBossPoint();
        //게임메니저에서 몬스터 제거
        game_manager.DestroyBoss();

        //엑티브 false로
        gameObject.SetActive(false);
    }

    //disable point에 닿았을 시 실행
    public void DieWithDisable()
    {
        //이펙트 생성
        GameObject effect = Instantiate(die_effect);
        effect.transform.localPosition = transform.localPosition;
        effect.GetComponent<MonsterParticle>().die_with_disable = true;

        //위치 변경
        if (transform.localPosition.x >= 0)
            transform.localPosition = Define.init_pos_plus;
        else
            transform.localPosition = Define.init_pos_minus;

        //가상 위치 제거
        map_manager.DestroyBossPoint();
        //게임메니저에서 몬스터 제거
        game_manager.DestroyBoss();

        //엑티브 false로
        gameObject.SetActive(false);
    }

    //리젠
    public void Regen()
    {
        //움직임 / 상태 초기화
        boss_data.movement = GameDAO.BossData.Movement.isMove;
        boss_data.state = GameDAO.BossData.State.isAlive;
        //랜덤 위치 추가
        map_manager.AddRandomBossPoint(global_state.level);
        //위치 초기화
        transform.localPosition = map_manager.GetBossRealPoint();
        //중력 초기화
        rb2d.gravityScale = 1f;
        //엑티브 true
        gameObject.SetActive(true);
    }

    //로드
    public void Load(GameDAO.BossData _boss_data)
    {
        //엑티브 true
        gameObject.SetActive(true);
        //데이터 초기화
        boss_data = _boss_data;
        //움직임 초기화
        boss_data.movement = GameDAO.BossData.Movement.isMove;
        //위치 초기화
        transform.localPosition = map_manager.GetBossRealPoint();
        //중력 초기화
        rb2d.gravityScale = 1f;
    }

    //공격
    protected void Attack()
    {
        //공격!
        if (attack_player)
            player_data.SubHP(boss_data.atk);
        else
        {
            if (attakc_target != null)
                attakc_target.SubHP(boss_data.atk);
            else
                boss_data.movement = GameDAO.BossData.Movement.isMove;
        }

        AttackChild();
    }

    //죽는 이펙트 초기화
    protected void DieEffectInit(string _effect)
    {
        die_effect = Resources.Load<GameObject>("Prefab/Game/Effects/" + _effect);
    }

    #endregion

    #region 상속

    protected abstract void Init();

    protected abstract void Move();

    protected virtual void DieChild() { }

    protected virtual void AttackChild() { }
    #endregion

    #region 피격/공격
    //플레이어 밀기
    public void PushPlayer()
    {
        //공중에 뜨니까 grounded -> false로
        player_data.grounded = false;
        //밀려나기
        player_transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        player_transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(50f * boss_data.facing, 50f));
    }

    //몬스터(스스로) 밀기
    protected void PushBoss()
    {
        //밀려나기(자기가 가는 반대방향으로)
        rb2d.velocity = Vector2.zero;
        rb2d.AddForce(new Vector2(40f * -boss_data.facing, 40f));
    }

    void OnTriggerEnter2D(Collider2D _col)
    {
        //땅에 닿으면
        if (_col.CompareTag("Ground"))
        {
            rb2d.gravityScale = 0;
            rb2d.velocity = Vector2.zero;

            //이펙트 있으면 실행
            if (ground_effect != null)
                ground_effect.gameObject.SetActive(true);

            //사운드 있으면 실행
            if (jump_down_sound != null)
                audio_source.PlayOneShot(jump_down_sound);
        }
    }

    void OnTriggerStay2D(Collider2D _col)
    {
        //플레이어 에게 닿으면
        if (_col.CompareTag("Player"))
        {
            attack_player = true;
            //공격 상태로 변경
            boss_data.movement = GameDAO.BossData.Movement.isAttack;
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
                boss_data.movement = GameDAO.BossData.Movement.isAttack;
            }
        }
    }

    void OnTriggerExit2D(Collider2D _col)
    {
        //땅을 벗어나면 다시 중력 적용
        if (_col.CompareTag("Ground"))
        {
            rb2d.gravityScale = 1;
        }

        //플레이어에게서 벗어나면 공격 해제
        if (_col.CompareTag("Player"))
        {
            boss_data.movement = GameDAO.BossData.Movement.isMove;
        }

        //건물에게서 벗어나면 공격 해제
        if (_col.CompareTag("Build"))
        {
            //타겟 지정
            attakc_target = null;
            boss_data.movement = GameDAO.BossData.Movement.isMove;
        }
    }
    #endregion
}
