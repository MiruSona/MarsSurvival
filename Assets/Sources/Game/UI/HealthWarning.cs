using UnityEngine;
using System.Collections;

public class HealthWarning : MonoBehaviour {

    //참조
    private SystemDAO.FileManager file_manager;

	//켜지면 멈춤 + 초기화
	void OnEnable () {
        Time.timeScale = 0;
        file_manager = SystemDAO.instance.file_manager;
    }

    //일시정지 풀기
    public void Resume()
    {
        Time.timeScale = 1;
        file_manager.see_hp_warnig = true;
        file_manager.SaveSeeHPWarning();
        gameObject.SetActive(false);
    }
}
