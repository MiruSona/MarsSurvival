using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using GoogleMobileAds.Api;

public class PauseMenu : MonoBehaviour
{

    //참조
    private Button sound_btn;
    private SystemDAO.FileManager file_manager;
    public GameObject help_menu;
    private BannerView bannerView;

    //초기화
    void Start()
    {
        file_manager = SystemDAO.instance.file_manager;
        sound_btn = transform.FindChild("Sound").GetComponent<Button>();

        //사운드 옵션 체크
        if (file_manager.option_data.sound)
        {
            AudioListener.volume = 1;
            sound_btn.interactable = true;
        }
        else
        {
            AudioListener.volume = 0;
            sound_btn.interactable = false;
        }

        RequestBanner();
    }

    void OnEnable()
    {
        if(bannerView != null)
            bannerView.Show();
    }

    //일시정지 풀기
    public void Resume()
    {
        bannerView.Hide();
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }

    //게임 종료
    public void ExitGame()
    {
        file_manager.SaveAll();
        Application.Quit();
    }

    //메인 메뉴로
    public void MainMenu()
    {
        bannerView.Hide();
        file_manager.SaveAll();
        Destroy(GameDAO.instance.gameObject);
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    //사운드 on off
    public void SoundOnOff()
    {
        if (file_manager.option_data.sound)
        {
            file_manager.option_data.sound = false;
            sound_btn.interactable = false;
            AudioListener.volume = 0;
        }
        else
        {
            file_manager.option_data.sound = true;
            sound_btn.interactable = true;
            AudioListener.volume = 1;
        }
    }

    //Help메뉴 켜기
    public void HelpMenu()
    {
        bannerView.Hide();
        help_menu.SetActive(true);
    }


    //Google Admob Banner
    private void RequestBanner()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3832257678078001/4447551371";
#elif UNITY_IPHONE
        string adUnitId = "INSERT_IOS_BANNER_AD_UNIT_ID_HERE";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Create a 320x50 banner at the top of the screen.
        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the banner with the request.
        bannerView.LoadAd(request);
    }

}
