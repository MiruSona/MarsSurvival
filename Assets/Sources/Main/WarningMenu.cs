using UnityEngine;
using System.Collections;

public class WarningMenu : MonoBehaviour {

    //참조
    private SystemDAO.FileManager file_manager;
    public GameObject Intro;

    //초기화
    void Start () {
        file_manager = SystemDAO.instance.file_manager;
    }

    //Yes버튼
    public void YesBtn()
    {
        //세이브 삭제 후 실행
        file_manager.DeleteBase();
        Intro.SetActive(true);
    }

    //No버튼
    public void NoBtn()
    {
        //그냥 false로
        file_manager.have_save = true;
        gameObject.SetActive(false);
    }
}
