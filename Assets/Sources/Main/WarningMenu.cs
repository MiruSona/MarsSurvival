using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class WarningMenu : MonoBehaviour {

    //참조
    private SystemDAO.FileManager file_manager;

    //초기화
    void Start () {
        file_manager = SystemDAO.instance.file_manager;
    }

    //Yes버튼
    public void YesBtn()
    {
        //세이브 삭제 후 실행
        file_manager.DeleteBase();
        SceneManager.LoadScene("GameMain");
    }

    //No버튼
    public void NoBtn()
    {
        //그냥 false로
        gameObject.SetActive(false);
    }
}
