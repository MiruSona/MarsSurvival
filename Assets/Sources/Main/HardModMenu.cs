using UnityEngine;
using System.Collections;

public class HardModMenu : MonoBehaviour {

    //참조
    private SystemDAO.FileManager file_manager;
    public GameObject Intro;

    //초기화
    void Start()
    {
        file_manager = SystemDAO.instance.file_manager;
    }

    //Yes버튼
    public void YesBtn()
    {
        file_manager.hard_mod = true;
        file_manager.SaveHardMod();
        Intro.SetActive(true);
    }

    //No버튼
    public void NoBtn()
    {
        file_manager.hard_mod = false;
        file_manager.SaveHardMod();
        Intro.SetActive(true);
    }
}
