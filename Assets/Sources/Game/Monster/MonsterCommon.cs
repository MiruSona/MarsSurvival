using UnityEngine;
using System.Collections.Generic;

public abstract class MonsterCommon : MonoBehaviour {

    //데이터
    public GameDAO.MonsterData monster_data = new GameDAO.MonsterData();
    protected bool attack_player = false;
    protected GameDAO.BuildData attakc_target = null;

    #region 참조
    protected GameDAO.PlayerData player_data;
    protected GameDAO.ArtifactData artifact_data;
    protected ArtifactImage artifact_image;
    protected SystemDAO.MapManager map_manager;
    protected SystemDAO.GlobalState global_state;
    protected GameManager game_manager;
    private SystemDAO.GameManagerData gm_data;

    protected Transform player_transform;
    protected Rigidbody2D rb2d;
    protected Animator animator;
    protected AudioSource audio_source;
    protected AudioClip bullet_sound;
    protected SpriteRenderer sprite_renderer;
    protected Vector3 scale;
    #endregion

    //이펙트
    protected Renderer monster_effect = null;
    protected GameObject die_effect;
    protected GameObject ground_effect;

    protected GameObject metal_effect;
    protected GameObject bio_effect;
    protected GameObject crystal_effect;
    
    #region 총알 관련
    public Stack<MonsterBullet> bullet_stack = new Stack<MonsterBullet>();
    protected GameObject bullet_prefab;
    protected ParticleSystem bullet_effect;
    protected Vector3 fire_pos; 
    #endregion

    //플레이어와 몬스터 사이 거리
    protected Vector3 between_distance;

    //인덱스
    public int index = 0;

    #region 자원
    //자원 양
    protected Define.Range bio_range = new Define.Range();
    protected Define.Range metal_range = new Define.Range();
    protected Define.Range crystal_range = new Define.Range();

    //자원 드랍 확률
    protected int bio_drop_rate = 0;
    protected int metal_drop_rate = 0;
    #endregion

    #region enum
    //강한 정도
    public enum Intensity
    {
        Weak,
        Normal,
        Strong
    }
    public Intensity intensity = Intensity.Weak;

    //원거리 / 근거리
    public enum AttackStyle
    {
        Melee,
        Range
    }
    public AttackStyle attack_style = AttackStyle.Melee; 
    #endregion

    //Enable 될때마다 초기화
    void OnEnable()
    {
        //초기화
        Init();
    }

    //초기화
    void Start () {
        //참조
        player_data = GameDAO.instance.player_data;
        artifact_data = GameDAO.instance.artifact_data;
        artifact_image = ArtifactImage.instance;
        map_manager = SystemDAO.instance.map_manager;
        global_state = SystemDAO.instance.global_state;
        game_manager = GameManager.instance;
        gm_data = SystemDAO.instance.gm_data;

        player_transform = GameObject.FindGameObjectWithTag("Player").transform;
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audio_source = GetComponent<AudioSource>();
        sprite_renderer = GetComponent<SpriteRenderer>();
        scale = transform.localScale;

        //이펙트
        bio_effect = Resources.Load("Prefab/Game/Effects/Bio_Effect") as GameObject;
        metal_effect = Resources.Load("Prefab/Game/Effects/Metal_Effect") as GameObject;
        crystal_effect = Resources.Load("Prefab/Game/Effects/Crystal_Effect") as GameObject;

        gameObject.SetActive(false);
    }
	
