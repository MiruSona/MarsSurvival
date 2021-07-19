using UnityEngine;
using System.Collections;

public class SaveExitMenu : MonoBehaviour {

    //참조
    private SystemDAO.FileManager file_manager;

	//초기화
	void Start () {
        file_manager = SystemDAO.instance.file_manager;
    }

    public void SaveExitBtn()
    {
        Application.Quit();
    }

    public void Resume()
    {
        gameObject.SetActive(false);
    }
}
