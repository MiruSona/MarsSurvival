using UnityEngine;
using System.Collections.Generic;

public class Player : SingleTon<Player>
{
    
    #region 참조
    private Rigidbody2D rb2d;
    private Animator anim;
    private SpriteRenderer sprite_renderer;
    private AudioSource audio_source;
    private GameDAO.PlayerData player_data;
    private GameDAO.SpaceShipData spaceship_data;
    private SystemDAO.MapManager map_manager;
    private SystemDAO.GameManagerData gm_data;
    private SystemDAO.FileManager file_manager;
    private BoundaryWait boundary_wait;
    #endregion
    
    #region 도구&무기
    [System.NonSerialized]
    public GameObject tool;
    [System.NonSerialized]
    public GameObject weapon; 
    #endregion
    
    #region 총알
    private Stack<PlayerBullet> bullet_stack = new Stack<PlayerBullet>();
    private Stack<PlayerBullet> bullet_used_stack = new Stack<PlayerBullet>();
    private GameObject bullet_prefab;
    private Transform fire_pos;
    #endregion
    
    #region 사운드
    private AudioClip get_substance_sound;  //자원 얻는 사운드
    private AudioClip repair_sound;         //수리 사운드
    private AudioClip get_laser_sound;      //레이저 발사 사운드
    private AudioClip damaged_sound;        //데미지 사운드
    private AudioClip warp_sound;           //워프 사운드
    private bool warp_sound_bool = false;   //워프 사운드 실행 여부 
    #endregion

    //애니메이션 관련
    private RuntimeAnimatorController player_anim;
    private RuntimeAnimatorController player_normal_anim;
    private RuntimeAnimatorController player_special_anim;

    //마찰력
    private float friction = 0.8f;

    //중독 캔버스
    private GameObject poison_ui;

    //초기화
    void Start()
    {
        //레퍼런스 초기화
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite_renderer = GetComponent<SpriteRenderer>();
        audio_source = GetComponent<AudioSource>();
        boundary_wait = transform.FindChild("BoundaryWait").GetComponent<BoundaryWait>();

        //도구 & 무기 초기화
        tool = transform.FindChild("Tool").gameObject;
        weapon = transform.FindChild("Weapon").gameObject;

        //사운드 초기화
        get_substance_sound = Resources.Load<AudioClip>("Sound/SoundEffect/GetSubstance");
        repair_sound = Resources.Load<AudioClip>("Sound/SoundEffect/Repair_Sound");
        get_laser_sound = Resources.Load<AudioClip>("Sound/SoundEffect/Laser_Fire");
        damaged_sound = Resources.Load<AudioClip>("Sound/SoundEffect/Player_Hit");
        warp_sound = Resources.Load<AudioClip>("Sound/SoundEffect/WarpSound");

        //DAO 초기화
        player_data = GameDAO.instance.player_data;
        spaceship_data = GameDAO.instance.spaceship_data;
        map_manager = SystemDAO.instance.map_manager;
        gm_data = SystemDAO.instance.gm_data;
        file_manager = SystemDAO.instance.file_manager;

        //총알 초기화
        bullet_prefab = Resources.Load("Prefab/Game/Player/PlayerBullet") as GameObject;
        fire_pos = weapon.transform.FindChild("FirePos");
        CreateBullets();

        //중독 캔버스 초기화
        poison_ui = transform.FindChild("PoisonUI").gameObject;

        //애니메이션 초기화
        player_anim = Resources.Load<RuntimeAnimatorController>("Anim/Game/Player/Player");
        player_normal_anim = Resources.Load<RuntimeAnimatorController>("Anim/Game/Player/Normal_Cloth/Player_Normal");
        player_special_anim = Resources.Load<RuntimeAnimatorController>("Anim/Game/Player/Special_Cloth/Player_Special");
    }

    #region 총알관련
    //총알 생성
    private void CreateBullets()
    {
        PlayerBullet bullet;
        for (int i = 0; i < player_data.weapon_data.bullet_max; i++)
        {
            //현제 총알 수 ++
            player_data.weapon_data.bullet_num++;
            //총알 생성
            bullet = Instantiate(bullet_prefab).GetComponent<PlayerBullet>();
            //스택에 쌓는다
            bullet_stack.Push(bullet);
        }
    }

