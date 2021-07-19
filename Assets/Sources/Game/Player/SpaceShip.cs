using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//using UnityEngine.SocialPlatforms;
//using GooglePlayGames;
//using GooglePlayGames.BasicApi;
//using GooglePlayGames.BasicApi.SavedGame;

public class SpaceShip : SingleTon<SpaceShip> {

    //HP관련
    [System.NonSerialized]
    public Image current_hp;
    private Text step_text;
    private Text current_text;

    //참조
    private GameDAO.PlayerData player_data;
    private GameDAO.SpaceShipData spaceship_data;
    private SpriteRenderer sprite_renderer;

    private GameObject electric;
    private GameObject smoke;

    private bool init_check = true;
    private int achivement_check = 0;

    // 초기화
    void Start() {
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
    void Update() {
                
        //색 바꾸기
        if (player_data.movement == GameDAO.PlayerData.Movement.isRepairSchedule)
            sprite_renderer.color = spaceship_data.repair_color;
        else
            sprite_renderer.color = Color.white;

        #region step 관련(전체 체력)
        //몇단계 인지 구하기
        spaceship_data.step = (int)(spaceship_data.step_hp / spaceship_data.current_max_hp);

        //스프라이트 처리
        sprite_renderer.sprite = spaceship_data.sprites[spaceship_data.step];

        //전부 수리(step == Define.spaceship_step)일 경우 엔딩 실행
        if (spaceship_data.step == Define.spaceship_step) {
            current_text.text = "100%";
            current_hp.fillAmount = 1f;
            //엔딩
            //if (Social.localUser.authenticated)
            //    Social.ReportProgress(GPGSSstatics.achievement_ready_to_go_back_to_the_earth, 100.0f, (bool success) => {
            //        if (success) {
            //            Debug.Log("Reported");
            //        } else {
            //        }
            //    });
            SystemDAO.instance.gm_data.state = SystemDAO.GameManagerData.State.isEnd;
            return;
        }

        //단계 표시
        step_text.text = (spaceship_data.step + 1) + "/" + Define.spaceship_step;

        //체력바 변경
        switch (spaceship_data.step) {
            case 0:
                current_hp.color = Color.red;
                break;

            case 1:
                current_hp.color = new Color(1.0f, 0.5f, 0f);
                if (spaceship_data.step_hp <= 100)
                    current_hp.fillAmount = 0;
                break;

            case 2:
                current_hp.color = Color.yellow;
                if (spaceship_data.step_hp <= 200)
                    current_hp.fillAmount = 0;
                break;

            case 3:
                current_hp.color = new Color(0.7f, 1f, 0.1f);
                if (spaceship_data.step_hp <= 300)
                    current_hp.fillAmount = 0;
                electric.SetActive(false);
                break;

            case 4:
                current_hp.color = Color.green;
                if (spaceship_data.step_hp <= 400)
                    current_hp.fillAmount = 0;
                electric.SetActive(false);
                smoke.SetActive(false);
                break;
        }

        if (achivement_check != spaceship_data.step) {
            //if (Social.localUser.authenticated && achivement_check >= 0) {
            //    Social.ReportProgress(GPGSSstatics.achievement_repairing, 100.0f, (bool success) => {
            //        if (success) {
            //            Debug.Log("Reported");
            //        } else {
            //        }
            //    });
            //}
            //if (Social.localUser.authenticated && achivement_check >= 1) {
            //    Social.ReportProgress(GPGSSstatics.achievement_twice, 100.0f, (bool success) => {
            //        if (success) {
            //            Debug.Log("Reported");
            //        } else {
            //        }
            //    });
            //}
            //if (Social.localUser.authenticated && achivement_check >= 2) {
            //    Social.ReportProgress(GPGSSstatics.achievement_little_boring, 100.0f, (bool success) => {
            //        if (success) {
            //            Debug.Log("Reported");
            //        } else {
            //        }
            //    });
            //}
            //if (Social.localUser.authenticated && achivement_check >= 3) {
            //    Social.ReportProgress(GPGSSstatics.achievement_only_one_to_go, 100.0f, (bool success) => {
            //        if (success) {
            //            Debug.Log("Reported");
            //        } else {
            //        }
            //    });
            //}
            //achivement_check = spaceship_data.step;
        }

        #endregion

        #region 현재 체력
        //현재 HP 구하기
        spaceship_data.SetCurrentHP(spaceship_data.step);

        if (init_check) {
            if (spaceship_data.current_hp == 50.0f)
                current_hp.fillAmount = 0.5f;
            else
                current_hp.fillAmount = spaceship_data.current_hp / spaceship_data.current_max_hp;
        }

        //현재 HP 처리
        current_text.text = string.Format("{0:0.0}%", (spaceship_data.current_hp / spaceship_data.current_max_hp * 100));
        #endregion
        init_check = false;
    }
}
