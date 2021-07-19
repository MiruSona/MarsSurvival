using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIController : MonoBehaviour {

    //참조
    private Player player;
    private GameDAO.PlayerData player_data;
    public GameObject pause_menu;
    public GameObject upgrade_menu;
    public GameObject build_menu;

    //상태표시
    private Image hp_bar;
    private Image bullet_bar;

    //쿨타임 표시
    private GameObject cool_panel;
    private Image cool_panel_image;

    //FPS 표시
    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 4 / 100;
        style.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        float fps = 1.0f / Time.deltaTime;
        string text = string.Format("({0:.0} fps)", fps);
        GUI.Label(rect, text, style);
    }

    //초기화
    void Start () {
        player = Player.instance;
        player_data = GameDAO.instance.player_data;

        cool_panel = transform.FindChild("CoolPanel").gameObject;
        cool_panel_image = cool_panel.GetComponent<Image>();
        hp_bar = transform.FindChild("HPBar").GetComponent<Image>();
        bullet_bar = transform.FindChild("BulletBar").GetComponent<Image>();
    }
	
	void FixedUpdate () {
        //HP 표시
        hp_bar.fillAmount = player_data.hp / player_data.max_hp;

        //총알 표시
        bullet_bar.fillAmount = 1.0f * (player_data.weapon_data.bullet_num / player_data.weapon_data.bullet_max);

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
        //공중에 있으면 실행 X
        if (!player_data.grounded)
            return;

        player_data.FacingRight();
        player_data.movement = GameDAO.PlayerData.Movement.isMove;
    }

    //왼쪽으로 움직이기
    public void MoveLeft()
    {
        //공중에 있으면 실행 X
        if (!player_data.grounded)
            return;

        player_data.FacingLeft();
        player_data.movement = GameDAO.PlayerData.Movement.isMove;
    }

    //움직임 멈추기(대기)
    public void MoveStop()
    {
        player_data.movement = GameDAO.PlayerData.Movement.isReady;
    }

    //공격
    public void Attack()
    {
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
        //공격 중 해제
        player_data.attacking = false;
    }
    
    //일시정지 메뉴
    public void Pause()
    {
        pause_menu.SetActive(true);
        Time.timeScale = 0;
    }

    //업그레이드 메뉴
    public void Upgrade()
    {
        upgrade_menu.SetActive(true);
        Time.timeScale = 0;
    }

    //업그레이드 메뉴
    public void Build()
    {
        build_menu.SetActive(true);
        Time.timeScale = 0;
    }
}
