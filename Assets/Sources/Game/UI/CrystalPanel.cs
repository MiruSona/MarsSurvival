using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CrystalPanel : MonoBehaviour {

    //참조
    GameDAO.PlayerData playerData;
    MarketService market_service;

    //값
    private const int small_value = 3;
    private const int medium_value = 18;
    private const int large_value = 40;

    void Start()
    {
        playerData = GameDAO.instance.player_data;
        market_service = MarketService.instance;
    }

    public void smallBuying() {
        if (playerData.my_subdata.CheckFull(small_value, 2))
            return;
        market_service.BuyProductID("test_crystal");
    }
    public void mediumBuying() {
        if (playerData.my_subdata.CheckFull(medium_value, 2))
            return;
        market_service.BuyProductID("crystal_medium");
    }
    public void largeBuying() {
        if (playerData.my_subdata.CheckFull(large_value, 2))
            return;
        market_service.BuyProductID("crystal_large");
    }

    public void Resume() {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
}
