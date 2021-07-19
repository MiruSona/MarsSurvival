using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerPoison : MonoBehaviour {

    //참조
    private Image poison_panel;
    private Text poison_time;
    private GameDAO.Timer renew_timer = new GameDAO.Timer(0, 1f);
    private GameDAO.PlayerData player_data;
    private Animator player_anim;
    private SystemDAO.GameManagerData gm_data;
    private SystemDAO.FileManager file_manager;

    //사운드
    private AudioSource audio_source;
    private AudioClip beep_sound;

    //초기화
    void Awake () {
        poison_panel = transform.FindChild("PoisonedCanvas").FindChild("PosionPanel").GetComponent<Image>();
        poison_time = transform.FindChild("PoisonedCanvas").FindChild("Time").GetComponent<Text>();
        player_anim = Player.instance.GetComponent<Animator>();

        player_data = GameDAO.instance.player_data;
        gm_data = SystemDAO.instance.gm_data;
        file_manager = SystemDAO.instance.file_manager;

        //사운드
        audio_source = transform.FindChild("PoisonedCanvas").GetComponent<AudioSource>();
        beep_sound = Resources.Load<AudioClip>("Sound/SoundEffect/Beep");
    }

    //엑티브시 타이머 초기화
    void OnEnable()
    {
        player_data.poisoned_timer.timer = player_data.poisoned_timer.term;
        if (Social.localUser.authenticated)
            Social.ReportProgress(GPGSSstatics.achievement_poison, 100.0f, (bool success) => {
                if (success) {
                    Debug.Log("Reported");
                } else {
                }
            });
        RenewUI();
        file_manager.SaveAll();
    }
	
	void Update () {

        //타이머 체크
        if(player_data.poisoned_timer.timer <= 0)
        {
            //상태 원상복귀
            player_anim.Play("Player_Idle");
            player_data.state = GameDAO.PlayerData.State.isAlive;
            gm_data.state = SystemDAO.GameManagerData.State.isPlay;
            player_data.poisoned_timer.timer = 0;
            gameObject.SetActive(false);
            return;
        }

        //시간 갱신
        if (renew_timer.timer < renew_timer.term)
            renew_timer.timer += Time.deltaTime;
        else
        {
            player_data.poisoned_timer.timer -= 1.0f;
            audio_source.PlayOneShot(beep_sound);
            RenewUI();

            renew_timer.timer = 0f;
        }
	}

    //UI갱신
    private void RenewUI()
    {
        //시간 갱신
        float left_minute = Mathf.Floor(player_data.poisoned_timer.timer % 3600.0f / 60.0f);
        float left_second = player_data.poisoned_timer.timer % 60.0f;
        string left_time = string.Format("{0:00} : {1:00}", left_minute, left_second);
        poison_time.text = left_time;

        //알파값 갱신
        Color color = poison_panel.color;
        color.a = player_data.poisoned_timer.timer / player_data.poisoned_timer.term * 0.82f;
        poison_panel.color = color;
    }

    //스킵
    public void Skip()
    {
        Advertisement.Show("rewardedVideo");

        //UI갱신
        player_data.poisoned_timer.timer = 0;
        RenewUI();

        //상태 원상복귀
        player_anim.Play("Player_Idle");
        player_data.state = GameDAO.PlayerData.State.isAlive;
        gm_data.state = SystemDAO.GameManagerData.State.isPlay;

        //세이브
        file_manager.SaveAll();

        //false
        gameObject.SetActive(false);
    }
}