    //총알 발사
    public void ShootBullet()
    {
        //총알
        PlayerBullet bullet;

        //총알 남았는지 확인
        if (bullet_stack.ToArray().Length != 0)
        {
            //공격 사운드
            audio_source.PlayOneShot(get_laser_sound);
            //총알 수--
            player_data.weapon_data.bullet_num--;
            //총알 가져옴
            bullet = bullet_stack.Pop();
            //재장전 위치 설정
            bullet.SetReloadPos(fire_pos.position);
            //발사!
            bullet.Shoot();
        }
    }

    //총알 회수
    public void ReloadBullet(PlayerBullet _bullet)
    {
        //총알을 사용한 총알 스택에 넣는다
        bullet_used_stack.Push(_bullet);
    }
    #endregion

    #region 컬리션 처리
    
    void OnCollisionEnter2D(Collision2D _col)
    {
        //땅에 닿을경우 처리
        if (_col.transform.CompareTag("Ground"))
            player_data.grounded = true;
        

        //경계 기다리는 스크립트 타겟 = null
        if (_col.transform.CompareTag("Boundary"))
            boundary_wait.target = _col.transform;
        
    }
    
    void OnCollisionExit2D(Collision2D _col)
    {
        //땅에서 벗어났을 경우
        if (_col.transform.CompareTag("Ground"))
            player_data.grounded = false;

        //경계 기다리는 스크립트 타겟 = null
        if (_col.transform.CompareTag("Boundary"))
            boundary_wait.target = null;
    }
    #endregion
    
    //애니메이션 처리
    void Update()
    {
        //어느 옷인지 확인
        switch (file_manager.cloth_buy)
        {
            case Define.NO_CLOTH:
                //player_anim이 아닐 경우 그걸로 초기화
                if (!anim.runtimeAnimatorController.Equals(player_anim))
                    anim.runtimeAnimatorController = player_anim;
                break;

            case Define.NORMAL_CLOTH:
                //player_normal_anim이 아닐 경우 그걸로 초기화
                if (!anim.runtimeAnimatorController.Equals(player_normal_anim))
                    anim.runtimeAnimatorController = player_normal_anim;
                break;

            case Define.SPECIAL_CLOTH:
                //player_special_anim이 아닐 경우 그걸로 초기화
                if (!anim.runtimeAnimatorController.Equals(player_special_anim))
                    anim.runtimeAnimatorController = player_special_anim;
                break;
        }
    }

