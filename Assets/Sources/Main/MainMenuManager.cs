using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
//using UnityEngine.Advertisements;

using System;

public class MainMenuManager : MonoBehaviour {

    //참조
    private SystemDAO.FileManager file_manager;
    private Credit credit;
    private GameObject warning_panel;
    private GameObject artifact_menu;
    private GameObject Intro;
    private GameObject warning_text;
    private GameObject no_save_text;    

    //초기화
    void Start() {
        file_manager = SystemDAO.instance.file_manager;
        credit = transform.FindChild("Credits").GetComponent<Credit>();
        warning_panel = transform.FindChild("Warning").gameObject;
        artifact_menu = transform.FindChild("ArtifactMenu").gameObject;
        Intro = transform.FindChild("IntroPanel").gameObject;

        warning_text = transform.FindChild("Warning_text").gameObject;
        no_save_text = transform.FindChild("NoSaveText").gameObject;

        //아티펙트 로드
        file_manager.LoadArtifact();

        //세이브 파일 있는지 체크
        file_manager.CheckSave();

        //광고 로드
        file_manager.LoadADData();

        //옵션 적용
        file_manager.LoadFile(Define.option_data_key);
        if (file_manager.option_data.sound)
            AudioListener.volume = 1;
        else
            AudioListener.volume = 0;
    }

    void FixedUpdate() {
        //백키 누르면 종료
        if (Input.GetKey(KeyCode.Escape))
            Application.Quit();
    }

    //뉴게임 버튼
    public void NewGameBtn() {
        //세이브 파일 있으면 경고
        if (file_manager.have_save)
            warning_panel.SetActive(true);
        //세이브 파일 없으면 인트로 실행
        else
            Intro.SetActive(true);
    }

    //컨티뉴 버튼
    public void ContinueBtn() {
        if (file_manager.have_save)
            SceneManager.LoadScene("GameMain");
        else if (!file_manager.have_save)
            no_save_text.SetActive(true);
    }

    //크레딧 버튼
    public void CreditBtn() {
        if (!credit.gameObject.activeSelf)
            credit.gameObject.SetActive(true);
        else {
            if (credit.alpha_enum == Credit.Alpha.Stay)
                credit.alpha_enum = Credit.Alpha.Sub;
        }
    }

    //아티펙트 버튼
    public void ArtifactBtn() {
        artifact_menu.SetActive(true);
    }

    //public void CommercialBtn() {
    //    if (!file_manager.have_save)
    //        no_save_text.SetActive(true);
    //    else if (file_manager.have_save)
    //    {
    //        if (file_manager.ad_data.day != DateTime.Now.Day)
    //        {
    //            file_manager.ad_data.shown_num = 0;
    //            file_manager.ad_data.look_ad_num = 0;
    //            file_manager.SaveADData();
    //        }
    //        //show
    //        //    if (file_manager.ad_data.shown_num < 1 && Advertisement.IsReady()) {
    //        //        Advertisement.Show("rewardedVideo");
    //        //        if (Advertisement.isShowing) {
    //        //            file_manager.ad_data.look_ad_num += 1;
    //        //            file_manager.ad_data.shown_num += 1;
    //        //            file_manager.ad_data.day = DateTime.Now.Day;
    //        //            file_manager.SaveADData();

    //        //        }
    //        //    }

    //        //    //not initialize yet
    //        //    else if (!warning_text.activeSelf && file_manager.ad_data.shown_num >= 1 | file_manager.ad_data.look_ad_num >= 1) //날짜값 받아서 판정하는것도 같이 넣어주
    //        //        warning_text.SetActive(true);

    //        //    Debug.Log(file_manager.ad_data.day);
    //        //    Debug.Log(file_manager.ad_data.look_ad_num);
    //    }
    //}

    //public void GPGSBtn() {
    //    if (Social.localUser.authenticated)
    //        Social.ShowAchievementsUI();
    //}

    //public void LBBtn()
    //{
    //    if (Social.localUser.authenticated)
    //        Social.ShowLeaderboardUI();
    //}
}
