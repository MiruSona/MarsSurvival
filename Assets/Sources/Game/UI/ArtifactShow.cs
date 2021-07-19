using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ArtifactShow : MonoBehaviour {

    private GameDAO.ArtifactData artifact_data;
    private RawImage clickedArtifact; //클릭한거 이미지
    private Text clickedText;
    private Button[] artifacts = new Button[15]; //아티팩트 버튼 정리

    //초기화
    void Awake () {
        artifact_data = GameDAO.instance.artifact_data;

        clickedArtifact = transform.FindChild("Highlighted").FindChild("RawImage").GetComponent<RawImage>();
        clickedText = GameObject.Find("FlavorTextPanel").GetComponentInChildren<Text>();
        clickedText.text = "";

        for (int i = 0; i < artifacts.Length; i++)
        {
            artifacts[i] = transform.FindChild("ArtifactBtns").FindChild("" + i).GetComponent<Button>();
            if (!artifact_data.artifacts[i])
            {
                artifacts[i].interactable = false; // 버튼 비활성화 함
                artifacts[i].GetComponentInChildren<RawImage>().color = new Color(1, 1, 1, 0);
            }
        }
    }

    //활성화 된거 표시
    void OnEnable() {
        for (int i = 0; i < artifacts.Length; i++) {
            if (artifact_data.artifacts[i]) {
                artifacts[i].interactable = true; // 버튼 비활성화 함
                artifacts[i].GetComponentInChildren<RawImage>().color = Color.white;
            }
        }
    }

    //클릭한 버튼 받아와서 실행시키는 함수
    public void ButtonClicked(GameObject clicked)
    { 
        //버튼 클릭했을때 하이라이트 이미지 텍스처 <= 버튼 차일드 RawImage 텍스처로 대체
        clickedArtifact.color = new Color(1, 1, 1, 1);
        clickedArtifact.texture = artifacts[int.Parse(clicked.name)].GetComponentInChildren<RawImage>().texture;

        if (clicked.name == "0") {
            clickedText.text = "Foot Print\nPlayer Move Speed +10%";
        } else if (clicked.name == "1") {
            clickedText.text = "Fossil\nTool Speed +5%";
        } else if (clicked.name == "2") {
            clickedText.text = "Spaceship Fragment\nTool Speed +5%";
        } else if (clicked.name == "3") {
            clickedText.text = "Skull\nPlayer Health +15%";
        } else if (clicked.name == "4") {
            clickedText.text = "Diary\nPlayer Health +15%";
        } else if (clicked.name == "5") {
            clickedText.text = "GraveStone\nPlayer Atk +10%";
        } else if (clicked.name == "6") {
            clickedText.text = "Shiny Stone\nField of View +30%";
        } else if (clicked.name == "7") {
            clickedText.text = "Slime Jelly\nPlayer Move Speed +10%";
        } else if (clicked.name == "8") {
            clickedText.text = "Vase\nFood Bio Using -1";
        } else if (clicked.name == "9") {
            clickedText.text = "Amber\nWall Durabillity +20%";
        } else if (clicked.name == "10") {
            clickedText.text = "Battery\nRegenerator Durabillity +20%";
        } else if (clicked.name == "11") {
            clickedText.text = "Candy\nFood Health Regen +10";
        } else if (clicked.name == "12") {
            clickedText.text = "Lipstick\nTurret Health +20%";
        } else if (clicked.name == "13") {
            clickedText.text = "Leaf\nTurret Atk +10%";
        } else if (clicked.name == "14") {
            clickedText.text = "Sandstorm Sample\nSandstorm Fov +30%";
        }
    }

    //일시정지 풀기
    public void Resume()
    {
        gameObject.SetActive(false);
    }
}