    //총괄
    void FixedUpdate()
    {
        #region GM상태 처리
        switch (gm_data.state)
        {
            //엔딩
            case SystemDAO.GameManagerData.State.isEnd:
                Ending();
                break;

            //일시정지
            case SystemDAO.GameManagerData.State.isPause:
                Pause();
                Warp();
                if (player_data.state == GameDAO.PlayerData.State.isPoisoned)
                    Poisoned();
                return;
        }
        #endregion

        //플레이 상태 아니면 실행X
        if (gm_data.state != SystemDAO.GameManagerData.State.isPlay)
            return;
        
        #region 주인공 상태 처리
        switch (player_data.state)
        {
            //살아 있을 경우
            case GameDAO.PlayerData.State.isAlive:
                if (sprite_renderer.color.a < 1.0f)
                    sprite_renderer.color = Color.white;
                break;

            //죽을 경우
            case GameDAO.PlayerData.State.isDead:
                Die();
                return;

            //피격 시
            case GameDAO.PlayerData.State.isDamaged:
                Damaged();
                break;
        }

        //공중에 뜰 경우(피격 시)
        if (!player_data.grounded)
        {
            OnAir();
            return;
        }
        #endregion

        //현재 속도
        Vector2 velocity = new Vector2(rb2d.velocity.x, 0);

        //플레이어 가상 위치 갱신
        map_manager.SetPlayerPoint(transform);

        //총알 장전
        Reload();

        #region 주인공 패턴
        switch (player_data.movement)
        {
            //대기
            case GameDAO.PlayerData.Movement.isReady:

                #region 대기
                //애니메이션 처리
                anim.SetBool("Move", false);
                anim.SetBool("Action", false);
                anim.SetBool("Fire", false);

                //땅에 닿았을 때 속도 처리
                if (player_data.grounded)
                {
                    velocity.x *= 0.1f;
                    rb2d.velocity = velocity;
                }

                //도구 & 무기 비활성화
                tool.SetActive(false);
                weapon.SetActive(false);
                #endregion

                break;

            //움직임 초기화
            case GameDAO.PlayerData.Movement.isMove:

                #region 움직임 초기화
                //땅에 닿았을 때 속도 처리
                if (player_data.grounded)
                {
                    velocity.x *= 0.1f;
                    rb2d.velocity = velocity;
                }

                //애니메이션 처리
                anim.SetBool("Move", true);
                anim.SetBool("Action", false);
                anim.SetBool("Fire", false);

                //이미지 방향 전환
                transform.localScale = new Vector3(player_data.facing, 1, 1);

                //도구 & 무기 비활성화
                tool.SetActive(false);
                weapon.SetActive(false);

                //다음단계로
                player_data.movement = GameDAO.PlayerData.Movement.isMoveSchedule;
                #endregion

                break;

            //움직임 업데이트
            case GameDAO.PlayerData.Movement.isMoveSchedule:

                #region 움직임 업데이트
                velocity.x *= friction;             //마찰력을 곱해준다
                rb2d.velocity = velocity;           //마찰력 곱해준 값을 다시 넣어준다.
                
                //현재속도 구하기(옷 구매여부 확인)
                float speed = player_data.speed;
                float max_speed = player_data.max_speed;
                if (file_manager.cloth_buy == Define.SPECIAL_CLOTH)
                {
                    speed *= Define.special_cloth_speed;
                    max_speed *= Define.special_cloth_speed;
                }

                //이동 -> 오른쪽 방향 * 속도 * 방향
                //땅에 있을 때만 실행
                if (player_data.grounded)
                    rb2d.AddForce((Vector2.right * speed) * player_data.facing);

                //최대 속도 제한
                if (rb2d.velocity.x > max_speed)
                    rb2d.velocity = new Vector2(max_speed, rb2d.velocity.y);

                if (rb2d.velocity.x < -max_speed)
                    rb2d.velocity = new Vector2(-max_speed, rb2d.velocity.y);
                #endregion

                break;

            //채취 초기화
            case GameDAO.PlayerData.Movement.isGather:

                #region 채취 초기화
                //땅에 닿았을 때 속도 처리
                if (player_data.grounded)
                {
                    velocity.x *= 0.1f;
                    rb2d.velocity = velocity;
                }

                //도구 활성화
                tool.SetActive(true);
                weapon.SetActive(false);
                //애니메이션 처리
                anim.SetBool("Move", false);
                anim.SetBool("Action", true);
                anim.SetBool("Fire", false);

                //이미지 방향 전환
                transform.localScale = new Vector3(player_data.facing, 1, 1);

                //다음단계로
                player_data.movement = GameDAO.PlayerData.Movement.isGatherSchedule;
                #endregion

                break;

            //채취 업데이트
            case GameDAO.PlayerData.Movement.isGatherSchedule:

                #region 채취 업데이트
                //타겟 자원이 없으면 대기상태로
                if (player_data.target_substance == null && player_data.target_bigsub == null)
                {
                    player_data.movement = GameDAO.PlayerData.Movement.isReady;
                    break;
                }

                //타이머 체크
                if (player_data.tool_timer.timer < player_data.tool_timer.term / player_data.tool_data.speed[player_data.tool_data.step])
                    player_data.tool_timer.timer += Time.deltaTime;
                else
                {
                    //자원 획득 사운드
                    audio_source.PlayOneShot(get_substance_sound);

                    //타겟 자원에서 얻은만큼 자기 자원에 저장
                    GameDAO.SubstanceData temp = new GameDAO.SubstanceData(0,0,0);
                    if(player_data.target_substance != null)
                        temp = player_data.target_substance.SubSubstanceData(player_data.tool_data.amount);
                    else
                        temp = player_data.target_bigsub.GetSubstance(player_data.tool_data.amount);
                    
                    player_data.my_subdata.AddAll(temp);

                    //타이머 초기화
                    player_data.tool_timer.timer = 0;
                }
                #endregion

                break;

            //수리 초기화
            case GameDAO.PlayerData.Movement.isRepair:

                #region 수리 초기화
                //땅에 닿았을 때 속도 처리
                if (player_data.grounded)
                {
                    velocity.x *= 0.1f;
                    rb2d.velocity = velocity;
                }

                //남은 자원 부족하면 실행X
                if (!player_data.my_subdata.CheckMore(spaceship_data.repair_need[spaceship_data.step]))
                {
                    //isReady로
                    player_data.movement = GameDAO.PlayerData.Movement.isReady;
                    break;
                }
                //도구 활성화
                tool.SetActive(true);
                weapon.SetActive(false);

                //애니메이션 처리
                anim.SetBool("Move", false);
                anim.SetBool("Action", true);
                anim.SetBool("Fire", false);

                //이미지 방향 전환
                transform.localScale = new Vector3(player_data.facing, 1, 1);

                //다음단계로
                player_data.movement = GameDAO.PlayerData.Movement.isRepairSchedule;
                #endregion

                break;

            //수리 업데이트
            case GameDAO.PlayerData.Movement.isRepairSchedule:

                #region 수리 업데이트
                //남은 자원 부족하면 실행X
                if (!player_data.my_subdata.CheckMore(spaceship_data.repair_need[spaceship_data.step]))
                {
                    //isReady로
                    player_data.movement = GameDAO.PlayerData.Movement.isReady;
                    break;
                }

                //타이머 체크
                if (player_data.tool_timer.timer < player_data.tool_timer.term / player_data.tool_data.speed[player_data.tool_data.step])
                    player_data.tool_timer.timer += Time.deltaTime;
                else
                {
                    //우주선hp++
                    spaceship_data.step_hp += Define.spaceship_recovery;
                    SpaceShip.instance.current_hp.fillAmount += 0.005f;

                    //사운드
                    audio_source.PlayOneShot(repair_sound);
                    //자원 감소
                    player_data.my_subdata.SubAll(spaceship_data.repair_need[spaceship_data.step]);
                    //타이머 초기화
                    player_data.tool_timer.timer = 0;
                }
                #endregion

                break;

            //공격 초기화
            case GameDAO.PlayerData.Movement.isAttack:

                #region 공격 초기화
                //땅에 닿았을 때 속도 처리
                if (player_data.grounded)
                {
                    velocity.x *= 0.1f;
                    rb2d.velocity = velocity;
                }

                //무기 활성화
                weapon.SetActive(true);
                tool.SetActive(false);

                //애니메이션 처리
                anim.SetBool("Move", false);
                anim.SetBool("Action", false);
                anim.SetBool("Fire", true);

                //이미지 방향 전환
                transform.localScale = new Vector3(player_data.facing, 1, 1);

                //처음에 한발 쏘도록
                player_data.attack_timer.timer = player_data.attack_timer.term;

                //다음단계로
                player_data.movement = GameDAO.PlayerData.Movement.isAttackSchedule;
                #endregion

                break;

            //공격 업데이트
            case GameDAO.PlayerData.Movement.isAttackSchedule:

                //UIController에서 공격 처리

                break;
        } 
        #endregion

    }

