using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenu : MonoBehaviour {

    //참조
    private Button sound_btn;
    private SystemDAO.FileManager file_manager;
    private GameManager game_manager;
    private SystemDAO.GameManagerData gm_data;

    public GameObject artifact_menu;

    //초기화
    void Start () {
        file_manager = SystemDAO.instance.file_manager;
        game_manager = GameManager.instance;
        gm_data = SystemDAO.instance.gm_data;
        sound_btn = transform.FindChild("Sound").GetComponent<Button>();
    }
	
    //일시정지 풀기
    public void Resume()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }

    //게임 종료
    public void ExitGame()
    {
        file_manager.SaveAll();
        Application.Quit();
    }

    //메인 메뉴로
    public void MainMenu()
    {
        file_manager.SaveAll();
        Destroy(GameDAO.instance.gameObject);
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    //사운드 on off
    public void SoundOnOff()
    {
        if (sound_btn.interactable)
        {
            sound_btn.interactable = false;
            AudioListener.volume = 0;
        }
        else
        {
            sound_btn.interactable = true;
            AudioListener.volume = 1;
        }
    }

    //아티펙트 메뉴
    public void Artifact()
    {
        artifact_menu.SetActive(true);
    }

    //세이브 삭제 - 테스트
    public void DeleteSave()
    {
        file_manager.DeletAllSave();
        Application.Quit();
    }
}
