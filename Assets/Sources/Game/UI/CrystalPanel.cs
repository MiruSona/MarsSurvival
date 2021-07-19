using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CrystalPanel : MonoBehaviour {

    //참조
    GameDAO.PlayerData playerData;
    SystemDAO.FileManager file_manager;

    //값
    private const int small_value = 3;
    private const int medium_value = 18;
    private const int large_value = 40;

    void Start()
    {
        playerData = GameDAO.instance.player_data;
        file_manager = SystemDAO.instance.file_manager;
    }

    public void smallBuying() {
        if (playerData.my_subdata.CheckFull(small_value, 2))
            return;

        playerData.my_subdata.crystal += small_value;
        file_manager.SaveAll();
    }
    public void mediumBuying() {
        if (playerData.my_subdata.CheckFull(medium_value, 2))
            return;

        playerData.my_subdata.crystal += medium_value;
        file_manager.SaveAll();
    }
    public void largeBuying() {
        if (playerData.my_subdata.CheckFull(large_value, 2))
            return;

        playerData.my_subdata.crystal += large_value;
        file_manager.SaveAll();
    }

    public void Resume() {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
}
