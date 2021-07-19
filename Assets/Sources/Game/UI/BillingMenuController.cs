using UnityEngine;
using System.Collections;

public class BillingMenuController : MonoBehaviour {
    public GameObject crystal_panel;
    public GameObject metal_bio_panel;

    public void Crystal_Panel() {
        crystal_panel.SetActive(true);
        Time.timeScale = 0;
    }
    public void Metal_Bio_Panel(bool isMetal) {
        if (isMetal) {
            metal_bio_panel.GetComponent<MetalBioPanel>().isMetal = true;
        } else if (!isMetal) {
            metal_bio_panel.GetComponent<MetalBioPanel>().isMetal = false;
        }
        metal_bio_panel.SetActive(true);
        Time.timeScale = 0;
    }
}
