using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CommWarnText : MonoBehaviour {

    void OnEnable() {
        gameObject.GetComponent<Text>().CrossFadeAlpha(0, 0, true);
        gameObject.GetComponent<Text>().CrossFadeAlpha(1, 1.5f, true);
        StartCoroutine(fadeout());
    }

    IEnumerator fadeout() {
        yield return new WaitForSeconds(2f);
        gameObject.GetComponent<Text>().CrossFadeAlpha(0, 0.5f, true);
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
}
