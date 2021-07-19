using UnityEngine;
using System.Collections;

public class ReviewMenu : MonoBehaviour {

    //참조
    private SystemDAO.FileManager file_manager;

	//초기화
	void Awake () {
        file_manager = SystemDAO.instance.file_manager;
    }

    //켜질때 timescale = 0
    void OnEnable()
    {
        Time.timeScale = 0;
    }

    public void YesBtn()
    {
        //리뷰 창 띄우기
        Application.OpenURL("market://details?id=hellofit.mars");
        //리뷰 본지 여부 = ture & 저장
        file_manager.see_review = true;
        file_manager.SaveSeeReview();
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    public void NoBtn()
    {
        //리뷰 본지 여부 = ture & 저장
        file_manager.see_review = true;
        file_manager.SaveSeeReview();
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    public void LaterBtn()
    {
        //그냥 끔
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }
}
