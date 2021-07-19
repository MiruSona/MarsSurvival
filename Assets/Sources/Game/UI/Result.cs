using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Result : MonoBehaviour {

    //참조
    private SystemDAO.GlobalState global_state;
    private SystemDAO.GameManagerData gm_data;
    private SystemDAO.FileManager file_manager;
    private Text play_time;

    void Awake()
    {
        play_time = transform.FindChild("Time").GetComponent<Text>();
        global_state = SystemDAO.instance.global_state;
        gm_data = SystemDAO.instance.gm_data;
        file_manager = SystemDAO.instance.file_manager;
    }

    //enable
    void OnEnable() {
        play_time.text = string.Format(
            "{0:00} : {1:00} : {2:00}", 
            (int)global_state.real_time / 3600, 
            (int)global_state.real_time / 60, 
            (int)global_state.real_time % 60);
    }

    public void GoMain()
    {
        //파일 삭제
        file_manager.DeleteBase();
        file_manager.SaveArtifact();

        //DAO 파괴
        Destroy(GameDAO.instance.gameObject);
        SceneManager.LoadScene("MainMenu");
    }
}
