using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpaceShip : SingleTon<SpaceShip> {

    //HP관련
    private Image current_hp;
    private Text step_text;
    private Text current_text;
    private int step = 0;

    //참조
    private GameDAO.PlayerData player_data;
    private GameDAO.SpaceShipData spaceship_data;
    private SpriteRenderer sprite_renderer;

    private GameObject electric;
    private GameObject smoke;

	// 초기화
	void Start () {
        player_data = GameDAO.instance.player_data;
        spaceship_data = GameDAO.instance.spaceship_data;
        sprite_renderer = GetComponent<SpriteRenderer>();

        current_hp = transform.FindChild("SpaceShipHP").FindChild("CurrentHP").GetComponent<Image>();
        step_text = transform.FindChild("SpaceShipHP").FindChild("StepText").GetComponent<Text>();
        current_text = transform.FindChild("SpaceShipHP").FindChild("CurrentText").GetComponent<Text>();

        electric = GameObject.Find("Electric");
        smoke = GameObject.Find("Smoke");
    }
	
	//프레임 변화
	void Update () {

        //색 바꾸기
        if (player_data.movement == GameDAO.PlayerData.Movement.isRepairSchedule)
            sprite_renderer.color = spaceship_data.repair_color;
        else
            sprite_renderer.color = Color.white;

        #region step 관련(전체 체력)
        //몇단계 인지 구하기
        step = (int)(spaceship_data.step_hp / spaceship_data.current_max_hp);

        //스프라이트 처리
        sprite_renderer.sprite = spaceship_data.sprites[step];

        //전부 수리(step == Define.spaceship_step)일 경우 엔딩 실행
        if (step == Define.spaceship_step)
        {
            current_text.text = "100%";
            //엔딩
            SystemDAO.instance.gm_data.state = SystemDAO.GameManagerData.State.isEnd;
            return;
        }

        //단계 표시
        step_text.text = (step + 1) + "/" + Define.spaceship_step; 

        //체력바 변경
        switch (step)
        {
            case 0:
                current_hp.color = Color.red;
                break;

            case 1:
                current_hp.color = new Color(1.0f, 0.5f, 0f);
                break;

            case 2:
                current_hp.color = Color.yellow;
                break;

            case 3:
                current_hp.color = new Color(0.7f, 1f, 0.1f);
                electric.SetActive(false);
                break;
                
            case 4:
                current_hp.color = Color.green;
                electric.SetActive(false);
                smoke.SetActive(false);
                break;
        }

        #endregion

        #region 현재 체력
        //현재 HP 구하기
        spaceship_data.SetCurrentHP(step);

        //현재 HP 처리
        current_text.text = (spaceship_data.current_hp / spaceship_data.current_max_hp * 100) + "%";
        current_hp.fillAmount = spaceship_data.current_hp / spaceship_data.current_max_hp; 
        #endregion
    }
}