    //업데이트
	void FixedUpdate () {

        //플레이 상태 아니면 실행X
        if (gm_data.state != SystemDAO.GameManagerData.State.isPlay)
            return;

        #region 죽음/Disable/Damage 체크
        //죽으면 실행 X
        if (monster_data.state == GameDAO.MonsterData.State.isDead)
        {
            Die();
            return;
        }

        //몬스터 가상 위치 갱신
        map_manager.SetMonsterPoint(transform, index);
        //몬스터가 diable point 넘어가면 쥬금!
        if (map_manager.CheckDisableMonster(index))
        {
            DieWithDisable();
            return;
        }

        //데미지 받으면
        if (monster_data.state == GameDAO.MonsterData.State.isDamaged)
        {
            Damaged();
        }
        #endregion
        
        #region 거리 계산 및 방향 변경

        //주인공과 몬스터 사이 거리 계산
        between_distance = transform.localPosition - player_transform.localPosition;

        //바라보는 방향 변경
        if (Vector3.Normalize(between_distance).x >= 0)
        {
            monster_data.FacingLeft();
            transform.localScale = new Vector3(monster_data.facing * scale.x, scale.y, scale.z);
        }
        else
        {
            monster_data.FacingRight();
            transform.localScale = new Vector3(monster_data.facing * scale.x, scale.y, scale.z);
        } 

        #endregion

        #region 몬스터 원거리 타입일 경우

        if (attack_style == AttackStyle.Range)
        {
            //대기상태가 아니라면
            if (monster_data.movement != GameDAO.MonsterData.Movement.isAttackMelee)
            {
                //사거리 안에 들어오면
                if (1 <= Mathf.Abs(between_distance.x) && Mathf.Abs(between_distance.x) <= 7.5)
                {
                    //사거리 보다 조금 더 들어와서야 공격!
                    if (Mathf.Abs(between_distance.x) <= 6)
                    {
                        monster_data.movement = GameDAO.MonsterData.Movement.isAttackRange;
                    }
                }
                else
                    monster_data.movement = GameDAO.MonsterData.Movement.isMove;
            }
        } 

        #endregion

        //낮에 죽는 몬스터 상속
        NightDie();

        //모래폭풍 없을때 죽는 몬스터 상속
        StormDie();

        #region 행동/패턴
        switch (monster_data.movement)
        {
            //대기
            case GameDAO.MonsterData.Movement.isReady:

                break;
            //움직일때
            case GameDAO.MonsterData.Movement.isMove:
                //애니메이션
                animator.SetBool("Move", true);
                animator.SetBool("Attack", false);
                //움직임
                if (monster_data.state == GameDAO.MonsterData.State.isAlive)
                    Move();
                break;
            //근접공격
            case GameDAO.MonsterData.Movement.isAttackMelee:
                //플레이어가 생존 상태 아니면 진행X
                if (player_data.state != GameDAO.PlayerData.State.isAlive)
                    break;

                //애니메이션
                animator.SetBool("Move", false);
                animator.SetBool("Attack", true);

                //플레이어 공격중이면
                if(attack_player)
                    PushPlayer();
                else
                {
                    //타겟이 죽으면 null로 초기화 및 움직임으로 변경
                    if (attakc_target.state == GameDAO.BuildData.State.isDead)
                    {
                        attakc_target = null;
                        monster_data.movement = GameDAO.MonsterData.Movement.isMove;
                    }
                }

                AttackMelee();

                break;
            //원거리 공격
            case GameDAO.MonsterData.Movement.isAttackRange:
                //애니메이션
                animator.SetBool("Move", false);
                animator.SetBool("Attack", true);

                //타이머 체크
                if (monster_data.atk_range_timer.timer < monster_data.atk_range_timer.term)
                    monster_data.atk_range_timer.timer += Time.deltaTime;
                else
                {
                    audio_source.PlayOneShot(bullet_sound);
                    AttackRange();
                    monster_data.atk_range_timer.timer = 0;
                }
                break;
        } 
        #endregion
    }

    #region 공통
    //자원 랜덤 생성
    protected void CreateSubstance(Define.Range bio, Define.Range metal, Define.Range crystal, int _bio_drop_rate, int _metal_drop_rate)
    {
        //0 ~ 9999 까지 랜덤 생성
        int chance = Random.Range(0, 10000);

        //나올 확률 - 테스트
        int crystal_probablity = 1000;

        //확률에 해당되면 생성
        if (chance < _bio_drop_rate)
            monster_data.give_subdata.bio = Random.Range(bio.min, bio.max);
        else
            monster_data.give_subdata.bio = 0;

        if (chance < _metal_drop_rate)
            monster_data.give_subdata.metal = Random.Range(metal.min, metal.max);
        else
            monster_data.give_subdata.metal = 0;

        if (chance < crystal_probablity)
            monster_data.give_subdata.crystal = Random.Range(crystal.min, crystal.max);
        else
            monster_data.give_subdata.crystal = 0;
    }

