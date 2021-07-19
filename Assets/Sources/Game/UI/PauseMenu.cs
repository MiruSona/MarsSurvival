using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

    //참조
    private Button sound_btn;
    private SystemDAO.FileManager file_manager;
    public GameObject help_menu;

    //초기화
    void Start() {
        file_manager = SystemDAO.instance.file_manager;
        sound_btn = transform.FindChild("Sound").GetComponent<Button>();

        //사운드 옵션 체크
        if (file_manager.option_data.sound) {
            AudioListener.volume = 1;
            sound_btn.interactable = true;
        } else {
            AudioListener.volume = 0;
            sound_btn.interactable = false;
        }
    }

    void OnEnable() {
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

    //public void GPGSBtn() {
    //    if (Social.localUser.authenticated)
    //        Social.ShowAchievementsUI();
    //}

    //public void LBBtn()
    //{
    //    if (Social.localUser.authenticated)
    //        Social.ShowLeaderboardUI();
    //}
}
