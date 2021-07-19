using UnityEngine;
using System.Collections.Generic;

public class Player : SingleTon<Player> {
    
    //참조
    private Rigidbody2D rb2d;
    private Animator anim;
    private SpriteRenderer sprite_renderer;
    private AudioSource audio_source;
    private GameDAO.PlayerData player_data;
    private GameDAO.SpaceShipData spaceship_data;
    private SystemDAO.MapManager map_manager;
    private SystemDAO.GameManagerData gm_data;

    //도구 & 무기
    [System.NonSerialized]
    public GameObject tool;
    [System.NonSerialized]
    public GameObject weapon;

    //총알
    private Stack<PlayerBullet> bullet_stack = new Stack<PlayerBullet>();
    private Stack<PlayerBullet> bullet_used_stack = new Stack<PlayerBullet>();
    private GameObject bullet_prefab;
    private Transform fire_pos;

    //사운드
    private AudioClip get_substance_sound;  //자원 얻는 사운드
    private AudioClip repair_sound;         //수리 사운드
    private AudioClip get_laser_sound;      //레이저 발사 사운드
    private AudioClip damaged_sound;        //데미지 사운드

    //마찰력
    private float friction = 0.8f;

    //초기화
    void Start() {
        //레퍼런스 초기화
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite_renderer = GetComponent<SpriteRenderer>();
        audio_source = GetComponent<AudioSource>();

        //도구 & 무기 초기화
        tool = transform.FindChild("Tool").gameObject;
        weapon = transform.FindChild("Weapon").gameObject;

        //사운드 초기화
        get_substance_sound = Resources.Load<AudioClip>("Sound/SoundEffect/GetSubstance");
        repair_sound = Resources.Load<AudioClip>("Sound/SoundEffect/Repair_Sound");
        get_laser_sound = Resources.Load<AudioClip>("Sound/SoundEffect/Laser_Fire");
        damaged_sound = Resources.Load<AudioClip>("Sound/SoundEffect/Player_Hit");

        //DAO 초기화
        player_data = GameDAO.instance.player_data;
        spaceship_data = GameDAO.instance.spaceship_data;
        map_manager = SystemDAO.instance.map_manager;
        gm_data = SystemDAO.instance.gm_data;

        //총알 초기화
        bullet_prefab = Resources.Load("Prefab/Game/Player/PlayerBullet") as GameObject;
        fire_pos = weapon.transform.FindChild("FirePos");
        CreateBullets();
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

    #region 땅 처리
    //땅에 닿을경우 처리
    void OnCollisionEnter2D(Collision2D _col)
    {
        if (_col.transform.CompareTag("Ground"))
        {
            player_data.grounded = true;
        }
    }

    //땅에서 벗어났을 경우
    void OnCollisionExit2D(Collision2D _col)
    {
        if (_col.transform.CompareTag("Ground"))
        {
            player_data.grounded = false;
        }
    } 
    #endregion

    //전체 처리
    void FixedUpdate() {

        //엔딩일 때
        if(gm_data.state == SystemDAO.GameManagerData.State.isEnd)
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

        //플레이 상태 아니면 실행X
        if (gm_data.state != SystemDAO.GameManagerData.State.isPlay)
            return;

        #region 죽을 경우
        //죽을 경우
        if (player_data.state == GameDAO.PlayerData.State.isDead)
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
            return;
        } 
        #endregion

        //현재 속도
        Vector2 velocity = new Vector2(rb2d.velocity.x, 0);

        //플레이어 가상 위치 갱신
        map_manager.SetPlayerPoint(transform);

        #region 피해 받을 경우
        //피해 받았을 시
        if (player_data.state == GameDAO.PlayerData.State.isDamaged)
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
        

        //공중에 뜰 경우(피격 시)
        if (!player_data.grounded)
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

            return;
        }
        #endregion

        #region 총알 장전
        //공격 중이 아닐 시
        if (player_data.movement != GameDAO.PlayerData.Movement.isAttack)
        {
            //사용된 총알이 있다면
            if (bullet_used_stack.ToArray().Length != 0)
            {
                if (player_data.reload_timer.timer < player_data.reload_timer.term)
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

                //이동 -> 오른쪽 방향 * 속도 * 방향
                //땅에 있을 때만 실행
                if (player_data.grounded)
                    rb2d.AddForce((Vector2.right * player_data.speed) * player_data.facing);

                //최대 속도 제한
                if (rb2d.velocity.x > player_data.max_speed)
                    rb2d.velocity = new Vector2(player_data.max_speed, rb2d.velocity.y);

                if (rb2d.velocity.x < -player_data.max_speed)
                    rb2d.velocity = new Vector2(-player_data.max_speed, rb2d.velocity.y); 
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
                if (player_data.target_substance == null)
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
                    GameDAO.SubstanceData temp = player_data.target_substance.SubSubstanceData(player_data.tool_data.amount);
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
                if (!player_data.my_subdata.CheckMore(spaceship_data.repair_need))
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
                if (!player_data.my_subdata.CheckMore(spaceship_data.repair_need))
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
                    //사운드
                    audio_source.PlayOneShot(repair_sound);
                    //자원 감소
                    player_data.my_subdata.SubAll(spaceship_data.repair_need);
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
        
    }
}
