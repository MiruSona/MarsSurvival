using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

    //참조
    private Button sound_btn;
    private Button commercial_off_btn;
    private SystemDAO.FileManager file_manager;
    public GameObject help_menu;
    MarketService market_service;

    //초기화
    void Start() {
        file_manager = SystemDAO.instance.file_manager;
        market_service = MarketService.instance;
        sound_btn = transform.FindChild("Sound").GetComponent<Button>();
        commercial_off_btn = transform.FindChild("CommercialOffBtn").GetComponent<Button>();

        //사운드 옵션 체크
        if (file_manager.option_data.sound) {
            AudioListener.volume = 1;
            sound_btn.interactable = true;
        } else {
            AudioListener.volume = 0;
            sound_btn.interactable = false;
        }
    }

    void Update() {
        if(file_manager.commercial_off && commercial_off_btn.interactable) {
            commercial_off_btn.interactable = false;
        }
    }

    //일시정지 풀기
    public void Resume() {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }

    //게임 종료
    public void ExitGame() {
        file_manager.SaveAll();
        Application.Quit();
    }

    //메인 메뉴로
    public void MainMenu() {
        Time.timeScale = 1f;

        file_manager.SaveAll();
        Destroy(SystemDAO.instance.gameObject);
        SceneManager.LoadScene("MainMenu");
    }

    //사운드 on off
    public void SoundOnOff() {
        if (file_manager.option_data.sound) {
            file_manager.option_data.sound = false;
            sound_btn.interactable = false;
            AudioListener.volume = 0;
        } else {
            file_manager.option_data.sound = true;
            sound_btn.interactable = true;
            AudioListener.volume = 1;
        }
    }

    //Help메뉴 켜기
    public void HelpMenu() {
        help_menu.SetActive(true);
    }

    //광고 제거
    public void CommercialOff() {
        market_service.BuyProductID("commercial_off");
        file_manager.commercial_off = true;
        file_manager.SaveCommerciealOff();
    }

    public void GPGSBtn() {
        if (Social.localUser.authenticated)
            Social.ShowAchievementsUI();
    }

    public void LBBtn() {
        if (Social.localUser.authenticated)
            Social.ShowLeaderboardUI();
    }
}
