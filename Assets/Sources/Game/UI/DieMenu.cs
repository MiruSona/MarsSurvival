using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class DieMenu : MonoBehaviour {

    //참조
    private SystemDAO.GameManagerData gm_data;
    private SystemDAO.FileManager file_manager;
    private Animator player_anim;
    private GameDAO.PlayerData player_data;
    private GameObject die_panel;
    private GameObject die_warning;
    public GameObject crystal_panel;
    private Image dark_panel;
    private Sprite[] icon = new Sprite[2];
    private Image continue_icon;

    //색변경
    private Color alpha_color = new Color(0f, 0f, 0f, 0f); 
    
    //초기화
	void Start () {
        gm_data = SystemDAO.instance.gm_data;
        file_manager = SystemDAO.instance.file_manager;
        player_anim = Player.instance.gameObject.GetComponent<Animator>();
        player_data = GameDAO.instance.player_data;
        die_panel = transform.FindChild("DiePanel").gameObject;
        die_warning = transform.FindChild("DieWarnig").gameObject;
        dark_panel = GetComponent<Image>();
        icon[0] = Resources.Load<Sprite>("Sprite/Game/UI/Crystal_Icon_die");
        icon[1] = Resources.Load<Sprite>("Sprite/Main/UI/Advertisement0");
        continue_icon = transform.FindChild("DiePanel").FindChild("Continue").FindChild("Icon").GetComponent<Image>();

        if (file_manager.ad_data.ad_rebirth)
            continue_icon.sprite = icon[0];
        else {
            continue_icon.sprite = icon[1];
        }
    }
	
    //패널 처리
	void Update () {
        if(alpha_color.a < 0.5f)
        {
            alpha_color.a += 0.01f;
            dark_panel.color = alpha_color;
        }
        else
            die_panel.SetActive(true);
    }
    //이어하기
    public void Continue()
    {
        //데이터 처리
        if (file_manager.ad_data.ad_rebirth) {
            //크리스탈 모자라는 경우
            if (player_data.my_subdata.crystal < 2)
            {
                crystal_panel.SetActive(true);
                return;
            }
                
            player_data.my_subdata.SubAll(new GameDAO.SubstanceData(0, 0, 2));
        }
        else {
            continue_icon.sprite = icon[0];
            //부활 여부 저장
            file_manager.ad_data.ad_rebirth = true;
            file_manager.SaveADData();
        }
        
        //플레이어 처리
        player_anim.Play("Player_Idle");
        player_data.hp = player_data.max_hp;
        player_data.movement = GameDAO.PlayerData.Movement.isReady;
        player_data.state = GameDAO.PlayerData.State.isDamaged;
        gm_data.state = SystemDAO.GameManagerData.State.isPlay;
        //세이브
        file_manager.SaveAll();
        Time.timeScale = 1;
        die_panel.SetActive(false);
        gameObject.SetActive(false);
    }
    
    //죽음을 택하겠다!
    public void Die()
    {
        die_warning.SetActive(true);
    }

    //경고창 Yes
    public void Yes()
    {
        //아티펙트 제외 다삭제
        file_manager.DeleteBase();

        gm_data.state = SystemDAO.GameManagerData.State.isInit;
        SceneManager.LoadScene("MainMenu");
        Destroy(GameDAO.instance.gameObject);
    }

    //경고창 No
    public void No()
    {
        die_warning.SetActive(false);
    }
}
