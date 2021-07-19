using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Artifacts : MonoBehaviour {

    private GameDAO.ArtifactData artifact_data;
    private GameDAO.PlayerData player_data;
    private GameDAO.TurretData turret_data;
    private GameObject[] addedArtifacts = new GameObject[2];
    private RawImage[] addedImages = new RawImage[2];
    private Button[] artifacts = new Button[15]; //아티팩트 버튼 정리
    #region 아티펙트 번호 / 이름
    // 0 번 = 발자국 - 걷다
    // 1 번 = 화석 - 돌
    // 2 번 = 우주선 파편 - 나무
    // 3 번 = 해골 - 돌
    // 4 번 = 다이어리 - 나무
    // 5 번 = 석판 - 돌
    // 6 번 = 빛나는 돌 - 돌
    // 7 번 = 슬라임 조각 - 슬라임
    // 8 번 = 꽃병 - 돌
    // 9 번 = 호박 - 나무
    //10 번 = 배터리 - 걷다
    //11 번 = 알사탕 - 걷다
    //12 번 = 립스틱 - 걷다
    //13 번 = 나뭇잎 - 나무
    //14 번 = 모래폭풍 샘플 - 모래 
    #endregion

    private RawImage clickedArtifact; //클릭한거 이미지 (왼쪽 상단 설명 쓰이는 부분 이미지임)
    private Text clickedText;
    private GameObject selectedObj;

    //초기화
    void Awake() {
        artifact_data = GameDAO.instance.artifact_data;
        player_data = GameDAO.instance.player_data;
        turret_data = GameDAO.instance.turret_data;

        clickedText = GameObject.Find("FlavorTextPanel").GetComponentInChildren<Text>();
        clickedText.text = "";

        //버튼 초기화
        for (int i = 0; i < addedImages.Length; i++)
            addedImages[i] = transform.FindChild("Selected" + i).FindChild("RawImage").GetComponent<RawImage>();

        clickedArtifact = transform.FindChild("Highlighted").FindChild("RawImage").GetComponent<RawImage>();

        for (int i = 0; i < artifacts.Length; i++) {
            artifacts[i] = transform.FindChild("" + i).GetComponent<Button>();
            if (!artifact_data.artifacts[i]) {
                artifacts[i].interactable = false; // 버튼 비활성화 함
                artifacts[i].GetComponentInChildren<RawImage>().color = new Color(1, 1, 1, 0);
            }
        }
    }

    public void ButtonClicked(GameObject clicked) { //클릭한 버튼 받아와서 실행시키는 함수
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
        //Add버튼 눌렀을 때 이미지는 clickedArtifact 넘기면 되고, 여기서 clicked 된 유물이 무엇인지 저장해 둠
        selectedObj = clicked;
    }

    public void AddButtonClicked() {
        if (selectedObj != null) { //무언가 선택되었을때
            if (addedArtifacts[0] == null && selectedObj != addedArtifacts[1]) { //첫번째 칸에 아무것도 없을때 && 다른 칸과 겹치지 않을때
                addedImages[0].texture = clickedArtifact.texture;
                addedImages[0].color = new Color(1, 1, 1, 1);
                addedArtifacts[0] = selectedObj;
                //selectedObj.GetComponent<Button>().interactable = false; // 버튼 비활성화 함
                //selectedObj.GetComponentInChildren<RawImage>().color = new Color(1, 1, 1, 0);
            } else if (addedArtifacts[1] == null && selectedObj != addedArtifacts[0]) { //두번째 칸에 아무것도 없을때 && 다른 칸과 겹치지 않을때
                addedImages[1].texture = clickedArtifact.texture;
                addedImages[1].color = new Color(1, 1, 1, 1);
                addedArtifacts[1] = selectedObj;  //이미지 보내고 선택된거 저장한다음(버튼째로)
                //selectedObj.GetComponent<Button>().interactable = false; // 버튼 비활성화 함
                //selectedObj.GetComponentInChildren<RawImage>().color = new Color(1, 1, 1, 0);
            } else { //둘 다 이미 들어가있을때
                     //아무것도 안함
            }
            //selectedObj = null;
        }
    }

    public void DeselectArtifact(GameObject clicked) {
        // 선택된 아티팩트 제거
        if (clicked == GameObject.Find("Selected0") && addedArtifacts[0] != null) { //클릭한게 윗칸이고, 윗칸에 무언가 있을때
            addedImages[0].color = new Color(1, 1, 1, 0); //이미지 안보이게 처리해두고
            //addedArtifacts[0].GetComponent<Button>().interactable = true; //해당 버튼 다시 활성화 함
            //addedArtifacts[0].GetComponentInChildren<RawImage>().color = new Color(1, 1, 1, 1);
            addedArtifacts[0] = null; // 선택된 것 비움
        } else if (clicked == GameObject.Find("Selected1") && addedArtifacts[1] != null) { //클릭한게 아랫칸이고, 아랫칸에 무언가 있을때
            addedImages[1].color = new Color(1, 1, 1, 0);
            //addedArtifacts[1].GetComponent<Button>().interactable = true; //해당 버튼 다시 활성화 함
            //addedArtifacts[1].GetComponentInChildren<RawImage>().color = new Color(1, 1, 1, 1);
            addedArtifacts[1] = null;
        } else {

        }
    }

    // 아티팩트 능력치 처리 함수, 파라미터 나오는대로 설정
    void ArtifactActivate(string[] _artifact) {
        for (int i = 0; i < 2; i++) {
            if (_artifact[i] == "0") {
                player_data.max_speed = 84f; // 이속 20% 증가
            } else if (_artifact[i] == "1") {
                // 금속 제련기 생성 시간 10% 감소
            } else if (_artifact[i] == "2") {
                // 식량 빌드시 소모 바이오 자원 감소
            } else if (_artifact[i] == "3") {
                player_data.hp *= 1.15f;
                player_data.max_hp *= 1.15f;// 플레이어 기본 체력 +15%
            } else if (_artifact[i] == "4") {
                player_data.hp *= 1.15f;
                player_data.max_hp *= 1.15f;// 플레이어 기본 체력 +15%
            } else if (_artifact[i] == "5") {
                for(int step = 0; step < Define.weapon_step_max; step++)
                    for(int level = 0; level < Define.weapon_level_max; level++)
                        player_data.weapon_data.atk[step, level] *= 1.1f;
            } else if (_artifact[i] == "6") {
                // 모래폭풍, 밤 시야 증가
            } else if (_artifact[i] == "7") {
                // 바이오 생성기 생성 시간 10% 감소
            } else if (_artifact[i] == "8") {
                player_data.tool_timer.term *= 0.95f;
                // 도구 속도 +5% 증가
            } else if (_artifact[i] == "9") {
                // Hp 회복기 1 쿨타임 감소
            } else if (_artifact[i] == "10") {
                turret_data.hp_max[0] *= 1.2f;
                turret_data.hp_max[1] *= 1.2f;
                turret_data.hp_max[2] *= 1.2f;
                // Hp 회복기 2, 벽, 포탑 체력 증가
            } else if (_artifact[i] == "11") {
                // 식량 회복량 증가
            } else if (_artifact[i] == "12") {
                // 
            } else if (_artifact[i] == "13") {
                turret_data.atk[0] *= 1.1f;
                turret_data.atk[1] *= 1.1f;
                turret_data.atk[2] *= 1.1f;
                // 포탑 공격력 증가
            } else if (_artifact[i] == "14") {
                // 모래폭풍 시야 증가
            } else {
                //pass
            }
        }
    }

    public void GameStartButton() {
        string[] selecting = new string[2];
        if (selecting[0] != null) {
            selecting[0] = addedArtifacts[0].ToString();
        }else if (selecting[1] != null) {
            selecting[1] = addedArtifacts[1].ToString();
        }
        ArtifactActivate(selecting);
        SceneManager.LoadScene("GameMain");
    }
}
