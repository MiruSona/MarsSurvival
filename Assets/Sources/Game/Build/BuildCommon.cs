using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class BuildCommon : MonoBehaviour {

    //참조
    protected SystemDAO.MapManager map_manager;
    protected BuildSlot build_slot;
    protected GameManager game_manager;
    private SystemDAO.GameManagerData gm_data;
    protected SpriteRenderer sprite_renderer;
    private Rigidbody2D rb2d;
    protected Vector3 local_scale;

    //죽을때 관련
    private GameObject die_effect;

    //체력바
    private Image hp_bar = null;

    //데이터
    public GameDAO.BuildData my_data;
    
    //처음 초기화
    void Awake()
    {
        map_manager = SystemDAO.instance.map_manager;
        build_slot = Player.instance.transform.FindChild("BuildSlot").GetComponent<BuildSlot>();
        game_manager = GameManager.instance;
        gm_data = SystemDAO.instance.gm_data;
        sprite_renderer = GetComponent<SpriteRenderer>();
        rb2d = GetComponent<Rigidbody2D>();
        local_scale = transform.localScale;

        //죽는 이펙트
        die_effect = Resources.Load<GameObject>("Prefab/Game/Effects/Build_Destroy_Effect");

        //체력바 초기화
        if (transform.FindChild("HPCanvas") != null)
            hp_bar = transform.FindChild("HPCanvas").FindChild("HPBar").GetComponent<Image>();

        AwakeInit();

        gameObject.SetActive(false);
    }

    //초기화
    public void Init(Vector3 _position)
    {
        //방향 바꿈
        my_data.facing = GameDAO.instance.player_data.facing;
        Vector3 scale = local_scale;
        scale.x *= my_data.facing;
        transform.localScale = scale;

        //HP 감소 방향 바꿈
        if(hp_bar != null)
        {
            if (my_data.facing == 1)
                hp_bar.fillOrigin = 0;
            else
                hp_bar.fillOrigin = 1;
        }

        //자신 위치 갱신
        transform.localPosition = _position;

        //가상 위치 갱신
        map_manager.SetBuildPoint(transform, my_data.index);

        //중력 초기화
        rb2d.gravityScale = 1f;

        //자식 초기화
        ChildInit();

        //활성화
        gameObject.SetActive(true);
    }

    public void Init(Vector3 _position, GameDAO.BuildData _build_data)
    {
        //내 데이터 초기화
        my_data = _build_data;
        
        //방향 바꿈
        Vector3 scale = local_scale;
        scale.x *= my_data.facing;
        transform.localScale = scale;

        //HP 감소 방향 바꿈
        if (hp_bar != null)
        {
            if (my_data.facing == 1)
                hp_bar.fillOrigin = 0;
            else
                hp_bar.fillOrigin = 1;
        }

        //자신 위치 갱신
        transform.localPosition = _position;

        //가상 위치 갱신
        map_manager.SetBuildPoint(transform, my_data.index);

        //중력 초기화
        rb2d.gravityScale = 1f;

        //자식 초기화
        LoadChildInit();

        //활성화
        gameObject.SetActive(true);
    }

    //업데이트
    void FixedUpdate()
    {
        //플레이 상태 아니면 실행X
        if (gm_data.state != SystemDAO.GameManagerData.State.isPlay)
            return;

        //체력 처리
        if(hp_bar != null)
        {
            hp_bar.fillAmount = my_data.hp / my_data.hp_max[my_data.step];
        }

        //죽으면 실행 X
        if (my_data.state == GameDAO.BuildData.State.isDead)
        {
            Die();
            return;
        }

        //disable point에 닿으면 disable
        if (map_manager.CheckDisableBuild(my_data.index))
        {
            CheckDisable();
            return;
        }

        //데미지 받으면
        if (my_data.state == GameDAO.BuildData.State.isDamaged)
        {
            Damaged();
        }
        
        //각 행동에 따른 패턴
        switch (my_data.movement)
        {
            //대기
            case GameDAO.BuildData.Movement.isReady:

                break;

            //공격
            case GameDAO.BuildData.Movement.isAttack:
                Attack();
                break;
        }
    }
    
    #region 공통

    //피격
    private void Damaged()
    {
        //(한번만 실행)
        if (my_data.damage_timer.timer == 0)
        {
            sprite_renderer.color = new Color(1.0f, 1.0f, 1.0f, 0.6f);
        }

        if (my_data.damage_timer.timer < my_data.damage_timer.term)
            my_data.damage_timer.timer += Time.deltaTime;
        else
        {
            //원래 색으로
            sprite_renderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            //상태 되돌림
            my_data.state = GameDAO.BuildData.State.isAlive;
            //시간 초기화
            my_data.damage_timer.timer = 0f;
        }
    }

    //죽음
    private void Die()
    {
        //파괴 이펙트 생성
        GameObject effect = Instantiate(die_effect);
        effect.transform.localPosition = transform.localPosition;

        //엑티브 false로
        gameObject.SetActive(false);

        //위치 변경
        if (transform.localPosition.x >= 0)
            transform.localPosition = Define.init_pos_plus;
        else
            transform.localPosition = Define.init_pos_minus;

        //가상 위치 제거
        map_manager.DestroyBuildPoint(my_data.index);
        //게임메니저에서 제거
        game_manager.DestroyBuild(my_data.index, my_data.pool);
        //인덱스 초기화
        my_data.index = -1;

        //빌드 다시 가능하게
        build_slot.slot1_bool = true;
        build_slot.slot2_bool = true;
    }
    
    //Disable
    private void CheckDisable()
    {
        //disable 범위 들어왔나 확인
        if (map_manager.CheckDisableBuild(my_data.index))
        {
            my_data.movement = GameDAO.BuildData.Movement.isReady;
            gameObject.SetActive(false);
        }
    }

    #endregion

    //상속
    protected abstract void Attack();
    protected abstract void ChildInit();
    protected abstract void LoadChildInit();
    protected abstract void AwakeInit();

    //중력0으로
    void OnTriggerEnter2D(Collider2D _col)
    {
        if (_col.CompareTag("Ground"))
        {
            rb2d.gravityScale = 0;
            rb2d.velocity = Vector2.zero;
        }
    }
}
