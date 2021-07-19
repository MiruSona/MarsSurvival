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
    private Button continue_btn;
    private Image dark_panel;

    //색변경
    private Color alpha_color = new Color(0f, 0f, 0f, 0f); 
    
    //초기화
	void Start () {
        gm_data = SystemDAO.instance.gm_data;
        file_manager = SystemDAO.instance.file_manager;
        player_anim = Player.instance.gameObject.GetComponent<Animator>();
        player_data = GameDAO.instance.player_data;
        die_panel = transform.FindChild("DiePanel").gameObject;
        continue_btn = die_panel.transform.FindChild("Continue").GetComponent<Button>();
        dark_panel = GetComponent<Image>();
	}
	
    //패널 처리
	void Update () {
        if(alpha_color.a < 0.5f)
        {
            alpha_color.a += 0.01f;
            dark_panel.color = alpha_color;
        }
        else
        {
            die_panel.SetActive(true);
            if (player_data.my_subdata.crystal < 2)
                continue_btn.interactable = false;
        }
    }

    public void Continue()
    {
        //애니메이션 처리
        player_anim.Play("Player_Idle");

        //데이터 처리
        player_data.my_subdata.SubAll(new GameDAO.SubstanceData(0, 0, 2));
        player_data.hp = player_data.max_hp;
        player_data.movement = GameDAO.PlayerData.Movement.isReady;
        player_data.state = GameDAO.PlayerData.State.isDamaged;
        gm_data.state = SystemDAO.GameManagerData.State.isPlay;
        die_panel.SetActive(false);
        gameObject.SetActive(false);
    }

    public void Die()
    {
        //아티펙트 제외 다삭제
        file_manager.DeleteBase();

        gm_data.state = SystemDAO.GameManagerData.State.isInit;
        Destroy(GameDAO.instance.gameObject);
        SceneManager.LoadScene("MainMenu");
    }
}
