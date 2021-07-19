using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerTalk : MonoBehaviour {


    private float timer;
    private float talkTime;

    public GameObject text;

    private string[] sentences = new string[19] {
        "I wanna go home.",
        "CHICKEN!!",
        "I can endure this time, \nI can do all things, \nand I can escape! ",
        "Be positive, \nI'm having a marvelous time, \nthat nobody can experience",
        "Only trees, rocks, and....um... \n\"Monsters\".",
        "Yeah, I see. \nEarth is pretty much...pretty. \nThis place is so desolate.",
        "It's over. I'll gonna die.",
        "If I have a baby in future, \nI'll tell this 'Unbelievable' saga.",
        "*Sniff* \nUrrgh, horrible smells... ",
        "Oh please... help...",
        "Did you hear about the boy \nwhose whole left side was cut off? \nHe's all right now.",
        "Too hot...",
        "Almond is die... \nIt becomes Di-A-mond. *laugh*",
        "Da da dum da dum da da da..",
        "PIZZA!!",
        "I want to watch Tv",
        "What have I done to deserve this?",
        "Darn it.",
        "I hate this planet." };

    void Start() {
        text.GetComponent<Text>().CrossFadeAlpha(0f, 0f, false);
        talkTime = Random.Range(Define.Day_Cycle, Define.Day_Cycle + Define.Day_1D3_Cycle);
    }

    void Update() {
        timer += Time.deltaTime;
        if(timer > talkTime) {
            //talk
            StartCoroutine(Talk());
            timer = 0f;
            talkTime = Random.Range(Define.Day_Cycle, Define.Day_Cycle + Define.Day_1D3_Cycle);
        }
    }

    IEnumerator Talk() {
        text.SetActive(true);
        text.GetComponent<Text>().text = sentences[(int)Random.Range(0, 19f)];
        text.GetComponent<Text>().CrossFadeAlpha(1f, 1f, false);
        iTween2.MoveTo(text, iTween2.Hash("y", text.transform.position.y + 0.2f, "time", 4f));
        yield return new WaitForSeconds(3.5f);
        text.GetComponent<Text>().CrossFadeAlpha(0f, 0.5f, false);
        yield return new WaitForSeconds(0.5f);
        text.transform.position -= new Vector3(0, 0.2f);
        text.SetActive(false);
    }
}
