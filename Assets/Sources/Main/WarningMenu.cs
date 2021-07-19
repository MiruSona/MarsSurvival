using UnityEngine;
using System.Collections;

public class WarningMenu : MonoBehaviour {

    //참조
    private SystemDAO.FileManager file_manager;
    public GameObject hardmod_menu;
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
        //하드모드 여부 창
        if (file_manager.clear_num == 0)
            Intro.SetActive(true);
        else
            hardmod_menu.SetActive(true);
    }

    //No버튼
    public void NoBtn()
    {
        //그냥 false로
        file_manager.have_save = true;
        gameObject.SetActive(false);
    }
}
