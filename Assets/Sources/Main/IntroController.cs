using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class IntroController : MonoBehaviour {

    //참조
    private SystemDAO.FileManager file_manager;

    //엔딩 이미지
    private Image my_img;
    private Image[] intro_img = new Image[3];

    //초기화
    void Awake()
    {
        file_manager = SystemDAO.instance.file_manager;
        my_img = GetComponent<Image>();
        for (int i = 0; i < intro_img.Length; i++)
            intro_img[i] = transform.FindChild("Img_" + i).GetComponent<Image>();

        //알파값 0으로
        my_img.CrossFadeAlpha(0, 0, true);

        //자식들 알파값도 0으로
        for (int i = 0; i < intro_img.Length; i++)
            intro_img[i].CrossFadeAlpha(0, 0, true);
    }

    //스타트
    void OnEnable()
    {
        //세이브 있으면 실행X
        if (file_manager.have_save)
            return;

        //서서히 밝아지게
        my_img.CrossFadeAlpha(1, 2f, true);

        //코루틴 시작
        StartCoroutine(IntroImage());
    }

    //인트로
    IEnumerator IntroImage()
    {
        //첫 이미지는 2.5초 뒤 보이게
        yield return new WaitForSeconds(2.5f);
        intro_img[0].CrossFadeAlpha(1, 0.8f, true);

        //각 이미지가 3초 마다 점점 보이게
        for (int i = 1; i < intro_img.Length; i++)
        {
            yield return new WaitForSeconds(3f);
            intro_img[i].CrossFadeAlpha(1, 0.8f, true);
        }

        //2초 뒤 이미지 전부 알파값 0으로(어두워 지게)
        yield return new WaitForSeconds(2f);        
        for (int i = 0; i < intro_img.Length - 1; i++)
            intro_img[i].color = Color.clear;

        //마지막 이미지는 천천히 어두워지게
        intro_img[2].CrossFadeAlpha(0, 0.8f, true);

        //다시 2초 뒤 게임 시작
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("GameMain");
        yield return null;
    }
}
