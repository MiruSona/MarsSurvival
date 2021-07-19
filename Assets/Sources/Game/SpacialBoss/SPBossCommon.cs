using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public abstract class SPBossCommon : MonoBehaviour {

    //데이터
    [System.NonSerialized]
    public GameDAO.SPBossData sp_boss_data = new GameDAO.SPBossData();
    protected bool attack_player = false;
    protected GameDAO.BuildData attakc_target = null;

    #region 참조
    protected GameDAO.PlayerData player_data;
    protected SystemDAO.MapManager map_manager;
    private SystemDAO.GlobalState global_state;
    protected SystemDAO.FileManager file_manager;
    private GameManager game_manager;
    private SystemDAO.GameManagerData gm_data;

    private Transform player_transform;
    private Rigidbody2D rb2d;
    protected Animator animator;
    protected AudioSource audio_source;
    private SpriteRenderer sprite_renderer;
    private Vector3 scale;

    private GameObject get_crystal_ui;
    #endregion

    #region 이펙트
    protected GameObject melee_attack_effect;
    protected GameObject range_attack_effect;

    private GameObject metal_effect;
    private GameObject bio_effect;
    private GameObject crystal_effect;

    private ParticleSystem die_effect;
    private GameObject spawn_effect;
    #endregion

    //사운드
    protected AudioClip die_sound = null;
    protected AudioClip move_sound = null;
    protected AudioClip melee_attack_sound = null;
    protected AudioClip range_attack_sound = null;
    private AudioClip spawn_sound = null;

    #region 총알 관련
    [System.NonSerialized]
    public Stack<SPBossBullet> bullet_stack = new Stack<SPBossBullet>();
    protected GameObject bullet_prefab = null;
    protected Transform fire_pos;
    #endregion

    //HP바
    protected Image hp_bar = null;

    //플레이어와 몬스터 사이 거리
    private Vector3 between_distance;

    #region 자원
    //자원 양
    protected Define.Range bio_range = new Define.Range();
    protected Define.Range metal_range = new Define.Range();

    //자원 드랍 확률
    protected int crystal_drop_rate = 0;
    #endregion

    //죽는 이펙트 이미 실행했는지 체크
    private bool done_die_effect = false;

    //초기화
    void Awake()
    {
        #region 참조
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
        #endregion

        //이펙트
        bio_effect = Resources.Load("Prefab/Game/Effects/Bio_Effect") as GameObject;
        metal_effect = Resources.Load("Prefab/Game/Effects/Metal_Effect") as GameObject;
        crystal_effect = Resources.Load("Prefab/Game/Effects/Crystal_Effect") as GameObject;

        die_effect = transform.FindChild("Die_Effect").GetComponent<ParticleSystem>();
        spawn_effect = transform.FindChild("Spawn_Effect").gameObject;

        //HP바
        hp_bar = transform.FindChild("HPCanvas").FindChild("HPBar").GetComponent<Image>();

        //사운드 초기화
        spawn_sound = Resources.Load<AudioClip>("Sound/SoundEffect/SPBossSpawn");

        Init();

        gameObject.SetActive(false);
    }

    //업데이트
    void FixedUpdate()
    {
        //플레이 상태 아니면 실행X
        if (gm_data.state != SystemDAO.GameManagerData.State.isPlay)
            return;

        #region 상태 체크
        switch (sp_boss_data.state)
        {
            case GameDAO.SPBossData.State.isDamaged:
                Damaged();
                break;

            case GameDAO.SPBossData.State.isDead:
                Die();
                return;
        }
        #endregion

        //가상 위치 갱신
        map_manager.SetSPBossPoint(transform);

        #region HP바 처리
        hp_bar.fillAmount = sp_boss_data.hp / sp_boss_data.hp_max;
        #endregion

        #region 거리 계산 및 방향 변경

        //주인공과 몬스터 사이 거리 계산
        between_distance = transform.localPosition - player_transform.localPosition;

        //바라보는 방향 변경
        if (Vector3.Normalize(between_distance).x >= 0)
        {
            sp_boss_data.FacingLeft();
            transform.localScale = new Vector3(sp_boss_data.facing * scale.x, scale.y, scale.z);
        }
        else
        {
            sp_boss_data.FacingRight();
            transform.localScale = new Vector3(sp_boss_data.facing * scale.x, scale.y, scale.z);
        }

        #endregion

        #region 행동/패턴
        switch (sp_boss_data.movement)
        {
            //대기
            case GameDAO.SPBossData.Movement.isReady:

                Ready();

                break;

            //움직일때
            case GameDAO.SPBossData.Movement.isMove:

                //살아있는 상태면 진행X
                if (sp_boss_data.state == GameDAO.SPBossData.State.isDead)
                    break;

                //움직임
                Move();

                break;

            //근접공격
            case GameDAO.SPBossData.Movement.isMeleeAttack:
                
                //플레이어가 생존 상태 아니면 진행X
                if (player_data.state != GameDAO.PlayerData.State.isAlive)
                    break;

                MeleeAttack();

                break;

            //원거리 공격
            case GameDAO.SPBossData.Movement.isRangeAttack:

                //플레이어가 생존 상태 아니면 진행X
                if (player_data.state != GameDAO.PlayerData.State.isAlive)
                    break;

                RangeAttack();

                break;
        }
        #endregion
    }

    #region State 관련
    //피격
    private void Damaged()
    {
        //밀리기(한번만 실행)
        if (sp_boss_data.damage_timer.timer == 0)
        {
            sprite_renderer.color = new Color(1.0f, 1.0f, 1.0f, 0.6f);
            PushBoss();
        }

        if (sp_boss_data.damage_timer.timer < sp_boss_data.damage_timer.term)
            sp_boss_data.damage_timer.timer += Time.deltaTime;
        else
        {
            //원래 색으로
            sprite_renderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            //상태 되돌림
            sp_boss_data.state = GameDAO.SPBossData.State.isAlive;
            //멈춤
            rb2d.velocity = Vector2.zero;
            //시간 초기화
            sp_boss_data.damage_timer.timer = 0f;
        }
    }

    //죽음
    private void Die()
    {
        //hp바 0으로
        hp_bar.fillAmount = 0;

        //죽는 이펙트 실행
        if (!die_effect.gameObject.activeSelf)
        {
            done_die_effect = true;
            die_effect.gameObject.SetActive(true);

            if (die_sound != null)
                audio_source.PlayOneShot(die_sound);
        }

        //죽는 이펙트 켜져있는동안은 실행X
        if (die_effect.isPlaying)
            return;

        //플레이어에게 자원++
        player_data.my_subdata.AddAll(sp_boss_data.give_subdata);

        //가상 위치 제거
        map_manager.DestroySPBossPoint();

        //게임메니져에서 파괴 표시
        game_manager.DestroySPBoss();

        #region 자원 이펙트 생성
        //플레이어 크리스탈 획득 이펙트
        if (sp_boss_data.give_subdata.crystal > 0 && !get_crystal_ui.activeSelf)
            get_crystal_ui.SetActive(true);
        
        if (sp_boss_data.give_subdata.bio > 0)
        {
            GameObject sub_effect = Instantiate(bio_effect);
            sub_effect.transform.localPosition = transform.localPosition;

            int substance = sp_boss_data.give_subdata.bio;
            if (substance > 0)
                sub_effect.GetComponent<ParticleSystem>().Emit(substance);
        }
        if (sp_boss_data.give_subdata.metal > 0)
        {
            GameObject sub_effect = Instantiate(metal_effect);
            sub_effect.transform.localPosition = transform.localPosition;

            int substance = sp_boss_data.give_subdata.metal;
            if (substance > 0)
                sub_effect.GetComponent<ParticleSystem>().Emit(substance);
        }
        if (sp_boss_data.give_subdata.crystal > 0)
        {
            GameObject sub_effect = Instantiate(crystal_effect);
            sub_effect.transform.localPosition = transform.localPosition;

            int substance = sp_boss_data.give_subdata.crystal;
            if (substance > 0)
                sub_effect.GetComponent<ParticleSystem>().Emit(substance);
        } 
        #endregion

        //위치 변경
        if (transform.localPosition.x >= 0)
            transform.localPosition = Define.init_pos_plus;
        else
            transform.localPosition = Define.init_pos_minus;

        //엑티브 false로
        gameObject.SetActive(false);
    }

    //리젠
    public void Regen()
    {
        //움직임 / 상태 초기화
        sp_boss_data.movement = GameDAO.SPBossData.Movement.isMove;
        sp_boss_data.state = GameDAO.SPBossData.State.isAlive;

        //초기화(상속)
        RegenInit();
        
        //리젠 이펙트
        spawn_effect.SetActive(true);

        //엑티브 true
        gameObject.SetActive(true);

        //사운드
        audio_source.PlayOneShot(spawn_sound);
    }

    //로드
    public void Load(GameDAO.SPBossData _sp_boss_data)
    {
        //데이터 초기화
        sp_boss_data = _sp_boss_data;
        //위치 초기화
        LoadChild();
        //엑티브 true
        gameObject.SetActive(true);
    }

    protected abstract void LoadChild();

    #endregion

    #region Movemonet 관련
    //대기
    protected virtual void Ready() { }

    //움직임
    protected abstract void Move();

    //공격
    protected abstract void MeleeAttack();

    protected virtual void RangeAttack()
    {
        //애니메이션
        animator.SetBool("Move", false);
        animator.SetBool("Attack", true);

        //타이머 체크
        if (sp_boss_data.range_atk_timer.timer < sp_boss_data.range_atk_timer.term)
            sp_boss_data.range_atk_timer.timer += Time.deltaTime;
        else
        {
            //총알 이펙트
            if (!range_attack_effect.activeSelf && range_attack_effect != null)
                range_attack_effect.SetActive(true);

            //공격 사운드
            if (range_attack_sound != null)
                audio_source.PlayOneShot(range_attack_sound);

            //총알 발사!
            ShootBullet();

            sp_boss_data.range_atk_timer.timer = 0;
        }
    }
    #endregion

    #region 공통
    //초기화
    protected abstract void Init();

    //리젠시 초기화
    protected abstract void RegenInit();

    //자원 랜덤 생성
    protected void CreateSubstance(Define.Range bio, Define.Range metal, int _crystal_drop_rate)
    {
        //0 ~ 9999 까지 랜덤 생성
        int chance = Random.Range(0, 10000);

        sp_boss_data.give_subdata.bio = Random.Range(bio.min, bio.max);

        sp_boss_data.give_subdata.metal = Random.Range(metal.min, metal.max);

        if (chance < _crystal_drop_rate)
            sp_boss_data.give_subdata.crystal = 1;
        else
            sp_boss_data.give_subdata.crystal = 0;
    }
    #endregion

    #region 총알 관련
    //총알 생성
    protected void CreateBullets(int _num, float _speed)
    {
        SPBossBullet bullet;
        for (int i = 0; i < _num; i++)
        {
            bullet = Instantiate(bullet_prefab).GetComponent<SPBossBullet>();
            bullet.Init(GetComponent<SPBossCommon>(), sp_boss_data.atk, _speed);
            bullet_stack.Push(bullet);
        }
    }

    //총알 발사
    protected void ShootBullet()
    {
        //총알
        SPBossBullet bullet;

        //총알 남았는지 확인
        if (bullet_stack.ToArray().Length != 0)
        {
            //총알 가져옴
            bullet = bullet_stack.Pop();
            //재장전 위치 설정
            bullet.SetReloadPos(fire_pos.position);
            //발사!
            Vector2 direction = Vector3.Normalize(fire_pos.position - player_transform.localPosition);
            bullet.Shoot(-direction);
        }
    }

    //총알 회수
    public void ReloadBullet(SPBossBullet _bullet)
    {
        bullet_stack.Push(_bullet);
    }
    #endregion

    #region 피격/공격
    //플레이어 밀기
    public void PushPlayer()
    {
        //공중에 뜨니까 grounded -> false로
        player_data.grounded = false;
        //밀려나기
        player_transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        player_transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(50f * sp_boss_data.facing, 50f));
    }

    //보스(스스로) 밀기
    protected virtual void PushBoss()
    {
        //밀려나기(자기가 가는 반대방향으로)
        rb2d.velocity = Vector2.zero;
        rb2d.AddForce(new Vector2(60f * -sp_boss_data.facing, 0f));
    }

    void OnTriggerStay2D(Collider2D _col)
    {
        TriggerChild(_col);
    }

    void OnTriggerExit2D(Collider2D _col)
    {
        //플레이어에게서 벗어나면 공격 해제
        if (_col.CompareTag("Player"))
        {
            sp_boss_data.movement = GameDAO.SPBossData.Movement.isMove;
        }

        //건물에게서 벗어나면 공격 해제
        if (_col.CompareTag("Build"))
        {
            //타겟 지정
            attakc_target = null;
            sp_boss_data.movement = GameDAO.SPBossData.Movement.isMove;
        }
    }

    protected abstract void TriggerChild(Collider2D _col);
    #endregion
}
