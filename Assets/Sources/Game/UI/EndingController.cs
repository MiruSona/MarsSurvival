using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EndingController : MonoBehaviour {

    //엔딩 이미지
    public Image[] endingImg = new Image[5];
    public GameObject result_panel;

    //초기화
    void Start() {
        GetComponent<Image>().CrossFadeAlpha(0, 0, true);
        for(int i = 0; i<endingImg.Length; i++) {
            endingImg[i].CrossFadeAlpha(0, 0, true);
        }
        GetComponent<Image>().CrossFadeAlpha(1, 4f, true);
        StartCoroutine(EndingImage());
    }
    
    IEnumerator EndingImage() {
        for (int i = 0; i < endingImg.Length; i++) {
            yield return new WaitForSeconds(4f);
            endingImg[i].GetComponent<Image>().CrossFadeAlpha(1, 0.8f, true);
        }
        yield return new WaitForSeconds(4f);
        result_panel.SetActive(true);
        yield return null;
    }
}
