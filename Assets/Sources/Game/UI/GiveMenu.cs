using UnityEngine;
using UnityEngine.Advertisements;
using System.Collections;

public class GiveMenu : MonoBehaviour {

    //참조
    private SystemDAO.FileManager file_manager;
    private GameDAO.PlayerData player_data;

    //초기화
    void Awake()
    {
        file_manager = SystemDAO.instance.file_manager;
        player_data = GameDAO.instance.player_data;
    }

    //켜질때 timescale = 0
    void OnEnable()
    {
        Time.timeScale = 0;
    }

    //광고 + 크리스탈 주는 버튼
    public void SeeADBtn()
    {
        if(!file_manager.commercial_off)
            Advertisement.Show("rewardedVideo");
        player_data.my_subdata.crystal += 2;
        file_manager.give_crystal = true;
        file_manager.SaveGiveCrystal();
        file_manager.SaveAll();
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    //취소버튼
    public void Cancle()
    {
        file_manager.give_crystal = true;
        file_manager.SaveGiveCrystal();
        file_manager.SaveAll();
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }
}
