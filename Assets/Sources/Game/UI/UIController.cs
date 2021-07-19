using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class UIController : MonoBehaviour {

    //참조
    private Player player;
    private GameDAO.PlayerData player_data;
    private GameDAO.BuffData buff_data;
    private SystemDAO.FileManager file_manager;
    private SystemDAO.GameManagerData gm_data;
    private SystemDAO.GlobalState global_state;
    public GameObject pause_menu;
    public GameObject upgrade_menu;
    public GameObject build_menu;

    //버프 관련
    public GameObject buff_menu;
    private Text buff_menu_text;
    private GameObject buff_btn;
    private GameObject buff_gauge;
    private Image buff_icon;
    private Image buff_bar;
    private bool check_init = false;

    //상태표시
    private Image hp_bar;
    private Image bullet_icon;
    private Image bullet_bar;

    //쿨타임 표시
    private GameObject cool_panel;
    private Image cool_panel_image;

    //초기화
    void Start () {
        player = Player.instance;
        player_data = GameDAO.instance.player_data;
        buff_data = GameDAO.instance.buff_data;
        file_manager = SystemDAO.instance.file_manager;
        gm_data = SystemDAO.instance.gm_data;
        global_state = SystemDAO.instance.global_state;

        cool_panel = transform.FindChild("CoolPanel").gameObject;
        cool_panel_image = cool_panel.GetComponent<Image>();

        hp_bar = transform.FindChild("HPBar").GetComponent<Image>();

        bullet_bar = transform.FindChild("BulletBar").GetComponent<Image>();
        bullet_icon = transform.FindChild("BulletIcon").GetComponent<Image>();

        buff_menu_text = buff_menu.transform.FindChild("Panel").FindChild("Text").GetComponent<Text>();
        buff_btn = transform.FindChild("BuffBtn").gameObject;
        buff_gauge = transform.FindChild("BuffGauge").gameObject;
        buff_bar = transform.FindChild("BuffGauge").FindChild("BuffBar").GetComponent<Image>();
        buff_icon = transform.FindChild("BuffGauge").FindChild("BuffIcon").GetComponent<Image>();
    }
	
	void FixedUpdate () {

        //버프 UI 초기화
        if (!check_init)
        {
            if (file_manager.buff_data.enable)
            {
                buff_btn.SetActive(false);
                buff_gauge.SetActive(true);
            }

            if(file_manager.buff_data.already_clicked)
                buff_btn.SetActive(false);

            check_init = true;
        }

        #region 장전바 변경
        //아이콘 변경
        bullet_icon.sprite = player_data.weapon_data.sprites[player_data.weapon_data.step];
        //색변경
        switch (player_data.weapon_data.step)
        {
            case 0:
                bullet_bar.color = new Color32(246, 255, 86, 255);
                break;
            case 1:
                bullet_bar.color = new Color32(255, 136, 66, 255);
                break;
            case 2:
                bullet_bar.color = new Color32(255, 36, 36, 255);
                break;
        }
        #endregion

        #region 버프UI 변경
        //버프 버튼 주기마다 버튼 껐다켰다 하기
        if (global_state.day_num % Define.Buff_Cycle == 0)
        {
            if(!buff_btn.activeSelf && !buff_gauge.activeSelf && !buff_data.already_clicked)
            {
                buff_btn.SetActive(true);
                buff_data.already_clicked = true;
            }                
        }
        else
        {
            if(buff_btn.activeSelf)
                buff_btn.SetActive(false);

            buff_data.already_clicked = false;
        }         
        

        if (buff_gauge.activeSelf)
        {
            //아이콘 변경
            buff_icon.sprite = buff_data.icons[buff_data.buff_kind];
            //게이지바 변경
            buff_bar.fillAmount = 1.0f - (buff_data.buff_timer.timer / buff_data.buff_timer.term);

            if (!buff_data.enable)
                buff_gauge.SetActive(false);
        }
        #endregion

        //HP 표시
        hp_bar.fillAmount = player_data.hp / player_data.max_hp;

        //총알 표시
        bullet_bar.fillAmount = 1.0f * (player_data.weapon_data.bullet_num / player_data.weapon_data.bullet_max);

        //플레이 아니면 실행X
        if (gm_data.state != SystemDAO.GameManagerData.State.isPlay)
            return;

        #region 공격 처리
        //공격 이외의 행동 시 attacking = false
        if (player_data.movement != GameDAO.PlayerData.Movement.isAttackSchedule)
            player_data.attacking = false;

        //공격 중일 시에 장전 못하도록
        if (player_data.attacking)
            player_data.reload_timer.timer = 0;

        //타이머 체크
        if (player_data.attack_timer.timer <= player_data.attack_timer.term)
            player_data.attack_timer.timer += Time.deltaTime;
        else
        {
            //만약 공격중(버튼 누르는 중)이면
            if (player_data.attacking)
            {
                //공격
                player.ShootBullet();
                //쿨패널 되돌리고
                cool_panel_image.fillAmount = 1.0f;
                //타이머 초기화
                player_data.attack_timer.timer = 0;
            }
            else  //공격 중이 아니면 쿨타임 해제
            {
                cool_panel.SetActive(false);
            }
        } 
        #endregion
    }
    
    //오른쪽으로 움직이기
    public void MoveRight()
    {
        //Play 아니면 실행X
        if (gm_data.state != SystemDAO.GameManagerData.State.isPlay)
            return;

        //공중에 있으면 실행 X
        if (!player_data.grounded)
            return;

        player_data.FacingRight();
        player_data.movement = GameDAO.PlayerData.Movement.isMove;
    }

    //왼쪽으로 움직이기
    public void MoveLeft()
    {
        //Play 아니면 실행X
        if (gm_data.state != SystemDAO.GameManagerData.State.isPlay)
            return;

        //공중에 있으면 실행 X
        if (!player_data.grounded)
            return;

        player_data.FacingLeft();
        player_data.movement = GameDAO.PlayerData.Movement.isMove;
    }

    //움직임 멈추기(대기)
    public void MoveStop()
    {
        //Play 아니면 실행X
        if (gm_data.state != SystemDAO.GameManagerData.State.isPlay)
            return;

        player_data.movement = GameDAO.PlayerData.Movement.isReady;
    }

    //공격
    public void Attack()
    {
        //Play 아니면 실행X
        if (gm_data.state != SystemDAO.GameManagerData.State.isPlay)
            return;

        //공격중으로 변경
        player_data.attacking = true;
        //쿨타임 패널 활성화(버튼 막기)
        cool_panel.SetActive(true);
        //공격!
        player_data.movement = GameDAO.PlayerData.Movement.isAttack;
    }

    //공격 중지
    public void AttackStop()
    {
        //Play 아니면 실행X
        if (gm_data.state != SystemDAO.GameManagerData.State.isPlay)
            return;

        //공격 중 해제
        player_data.attacking = false;
    }
    
    //일시정지 메뉴
    public void Pause()
    {
        //플레이 아니면 실행X
        if (gm_data.state != SystemDAO.GameManagerData.State.isPlay)
            return;

        pause_menu.SetActive(true);
        Time.timeScale = 0;
    }

    //업그레이드 메뉴
    public void Upgrade()
    {
        //플레이 아니면 실행X
        if (gm_data.state != SystemDAO.GameManagerData.State.isPlay)
            return;

        upgrade_menu.SetActive(true);
        Time.timeScale = 0;
    }

    //빌드 메뉴
    public void Build()
    {
        //플레이 아니면 실행X
        if (gm_data.state != SystemDAO.GameManagerData.State.isPlay)
            return;

        build_menu.SetActive(true);
        Time.timeScale = 0;
    }

    //버프 버튼
    public void BuffBtn()
    {
        //플레이 아니면 실행X
        if (gm_data.state != SystemDAO.GameManagerData.State.isPlay)
            return;

        buff_menu.SetActive(true);

        //버프 메뉴 텍스트 변경
        switch (buff_data.buff_kind)
        {
            case Define.BuffAttack:
                if (TextTrans.instance.languageCode == "KR") {
                    buff_menu_text.text = "광고를 보시면 공격력이 증가됩니다. 받으시습니까?\n(2분간 지속됩니다)";
                } else if (TextTrans.instance.languageCode == "JP") {
                    buff_menu_text.text = "広告を見ると攻撃力が上昇します.\n(2分間続きます)";
                } else {
                    buff_menu_text.text = "If you see AD, your attack will be stronger\n(for 2 minutes)";
                }
                break;

            case Define.BuffPlayerSpeed:
                if (TextTrans.instance.languageCode == "KR") {
                    buff_menu_text.text = "광고를 보시면 이동 속도가 증가됩니다. 받으시습니까?\n(2분간 지속됩니다)";
                } else if (TextTrans.instance.languageCode == "JP") {
                    buff_menu_text.text = "広告を見るとスピードが上昇します.\n(2分間続きます)";
                } else {
                    buff_menu_text.text = "If you see AD, your move speed will be faster\n(for 2 minutes)";
                }
                break;

            case Define.BuffToolSpeed:
                if (TextTrans.instance.languageCode == "KR") {
                    buff_menu_text.text = "광고를 보시면 도구 속도가 증가됩니다. 받으시습니까?\n(2분간 지속됩니다)";
                } else if (TextTrans.instance.languageCode == "JP") {
                    buff_menu_text.text = "広告を見ると道具のスピードが上昇します.\n(2分間続きます)";
                } else {
                    buff_menu_text.text = "If you see AD, your tool speed will be faster\n(for 2 minutes)";
                }
                break;
        }
        Time.timeScale = 0;
    }

    //버프 메뉴
    public void BuffMenuYes()
    {
        //플레이 아니면 실행X
        if (gm_data.state != SystemDAO.GameManagerData.State.isPlay)
            return;

        if(!file_manager.commercial_off)
            Advertisement.Show("rewardedVideo");
        buff_data.enable = true;
        buff_data.already_clicked = true;
        buff_gauge.SetActive(true);
        buff_btn.SetActive(false);
        buff_menu.SetActive(false);
        Time.timeScale = 1;
    }

    //버프 메뉴
    public void BuffMenuNo()
    {
        buff_data.enable = false;
        buff_data.already_clicked = false;
        buff_menu.SetActive(false);
        Time.timeScale = 1;
    }
}