    #region GM상태 관련 함수

    //엔딩처리
    private void Ending()
    {
        //애니메이션 실행
        anim.Play("Player_Idle");
        anim.SetBool("Move", false);
        anim.SetBool("Action", false);
        anim.SetBool("Fire", false);
        //도구 & 무기 false
        tool.SetActive(false);
        weapon.SetActive(false);
    }

    //일시정지 처리
    private void Pause()
    {
        //애니메이션 false
        anim.SetBool("Move", false);
        anim.SetBool("Action", false);
        anim.SetBool("Fire", false);
        //도구 & 무기 false
        tool.SetActive(false);
        weapon.SetActive(false);

        //움직임 멈춤
        player_data.movement = GameDAO.PlayerData.Movement.isReady;
        rb2d.velocity = Vector2.zero;
    }

    #endregion

    #region 주인공 관련 함수

    #region 주인공 상태 처리

    //피격 처리
    private void Damaged()
    {
        //색변화
        sprite_renderer.color = new Color(1.0f, 1.0f, 1.0f, 0.6f);

        //한번 실행
        if (player_data.damaged_timer.timer == 0)
        {
            //카메라 흔들기
            iTween2.ShakePosition(gameObject, new Vector3(0.03f, 0.03f, 0), 0.2f);
            //사운드 플레이
            audio_source.PlayOneShot(damaged_sound);
        }

        //타이머
        if (player_data.damaged_timer.timer < player_data.damaged_timer.term)
            player_data.damaged_timer.timer += Time.deltaTime;
        else
        {
            //색 되돌림
            sprite_renderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            //상태 되돌림
            player_data.state = GameDAO.PlayerData.State.isAlive;
            //타이머 초기화
            player_data.damaged_timer.timer = 0f;
        }
    }

