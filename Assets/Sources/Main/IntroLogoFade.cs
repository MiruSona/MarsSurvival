using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class IntroLogoFade : MonoBehaviour {

	// Use this for initialization
	void Start () {
        gameObject.GetComponent<Image>().CrossFadeAlpha(0, 0, true);
        StartCoroutine(FadeInOut());
	}

    IEnumerator FadeInOut() {
        gameObject.GetComponent<Image>().CrossFadeAlpha(1, 0.75f, true);
        yield return new WaitForSeconds(1.5f);
        gameObject.GetComponent<Image>().CrossFadeAlpha(0, 0.75f, true);
        yield return new WaitForSeconds(0.75f);
        SceneManager.LoadScene("MainMenu");
    }
}