    //피격
    protected void Damaged()
    {
        //밀리기(한번만 실행)
        if (monster_data.damage_timer.timer == 0)
        {
            sprite_renderer.color = new Color(1.0f, 1.0f, 1.0f, 0.6f);
            PushMonster();
        }

        if (monster_data.damage_timer.timer < monster_data.damage_timer.term)
            monster_data.damage_timer.timer += Time.deltaTime;
        else
        {
            //원래 색으로
            sprite_renderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            //상태 되돌림
            monster_data.state = GameDAO.MonsterData.State.isAlive;
            //시간 초기화
            monster_data.damage_timer.timer = 0f;
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
        player_data.my_subdata.AddAll(monster_data.give_subdata);
        //자원 이펙트 생성
        if(monster_data.give_subdata.bio > 0)
        {
            GameObject sub_effect = Instantiate(bio_effect);
            sub_effect.transform.localPosition = transform.localPosition;

            int substance = monster_data.give_subdata.bio;
            if (substance > 0)
                sub_effect.GetComponent<ParticleSystem>().Emit(substance);
        }
        if (monster_data.give_subdata.metal > 0)
        {
            GameObject sub_effect = Instantiate(metal_effect);
            sub_effect.transform.localPosition = transform.localPosition;

            int substance = monster_data.give_subdata.metal;
            if (substance > 0)
                sub_effect.GetComponent<ParticleSystem>().Emit(substance);
        }
        if (monster_data.give_subdata.crystal > 0)
        {
            GameObject sub_effect = Instantiate(crystal_effect);
            sub_effect.transform.localPosition = transform.localPosition;

            int substance = monster_data.give_subdata.crystal;
            if (substance > 0)
                sub_effect.GetComponent<ParticleSystem>().Emit(substance);
        }

        //위치 변경
        if (transform.localPosition.x >= 0)
            transform.localPosition = Define.init_pos_plus;
        else
            transform.localPosition = Define.init_pos_minus;

        //가상 위치 제거
        map_manager.DestroyMonsterPoint(index);
        //게임메니저에서 몬스터 제거
        game_manager.DestroyMonster(index, monster_data.pool);

        //엑티브 false로
        gameObject.SetActive(false);
    }

    //disable point에 닿았을 시 실행
    public void DieWithDisable()
    {
        //엑티브 false로
        gameObject.SetActive(false);
        //죽는 이펙트 생성
        //GameObject effect = Instantiate(die_effect);
        //effect.transform.localPosition = transform.localPosition;
        //위치 변경
        if (transform.localPosition.x >= 0)
            transform.localPosition = Define.init_pos_plus;
        else
            transform.localPosition = Define.init_pos_minus;
        //가상 위치 제거
        map_manager.DestroyMonsterPoint(index);
        //게임메니저에서 몬스터 제거
        game_manager.DestroyMonster(index, monster_data.pool);
    }

    //리젠
    public void Regen()
    {
        //움직임 / 상태 초기화
        monster_data.movement = GameDAO.MonsterData.Movement.isMove;
        monster_data.state = GameDAO.MonsterData.State.isAlive;
        //랜덤 위치 추가
        map_manager.AddRandomMonsterPoint(index, global_state.level);
        //위치 초기화
        transform.localPosition = map_manager.GetMonsterRealPoint(0, index);
        //중력 초기화
        rb2d.gravityScale = 1f;
        //레이어 초기화
        sprite_renderer.sortingOrder = index;
        if (monster_effect != null)
            monster_effect.sortingOrder = index - 1;
        //엑티브 true
        gameObject.SetActive(true);
    }

    //근접 공격
    protected void AttackMelee()
    {
        //공격!
        if (attack_player)
            player_data.SubHP(monster_data.atk_melee);
        else
        {
            if (attakc_target != null)
                attakc_target.SubHP(monster_data.atk_melee);
            else
                monster_data.movement = GameDAO.MonsterData.Movement.isMove;
        }
    }

    //죽는 이펙트 초기화
    protected void DieEffectInit(string _effect)
    {
        die_effect = Resources.Load<GameObject>("Prefab/Game/Effects/" + _effect);
    }

    //확률 체크
    protected bool CheckRandomRange(int rate)
    {
        bool send_bool = false;

        int random = Random.Range(0, 10000);

        if (0 <= random && random <= rate)
        {
            send_bool = true;
        }

        return send_bool;
    }
    #endregion

    #region 상속

    protected abstract void Init();

    protected abstract void Move();
    
    protected abstract void AttackRange();

    protected virtual void NightDie() { }

    protected virtual void StormDie() { }

    protected virtual void DieChild() { }
    #endregion

    #region 총알 관련
    //총알 생성
    protected void CreateBullets(int _num, float _speed)
    {
        MonsterBullet bullet;
        for (int i = 0; i < _num; i++)
        {
            bullet = Instantiate(bullet_prefab).GetComponent<MonsterBullet>();
            bullet.Init(GetComponent<MonsterCommon>(), monster_data.atk_range, _speed);
            bullet_stack.Push(bullet);
        }
    }

    //총알 발사
    protected void ShootBullet(float _offset_x, float _offset_y)
    {
        //총알
        MonsterBullet bullet;
        //위치 초기화
        fire_pos = transform.localPosition;
        fire_pos.x += _offset_x;
        fire_pos.y += _offset_y;

        //총알 남았는지 확인
        if (bullet_stack.ToArray().Length != 0)
        {
            //총알 가져옴
            bullet = bullet_stack.Pop();
            //재장전 위치 설정
            bullet.SetReloadPos(fire_pos);
            //발사!
            bullet.Shoot(monster_data.facing);
        }
    }

    //총알 회수
    public void ReloadBullet(MonsterBullet _bullet)
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
        player_transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(50f * monster_data.facing, 50f));
    }

    //몬스터(스스로) 밀기
    protected void PushMonster()
    {
        //밀려나기(자기가 가는 반대방향으로)
        rb2d.velocity = Vector2.zero;
        rb2d.AddForce(new Vector2(40f * -monster_data.facing, 40f));
    }

    void OnTriggerEnter2D(Collider2D _col)
    {
        //땅에 닿으면
        if (_col.CompareTag("Ground"))
        {
            rb2d.gravityScale = 0;
            rb2d.velocity = Vector2.zero;
        }
    }

    void OnTriggerStay2D(Collider2D _col)
    {
        //플레이어 에게 닿으면
        if (_col.CompareTag("Player"))
        {
            attack_player = true;
            //근거리 공격 상태로 변경
            monster_data.movement = GameDAO.MonsterData.Movement.isAttackMelee;
        }

        //건물 에게 닿으면
        if (_col.CompareTag("Build"))
        {
            //회복기면 공격X
            if (_col.GetComponent<BuildCommon>().my_data.pool == GameDAO.BuildData.Pool.Regenerator)
                return;

            attack_player = false;
            //타겟 지정
            attakc_target = _col.GetComponent<BuildCommon>().my_data;
            //근거리 공격 상태로 변경
            monster_data.movement = GameDAO.MonsterData.Movement.isAttackMelee;
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
            monster_data.movement = GameDAO.MonsterData.Movement.isMove;
        }

        //건물에게서 벗어나면 공격 해제
        if (_col.CompareTag("Build"))
        {
            //타겟 지정
            attakc_target = null;
            monster_data.movement = GameDAO.MonsterData.Movement.isMove;
        }
    } 
    #endregion
}
