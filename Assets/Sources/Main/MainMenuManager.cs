using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuManager : MonoBehaviour {

    //참조
    private SystemDAO.FileManager file_manager;
    private Credit credit;
    private GameObject warning_panel;
    private GameObject artifact_menu;
    private GameObject continue_btn;

    //초기화
    void Start()
    {
        file_manager = SystemDAO.instance.file_manager;
        credit = transform.FindChild("Credits").GetComponent<Credit>();
        warning_panel = transform.FindChild("Warning").gameObject;
        artifact_menu = transform.FindChild("ArtifactMenu").gameObject;
        continue_btn = transform.FindChild("Continue").gameObject;
    }

    void Update()
    {
        //세이브 있으면 컨티뉴 활성화
        if (file_manager.have_save)
            continue_btn.SetActive(true);
        else
            continue_btn.SetActive(false);

        //백키 누르면 종료
        if (Input.GetKey(KeyCode.Escape))
            Application.Quit();
    }

    //뉴게임 버튼
    public void NewGameBtn() {
        //세이브 파일 있으면 경고
        if (file_manager.have_save)
            warning_panel.SetActive(true);
        //세이브 파일 없으면 바로 실행
        else
            SceneManager.LoadScene("GameMain");
    }

    //컨티뉴 버튼
    public void ContinueBtn()
    {
        SceneManager.LoadScene("GameMain");
    }

    //크레딧 버튼
    public void CreditBtn()
    {
        if (!credit.gameObject.activeSelf)
            credit.gameObject.SetActive(true);
        else
        {
            if (credit.alpha_enum == Credit.Alpha.Stay)
                credit.alpha_enum = Credit.Alpha.Sub;
        }
    }

    //아티펙트 버튼
    public void ArtifactBtn()
    {
        artifact_menu.SetActive(true);
    }
}