    //공중 처리
    private void OnAir()
    {
        //모든 애니메이션 끈다
        anim.SetBool("Move", false);
        anim.SetBool("Action", false);

        //도구 & 무기도 끈다
        tool.SetActive(false);

        //초기화 상태로 되돌린다
        switch (player_data.movement)
        {
            case GameDAO.PlayerData.Movement.isGatherSchedule:
                player_data.movement = GameDAO.PlayerData.Movement.isGather;
                break;

            case GameDAO.PlayerData.Movement.isMoveSchedule:
                player_data.movement = GameDAO.PlayerData.Movement.isMove;
                break;

            case GameDAO.PlayerData.Movement.isRepairSchedule:
                player_data.movement = GameDAO.PlayerData.Movement.isRepair;
                break;
        }
    }

    //죽을 경우
    private void Die()
    {
        //애니메이션 실행
        anim.Play("Player_Die");
        anim.SetBool("Move", false);
        anim.SetBool("Action", false);
        anim.SetBool("Fire", false);
        //도구 & 무기 false
        tool.SetActive(false);
        weapon.SetActive(false);
        //GM이 죽을때 경우 실행
        gm_data.state = SystemDAO.GameManagerData.State.isDead;
    }

    //중독 될 경우
    public void Poisoned()
    {
        //UI 꺼져있을 경우에만 실행
        if (poison_ui.activeSelf)
            return;

        //UI 켜기
        poison_ui.SetActive(true);
        
        //좌우 반전 방지
        Vector3 scale = poison_ui.transform.localScale;
        scale.x = player_data.facing;
        poison_ui.transform.localScale = scale;

        //중독 상태 애니메이션 실행
        anim.Play("Player_Poisoned");
    } 

    #endregion

    //워프 처리
    private void Warp()
    {
        //워프중이면
        if (!anim.GetBool("Warp"))
            return;

        //경계값 구하기
        float boundary = 0;

        //워프 사운드 실행
        if (!warp_sound_bool)
        {
            audio_source.PlayOneShot(warp_sound);
            warp_sound_bool = true;
        }

        //경계 넘어갈때 까지 이동
        if (player_data.warp_up)
        {
            //위로 이동
            boundary = Define.camera_height_half + 10f;
            if (transform.localPosition.y < Define.camera_height_half + 10f)
            {
                rb2d.gravityScale = 0;
                transform.Translate(0, 0.4f, 0);

                //경계에 닿았으면 우주선으로 이동
                if (transform.localPosition.y >= boundary)
                {
                    //플레이어 이동
                    map_manager.player_point = 0;
                    transform.localPosition = map_manager.GetPlayerRealPoint(boundary);

                    //카메라 이동
                    Vector3 camera_position = Camera.main.transform.localPosition;
                    camera_position.x = transform.localPosition.x;
                    Camera.main.transform.localPosition = camera_position;

                    player_data.warp_up = false;
                }
            }
        }
        else
        {
            boundary = Define.ground_position + ((sprite_renderer.sprite.bounds.size.y / 2) * transform.localScale.y);
            //바닥까지 이동
            if (transform.localPosition.y > boundary)
            {
                rb2d.gravityScale = 0;
                transform.Translate(0, -0.4f, 0);

                //바닥에 닿았으면 애니메이션 해제 및 게임 재게
                if (transform.localPosition.y <= boundary)
                {
                    anim.SetBool("Warp", false);
                    rb2d.gravityScale = 1;
                    gm_data.state = SystemDAO.GameManagerData.State.isPlay;

                    audio_source.PlayOneShot(warp_sound);
                    warp_sound_bool = false;
                }
            }
        }
    }

    //총알 장전
    private void Reload()
    {
        //공격 중이면 실행X
        if (player_data.movement == GameDAO.PlayerData.Movement.isAttack)
            return;

        //사용된 총알이 있다면
        if (bullet_used_stack.ToArray().Length != 0)
        {
            float reload_term = player_data.reload_timer.term / player_data.weapon_data.reload_speed[player_data.weapon_data.step];
            if (player_data.reload_timer.timer < reload_term)
            {
                //총알 재장전 타이머++
                player_data.reload_timer.timer += Time.deltaTime;
            }
            else
            {
                //장전된 총알 수++
                player_data.weapon_data.bullet_num++;
                //사용된 총알에서 빼와서 총알 장전
                bullet_stack.Push(bullet_used_stack.Pop());
                //타이머 초기화
                player_data.reload_timer.timer = 0;
            }
        }
    }
    #endregion
}
