using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MetalBioPanel : MonoBehaviour {

    public bool isMetal = false;

    private SystemDAO.FileManager file_manager;
    private GameDAO.PlayerData playerData;

    public Image small;
    public Image medium;
    public Image large;

    public Sprite smallMetalSprite;
    public Sprite mediumMetalSprite;
    public Sprite largeMetalSprite;

    public Sprite smallBioSprite;
    public Sprite mediumBioSprite;
    public Sprite largeBioSprite;

    public Image purchaseCheckImage;

    public GameObject purchaseCheckPanel;

    private const int small_value = 20;
    private const int medium_value = 120;
    private const int large_value = 250;

    private int check;

    void OnEnable() {
        file_manager = SystemDAO.instance.file_manager;
        playerData = GameDAO.instance.player_data;
        if (isMetal) {
            small.sprite = smallMetalSprite;
            medium.sprite = mediumMetalSprite;
            large.sprite = largeMetalSprite;
        } else {
            small.sprite = smallBioSprite;
            medium.sprite = mediumBioSprite;
            large.sprite = largeBioSprite;
        }
    }

    public void BtnClick(int size) {
        if (isMetal)
        {
            switch (size)
            {
                case 0:
                    if (!playerData.my_subdata.CheckFull(small_value, 0))
                    {
                        purchaseCheckImage.sprite = smallMetalSprite;
                        purchaseCheckPanel.SetActive(true);
                    }
                    break;
                case 1:
                    if (!playerData.my_subdata.CheckFull(medium_value, 0))
                    {
                        purchaseCheckImage.sprite = mediumMetalSprite;
                        purchaseCheckPanel.SetActive(true);
                    }
                    break;
                case 2:
                    if (!playerData.my_subdata.CheckFull(large_value, 0))
                    {
                        purchaseCheckImage.sprite = largeMetalSprite;
                        purchaseCheckPanel.SetActive(true);
                    }
                    break;
            }
        }
        else
        {
            switch (size)
            {
                case 0:
                    if (!playerData.my_subdata.CheckFull(small_value, 1))
                    {
                        purchaseCheckImage.sprite = smallBioSprite;
                        purchaseCheckPanel.SetActive(true);
                    }
                    break;
                case 1:
                    if (!playerData.my_subdata.CheckFull(medium_value, 1))
                    {
                        purchaseCheckImage.sprite = mediumBioSprite;
                        purchaseCheckPanel.SetActive(true);
                    }
                    break;
                case 2:
                    if (!playerData.my_subdata.CheckFull(large_value, 1))
                    {
                        purchaseCheckImage.sprite = largeBioSprite;
                        purchaseCheckPanel.SetActive(true);
                    }
                    break;
            }
        }
        check = size;
    }

    public void NoBtn() {
        purchaseCheckPanel.SetActive(false);
    }

    public void Buying() { //0 S, 1 M, 2 L
        purchaseCheckPanel.SetActive(false);
        if (check == 0) {
            smallBuying();
        } else if (check == 1) {
            mediumBuying();
        } else if (check == 2) {
            largeBuying();
        }
        file_manager.SaveAll();
    }

    void smallBuying() {
        if (playerData.my_subdata.crystal >= 1) {
            playerData.my_subdata.crystal -= 1;
            if (isMetal)
                playerData.my_subdata.metal += small_value;
            else
                playerData.my_subdata.bio += small_value;
        }
    }
    void mediumBuying() {
        if (playerData.my_subdata.crystal >= 5) {
            playerData.my_subdata.crystal -= 5;
            if (isMetal)
                playerData.my_subdata.metal += medium_value;
            else
                playerData.my_subdata.bio += medium_value;
        }
    }
    void largeBuying() {
        if (playerData.my_subdata.crystal >= 10) {
            playerData.my_subdata.crystal -= 10;
            if (isMetal)
                playerData.my_subdata.metal += large_value;
            else
                playerData.my_subdata.bio += large_value;
        }
    }


    public void Resume() {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
}
