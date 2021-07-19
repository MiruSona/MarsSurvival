using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Result : MonoBehaviour {

    //참조
    private SystemDAO.GlobalState global_state;
    private Text play_time;

    void Awake()
    {
        play_time = transform.FindChild("Time").GetComponent<Text>();
        global_state = SystemDAO.instance.global_state;
    }

    //enable
    void OnEnable() {
        play_time.text = string.Format(
            "{0:00} : {1:00} : {2:00}", 
            (int)global_state.real_time / 3600, 
            (int)global_state.real_time % 3600 / 60, 
            (int)global_state.real_time % 60);
    }

    public void GoMain()
    {
        //DAO 파괴
        Destroy(GameDAO.instance.gameObject);
        SceneManager.LoadScene("MainMenu");
    }
}
