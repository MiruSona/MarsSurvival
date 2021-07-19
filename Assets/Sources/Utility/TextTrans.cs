using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class TextTrans : SingleTon<TextTrans> {
    private SystemDAO.FileManager file_manager;

    public string languageCode;
    private Font fontset;

    #region MainMenu
    private GameObject noSaveDataText;
    private GameObject crystalWarningText;
    private GameObject newGameWarningText;
    private GameObject saveExitText;

    private Image flagIcon;
    #endregion


    #region GameMain
    private GameObject die_menu;
    private GameObject health_warning;
    private GameObject build_menu;
    private GameObject build_armor;
    private GameObject upgrade_menu;
    private GameObject review_menu;
    private GameObject give_crystal_menu;
    private GameObject pause_menu;
    private GameObject crystal_panel;
    private GameObject metalbio_panel;
    private GameObject result_panel;
    private GameObject artifact_panel;
    private GameObject buff_panel;

    private GameObject poisonUI;
    #endregion



    // Use this for initialization
    void Awake() { //OnLevelWasLoaded보다 Start가 느림...
        DontDestroyOnLoad(gameObject);
        //Debug.Log(Application.systemLanguage);

        file_manager = SystemDAO.instance.file_manager;

        //언어 코드 로드
        file_manager.LoadLanguageCode();
        Debug.Log(file_manager.languageCode);

        languageCode = "EN";
        Debug.Log(languageCode);

        if (file_manager.languageCode == null) { //세이브 없을 때 시작시 언어 변경함

                languageCode = "EN";
                file_manager.SaveLanguageCode("EN");
                fontset = Resources.Load<Font>("Etc/Font/slkscr");
            
            Debug.Log(languageCode);
        }

        //===========================================================

            fontset = Resources.Load<Font>("Etc/Font/slkscr");
        
        //===========================================================
        //세이브 파일 있을 경우에는 언어 코드 가져옴

        Debug.Log(fontset);
    }


    // Update is called once per frame
    void Update() {

    }

    void OnLevelWasLoaded(int scene) {
        if (scene == 1) {
            languageCode = "EN";
            //깃발 아이콘 변경
            GameObject.Find("LanguageIcon").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/Main/UI/Icon_" + languageCode);

            //예 & 아니오 버튼 변경
            //if (languageCode == "KR") {
            //    foreach (Button btn in FindObjectsOfType<Button>()) {
            //        if (btn.name == "YesBtn")
            //            btn.GetComponentInChildren<Text>().text = "네";
            //        else if (btn.name == "NoBtn")
            //            btn.GetComponentInChildren<Text>().text = "아니오";
            //    }
            //} else if (languageCode == "JP") {
            //    foreach (Button btn in FindObjectsOfType<Button>()) {
            //        if (btn.name == "YesBtn")
            //            btn.GetComponentInChildren<Text>().text = "はい";
            //        else if (btn.name == "NoBtn")
            //            btn.GetComponentInChildren<Text>().text = "いいえ";
            //    }
            //} else {
            //    foreach (Button btn in FindObjectsOfType<Button>()) {
            //        if (btn.name == "YesBtn")
            //            btn.GetComponentInChildren<Text>().text = "Yes";
            //        else if (btn.name == "NoBtn")
            //            btn.GetComponentInChildren<Text>().text = "No";
            //    }
            //}

            //타이틀 & 버튼 스프라이트 교체
            GameObject.Find("Title").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/Main/Background/Main_Title_" + languageCode);
            GameObject.Find("NewGameBtn").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/Main/UI/New Game_" + languageCode);
            GameObject.Find("Continue").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/Main/UI/Continue_" + languageCode);
            GameObject.Find("ArtifactBtn").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/Main/UI/Artifact_" + languageCode);

            //텍스트 불러오기
            noSaveDataText = GameObject.Find("MainMenu").transform.FindChild("NoSaveText").gameObject;
            crystalWarningText = GameObject.Find("MainMenu").transform.FindChild("Warning_text").gameObject;
            newGameWarningText = GameObject.Find("MainMenu").transform.FindChild("Warning").FindChild("Panel").FindChild("WarningText").gameObject;
            saveExitText = GameObject.Find("MainMenu").transform.FindChild("SaveExit").FindChild("Panel").FindChild("SaveExitText").gameObject;

            //텍스트 폰트 변경
            noSaveDataText.GetComponent<Text>().font = fontset;
            crystalWarningText.GetComponent<Text>().font = fontset;
            newGameWarningText.GetComponent<Text>().font = fontset;
            saveExitText.GetComponent<Text>().font = fontset;
            GameObject.Find("MainMenu").transform.FindChild("HardModMenu").FindChild("Panel").FindChild("HardText").GetComponent<Text>().font = fontset;
            GameObject.Find("MainMenu").transform.FindChild("SaveExit").FindChild("Panel").FindChild("YesBtn").FindChild("Text").GetComponent<Text>().font = fontset;
            GameObject.Find("MainMenu").transform.FindChild("SaveExit").FindChild("Panel").FindChild("NoBtn").FindChild("Text").GetComponent<Text>().font = fontset;
            GameObject.Find("MainMenu").transform.FindChild("Warning").FindChild("Panel").FindChild("YesBtn").FindChild("Text").GetComponent<Text>().font = fontset;
            GameObject.Find("MainMenu").transform.FindChild("Warning").FindChild("Panel").FindChild("NoBtn").FindChild("Text").GetComponent<Text>().font = fontset;
            GameObject.Find("MainMenu").transform.FindChild("HardModMenu").FindChild("Panel").FindChild("YesBtn").FindChild("Text").GetComponent<Text>().font = fontset;
            GameObject.Find("MainMenu").transform.FindChild("HardModMenu").FindChild("Panel").FindChild("NoBtn").FindChild("Text").GetComponent<Text>().font = fontset;


            //텍스트 변경
            if (languageCode == "KR") {
                noSaveDataText.GetComponent<Text>().text = "세이브 파일이 없습니다.";
                crystalWarningText.GetComponent<Text>().text = "광고 크리스탈은\n매일 밤 12시에 초기화 됩니다";
                newGameWarningText.GetComponent<Text>().text = "정말로 새 게임을 하시겠습니까?";
                saveExitText.GetComponent<Text>().text = "저장 및 게임을 종료하시겠습니까?";
                GameObject.Find("MainMenu").transform.FindChild("Warning").FindChild("Panel").FindChild("YesBtn").FindChild("Text").GetComponent<Text>().text = "네";
                GameObject.Find("MainMenu").transform.FindChild("Warning").FindChild("Panel").FindChild("NoBtn").FindChild("Text").GetComponent<Text>().text = "아니오";
                GameObject.Find("MainMenu").transform.FindChild("SaveExit").FindChild("Panel").FindChild("YesBtn").FindChild("Text").GetComponent<Text>().text = "네";
                GameObject.Find("MainMenu").transform.FindChild("SaveExit").FindChild("Panel").FindChild("NoBtn").FindChild("Text").GetComponent<Text>().text = "아니오";
                GameObject.Find("MainMenu").transform.FindChild("HardModMenu").FindChild("Panel").FindChild("YesBtn").FindChild("Text").GetComponent<Text>().text = "네";
                GameObject.Find("MainMenu").transform.FindChild("HardModMenu").FindChild("Panel").FindChild("NoBtn").FindChild("Text").GetComponent<Text>().text = "아니오";
                GameObject.Find("MainMenu").transform.FindChild("HardModMenu").FindChild("Panel").FindChild("HardText").GetComponent<Text>().text = "하드모드로 플레이 하시겠습니까?\n(난이도가 상승합니다)";
            } else if (languageCode == "JP") {
                noSaveDataText.GetComponent<Text>().text = "セーブファイルがありません";
                crystalWarningText.GetComponent<Text>().text = "無料のクリスタルは\n毎日子正に初期化されます";
                newGameWarningText.GetComponent<Text>().text = "本当に新しいゲームを\nスタートしますか？\nセーブファイルが削除されます。";
                saveExitText.GetComponent<Text>().text = "ゲームを終了しますか？";
                GameObject.Find("MainMenu").transform.FindChild("Warning").FindChild("Panel").FindChild("YesBtn").FindChild("Text").GetComponent<Text>().text = "はい";
                GameObject.Find("MainMenu").transform.FindChild("Warning").FindChild("Panel").FindChild("NoBtn").FindChild("Text").GetComponent<Text>().text = "いいえ";
                GameObject.Find("MainMenu").transform.FindChild("SaveExit").FindChild("Panel").FindChild("YesBtn").FindChild("Text").GetComponent<Text>().text = "はい";
                GameObject.Find("MainMenu").transform.FindChild("SaveExit").FindChild("Panel").FindChild("NoBtn").FindChild("Text").GetComponent<Text>().text = "いいえ";
                GameObject.Find("MainMenu").transform.FindChild("HardModMenu").FindChild("Panel").FindChild("YesBtn").FindChild("Text").GetComponent<Text>().text = "はい";
                GameObject.Find("MainMenu").transform.FindChild("HardModMenu").FindChild("Panel").FindChild("NoBtn").FindChild("Text").GetComponent<Text>().text = "いいえ";
                GameObject.Find("MainMenu").transform.FindChild("HardModMenu").FindChild("Panel").FindChild("HardText").GetComponent<Text>().text = "ハードモードでプレイしますか？\n(難易度が上昇します)";
            } else {
                noSaveDataText.GetComponent<Text>().text = "There is no saved data";
                crystalWarningText.GetComponent<Text>().text = "You can get an another free crystal\nafter every 12:00AM";
                newGameWarningText.GetComponent<Text>().text = "Really want to play a new game?\nsave file will be erased.";
                saveExitText.GetComponent<Text>().text = "Really want to exit game?";
                GameObject.Find("MainMenu").transform.FindChild("Warning").FindChild("Panel").FindChild("YesBtn").FindChild("Text").GetComponent<Text>().text = "Yes";
                GameObject.Find("MainMenu").transform.FindChild("Warning").FindChild("Panel").FindChild("NoBtn").FindChild("Text").GetComponent<Text>().text = "No";
                GameObject.Find("MainMenu").transform.FindChild("SaveExit").FindChild("Panel").FindChild("YesBtn").FindChild("Text").GetComponent<Text>().text = "Yes";
                GameObject.Find("MainMenu").transform.FindChild("SaveExit").FindChild("Panel").FindChild("NoBtn").FindChild("Text").GetComponent<Text>().text = "No";
                GameObject.Find("MainMenu").transform.FindChild("HardModMenu").FindChild("Panel").FindChild("YesBtn").FindChild("Text").GetComponent<Text>().text = "Yes";
                GameObject.Find("MainMenu").transform.FindChild("HardModMenu").FindChild("Panel").FindChild("NoBtn").FindChild("Text").GetComponent<Text>().text = "No";
                GameObject.Find("MainMenu").transform.FindChild("HardModMenu").FindChild("Panel").FindChild("HardText").GetComponent<Text>().text = "Really want to play hard mode?\n(It is harder than normal)";
            }
            
        } else if (scene == 2) {
            languageCode = "EN";

            GameObject.Find("TalkCanvas").transform.FindChild("TalkText").GetComponent<Text>().font = fontset;

            die_menu = GameObject.Find("GameManager").transform.FindChild("UIManager").FindChild("UICanvas").FindChild("DieMenu").gameObject;
            health_warning = GameObject.Find("GameManager").transform.FindChild("UIManager").FindChild("UICanvas").FindChild("HealthWarnig").gameObject;
            build_menu = GameObject.Find("GameManager").transform.FindChild("UIManager").FindChild("UICanvas").FindChild("BuildMenu").gameObject;
            build_armor = GameObject.Find("GameManager").transform.FindChild("UIManager").FindChild("UICanvas").FindChild("BuildMenu").FindChild("Armor_Pop").gameObject;
            upgrade_menu = GameObject.Find("GameManager").transform.FindChild("UIManager").FindChild("UICanvas").FindChild("UpgradeMenu").gameObject;
            review_menu = GameObject.Find("GameManager").transform.FindChild("UIManager").FindChild("UICanvas").FindChild("ReviewMenu").gameObject;
            give_crystal_menu = GameObject.Find("GameManager").transform.FindChild("UIManager").FindChild("UICanvas").FindChild("GiveMenu").gameObject;
            pause_menu = GameObject.Find("GameManager").transform.FindChild("UIManager").FindChild("UICanvas").FindChild("PauseMenu").gameObject;
            crystal_panel = GameObject.Find("GameManager").transform.FindChild("UIManager").FindChild("UICanvas").FindChild("CrystalPanel").gameObject;
            metalbio_panel = GameObject.Find("GameManager").transform.FindChild("UIManager").FindChild("UICanvas").FindChild("MetalBioPanel").gameObject;
            result_panel = GameObject.Find("GameManager").transform.FindChild("UIManager").FindChild("UICanvas").FindChild("ResultPanel").FindChild("ResultPanel").gameObject;
            artifact_panel = GameObject.Find("GameManager").transform.FindChild("UIManager").FindChild("UICanvas").FindChild("GetArtifactPanel").gameObject;
            buff_panel = GameObject.Find("GameManager").transform.FindChild("UIManager").FindChild("UICanvas").FindChild("BuffMenu").gameObject;

            #region 일시정지 메뉴
            SpriteState sprState = new SpriteState();

            pause_menu.transform.FindChild("Sound").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/Game/UI/Btn_" + languageCode + "/Pause_Sound_On");
            sprState.pressedSprite = Resources.Load<Sprite>("Sprite/Game/UI/Btn_" + languageCode + "/Pause_Sound_Off");
            sprState.highlightedSprite = Resources.Load<Sprite>("Sprite/Game/UI/Btn_" + languageCode + "/Pause_Sound_On");
            sprState.disabledSprite = Resources.Load<Sprite>("Sprite/Game/UI/Btn_" + languageCode + "/Pause_Sound_Off");

            pause_menu.transform.FindChild("Sound").GetComponent<Button>().spriteState = sprState;
             
            pause_menu.transform.FindChild("Help").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/Game/UI/Btn_" + languageCode + "/Pause_Help");
            sprState.pressedSprite = Resources.Load<Sprite>("Sprite/Game/UI/Btn_" + languageCode + "/Pause_Help_Pressed");
            sprState.highlightedSprite = Resources.Load<Sprite>("Sprite/Game/UI/Btn_" + languageCode + "/Pause_Help");
            sprState.disabledSprite = Resources.Load<Sprite>("Sprite/Game/UI/Btn_" + languageCode + "/Pause_Help_Pressed");

            pause_menu.transform.FindChild("Help").GetComponent<Button>().spriteState = sprState;

            pause_menu.transform.FindChild("MainMenu").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/Game/UI/Btn_" + languageCode + "/Pause_MainMenu");
            sprState.pressedSprite = Resources.Load<Sprite>("Sprite/Game/UI/Btn_" + languageCode + "/Pause_MainMenu_Pressed");
            sprState.highlightedSprite = Resources.Load<Sprite>("Sprite/Game/UI/Btn_" + languageCode + "/Pause_MainMenu");
            sprState.disabledSprite = Resources.Load<Sprite>("Sprite/Game/UI/Btn_" + languageCode + "/Pause_MainMenu_Pressed");

            pause_menu.transform.FindChild("MainMenu").GetComponent<Button>().spriteState = sprState;

            pause_menu.transform.FindChild("Exit").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/Game/UI/Btn_" + languageCode + "/Pause_Exit");
            sprState.pressedSprite = Resources.Load<Sprite>("Sprite/Game/UI/Btn_" + languageCode + "/Pause_Exit_Pressed");
            sprState.highlightedSprite = Resources.Load<Sprite>("Sprite/Game/UI/Btn_" + languageCode + "/Pause_Exit");
            sprState.disabledSprite = Resources.Load<Sprite>("Sprite/Game/UI/Btn_" + languageCode + "/Pause_Exit_Pressed");

            pause_menu.transform.FindChild("Exit").GetComponent<Button>().spriteState = sprState;

            pause_menu.transform.FindChild("CommercialOffBtn").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/Game/UI/commercial_off_Btn_"+languageCode);
            sprState.pressedSprite = Resources.Load<Sprite>("Sprite/Game/UI/commercial_off_Btn_" + languageCode);
            sprState.highlightedSprite = Resources.Load<Sprite>("Sprite / Game / UI / commercial_off_Btn_"+languageCode);
            sprState.disabledSprite = Resources.Load<Sprite>("Sprite/Game/UI/commercial_off_Btn_Purchased_" + languageCode);

            pause_menu.transform.FindChild("CommercialOffBtn").GetComponent<Button>().spriteState = sprState;

            //if (languageCode != "KR") {
            //    pause_menu.transform.FindChild("CommercialOffBtn").gameObject.SetActive(false);
            //}
            #endregion

            #region 업그레이드 메뉴
            upgrade_menu.transform.FindChild("Weapon").FindChild("Title").GetComponent<Text>().font = fontset;
            upgrade_menu.transform.FindChild("Tool").FindChild("Title").GetComponent<Text>().font = fontset;
            upgrade_menu.transform.FindChild("Turret").FindChild("Title").GetComponent<Text>().font = fontset;
            upgrade_menu.transform.FindChild("Shield").FindChild("Title").GetComponent<Text>().font = fontset;

            if (languageCode == "KR") {
                upgrade_menu.transform.FindChild("Weapon").FindChild("Title").GetComponent<Text>().text = "무기 업그레이드";
                upgrade_menu.transform.FindChild("Tool").FindChild("Title").GetComponent<Text>().text = "도구 업그레이드";
                upgrade_menu.transform.FindChild("Turret").FindChild("Title").GetComponent<Text>().text = "터렛 업그레이드";
                upgrade_menu.transform.FindChild("Shield").FindChild("Title").GetComponent<Text>().text = "실드 업그레이드";
            } else if (languageCode == "JP") {
                upgrade_menu.transform.FindChild("Weapon").FindChild("Title").GetComponent<Text>().text = "武器 アップグレード";
                upgrade_menu.transform.FindChild("Tool").FindChild("Title").GetComponent<Text>().text = "道具 アップグレード";
                upgrade_menu.transform.FindChild("Turret").FindChild("Title").GetComponent<Text>().text = "ターレット アップグレード";
                upgrade_menu.transform.FindChild("Shield").FindChild("Title").GetComponent<Text>().text = "シールド アップグレード";
            } else {
                upgrade_menu.transform.FindChild("Weapon").FindChild("Title").GetComponent<Text>().text = "Weapon Upgrade";
                upgrade_menu.transform.FindChild("Tool").FindChild("Title").GetComponent<Text>().text = "Tool Upgrade";
                upgrade_menu.transform.FindChild("Turret").FindChild("Title").GetComponent<Text>().text = "Turret Upgrade";
                upgrade_menu.transform.FindChild("Shield").FindChild("Title").GetComponent<Text>().text = "Shild Upgrade";
            }
            #endregion

            #region 빌드 메뉴
            build_menu.transform.FindChild("Turret").FindChild("Title").GetComponent<Text>().font = fontset;
            build_menu.transform.FindChild("Turret").FindChild("Limit").FindChild("Text").GetComponent<Text>().font = fontset;
            build_menu.transform.FindChild("Shield").FindChild("Title").GetComponent<Text>().font = fontset;
            build_menu.transform.FindChild("Shield").FindChild("Limit").FindChild("Text").GetComponent<Text>().font = fontset;
            build_menu.transform.FindChild("Mine").FindChild("Title").GetComponent<Text>().font = fontset;
            build_menu.transform.FindChild("Mine").FindChild("Limit").FindChild("Text").GetComponent<Text>().font = fontset;
            build_menu.transform.FindChild("Meal").FindChild("Title").GetComponent<Text>().font = fontset;
            build_menu.transform.FindChild("Warp").FindChild("Title").GetComponent<Text>().font = fontset;
            build_menu.transform.FindChild("Armor").FindChild("Title").GetComponent<Text>().font = fontset;
            build_menu.transform.FindChild("Armor_Pop").FindChild("Panel").FindChild("Armor+1").FindChild("Title").GetComponent<Text>().font = fontset;
            build_menu.transform.FindChild("Armor_Pop").FindChild("Panel").FindChild("Armor+2").FindChild("Title").GetComponent<Text>().font = fontset;

            if (languageCode == "KR") {
                build_menu.transform.FindChild("Turret").FindChild("Title").GetComponent<Text>().text = "터렛";
                build_menu.transform.FindChild("Turret").FindChild("Limit").FindChild("Text").GetComponent<Text>().text = "더이상 건설하실 수 없습니다.";
                build_menu.transform.FindChild("Shield").FindChild("Title").GetComponent<Text>().text = "실드";
                build_menu.transform.FindChild("Shield").FindChild("Limit").FindChild("Text").GetComponent<Text>().text = "더이상 건설하실 수 없습니다.";
                build_menu.transform.FindChild("Mine").FindChild("Title").GetComponent<Text>().text = "지뢰";
                build_menu.transform.FindChild("Mine").FindChild("Limit").FindChild("Text").GetComponent<Text>().text = "더이상 건설하실 수 없습니다.";
                build_menu.transform.FindChild("Meal").FindChild("Title").GetComponent<Text>().text = "고구마";
                build_menu.transform.FindChild("Warp").FindChild("Title").GetComponent<Text>().text = "워프";
                build_menu.transform.FindChild("Armor").FindChild("Title").GetComponent<Text>().text = "아머";
                build_menu.transform.FindChild("Armor_Pop").FindChild("Panel").FindChild("Armor+1").FindChild("Title").GetComponent<Text>().text = "아머+1";
                build_menu.transform.FindChild("Armor_Pop").FindChild("Panel").FindChild("Armor+2").FindChild("Title").GetComponent<Text>().text = "아머+2";
                build_menu.transform.FindChild("Armor_Pop").FindChild("Panel").FindChild("Armor+2").FindChild("PriceText").GetComponent<Text>().text = "￦3,000";
            } else if (languageCode == "JP") {
                build_menu.transform.FindChild("Turret").FindChild("Title").GetComponent<Text>().text = "ターレット";
                build_menu.transform.FindChild("Turret").FindChild("Limit").FindChild("Text").GetComponent<Text>().text = "リミットに到達";
                build_menu.transform.FindChild("Shield").FindChild("Title").GetComponent<Text>().text = "シールド";
                build_menu.transform.FindChild("Shield").FindChild("Limit").FindChild("Text").GetComponent<Text>().text = "リミットに到達";
                build_menu.transform.FindChild("Mine").FindChild("Title").GetComponent<Text>().text = "地雷";
                build_menu.transform.FindChild("Mine").FindChild("Limit").FindChild("Text").GetComponent<Text>().text = "リミットに到達";
                build_menu.transform.FindChild("Meal").FindChild("Title").GetComponent<Text>().text = "いも";
                build_menu.transform.FindChild("Warp").FindChild("Title").GetComponent<Text>().text = "ワープ";
                build_menu.transform.FindChild("Armor").FindChild("Title").GetComponent<Text>().text = "アーマー";
                build_menu.transform.FindChild("Armor_Pop").FindChild("Panel").FindChild("Armor+1").FindChild("Title").GetComponent<Text>().text = "アーマー+1";
                build_menu.transform.FindChild("Armor_Pop").FindChild("Panel").FindChild("Armor+2").FindChild("Title").GetComponent<Text>().text = "アーマー+2";
                build_menu.transform.FindChild("Armor_Pop").FindChild("Panel").FindChild("Armor+2").FindChild("PriceText").GetComponent<Text>().text = "¥300";
            } else {
                build_menu.transform.FindChild("Turret").FindChild("Title").GetComponent<Text>().text = "Turret";
                build_menu.transform.FindChild("Turret").FindChild("Limit").FindChild("Text").GetComponent<Text>().text = "Build Limit";
                build_menu.transform.FindChild("Shield").FindChild("Title").GetComponent<Text>().text = "Shild";
                build_menu.transform.FindChild("Shield").FindChild("Limit").FindChild("Text").GetComponent<Text>().text = "Build Limit";
                build_menu.transform.FindChild("Mine").FindChild("Title").GetComponent<Text>().text = "Mine";
                build_menu.transform.FindChild("Mine").FindChild("Limit").FindChild("Text").GetComponent<Text>().text = "Limit";
                build_menu.transform.FindChild("Meal").FindChild("Title").GetComponent<Text>().text = "Sweet Potato";
                build_menu.transform.FindChild("Warp").FindChild("Title").GetComponent<Text>().text = "Warp";
                build_menu.transform.FindChild("Armor").FindChild("Title").GetComponent<Text>().text = "Armor";
                build_menu.transform.FindChild("Armor_Pop").FindChild("Panel").FindChild("Armor+1").FindChild("Title").GetComponent<Text>().text = "Armor+1";
                build_menu.transform.FindChild("Armor_Pop").FindChild("Panel").FindChild("Armor+2").FindChild("Title").GetComponent<Text>().text = "Armor+2";
                build_menu.transform.FindChild("Armor_Pop").FindChild("Panel").FindChild("Armor+2").FindChild("PriceText").GetComponent<Text>().text = "$2.99";
            }
            #endregion

            #region 결과창
            result_panel.transform.FindChild("ResultText").GetComponent<Text>().font = fontset;
            result_panel.transform.FindChild("GoMenuBTN").FindChild("Text").GetComponent<Text>().font = fontset;
            result_panel.transform.FindChild("PlayTimeText").GetComponent<Text>().font = fontset;
            if (languageCode == "KR") {
                result_panel.transform.FindChild("ResultText").GetComponent<Text>().text = "결 과";
                result_panel.transform.FindChild("GoMenuBTN").FindChild("Text").GetComponent<Text>().text = "메인 메뉴";
                result_panel.transform.FindChild("PlayTimeText").GetComponent<Text>().text = "플레이 시간 :";
            } else if (languageCode == "JP") {
                result_panel.transform.FindChild("ResultText").GetComponent<Text>().text = "結　果";
                result_panel.transform.FindChild("GoMenuBTN").FindChild("Text").GetComponent<Text>().text = "Main Menu";
                result_panel.transform.FindChild("PlayTimeText").GetComponent<Text>().text = "プレイタイム:";
            } else {
                result_panel.transform.FindChild("ResultText").GetComponent<Text>().text = "Result";
                result_panel.transform.FindChild("GoMenuBTN").FindChild("Text").GetComponent<Text>().text = "Main Menu";
                result_panel.transform.FindChild("PlayTimeText").GetComponent<Text>().text = "Play Time :";
            }
            #endregion

            #region 아티팩트 패널
            artifact_panel.transform.FindChild("Text").GetComponent<Text>().font = fontset;
            if (languageCode == "KR") {
                artifact_panel.transform.FindChild("Text").GetComponent<Text>().text = "아티팩트를 획득했다!";
            } else if (languageCode == "JP") {
                artifact_panel.transform.FindChild("Text").GetComponent<Text>().text = "アーチフェクトを獲得した！";
            } else {
                artifact_panel.transform.FindChild("Text").GetComponent<Text>().text = "You get an artifact!";
            }
            #endregion

            #region 사망 메뉴
            die_menu.transform.FindChild("DiePanel").FindChild("Text").GetComponent<Text>().font = fontset;
            die_menu.transform.FindChild("DiePanel").FindChild("Continue").FindChild("Text").GetComponent<Text>().font = fontset;
            die_menu.transform.FindChild("DiePanel").FindChild("Die").FindChild("Text").GetComponent<Text>().font = fontset;
            die_menu.transform.FindChild("DieWarnig").FindChild("Text").GetComponent<Text>().font = fontset;
            die_menu.transform.FindChild("DieWarnig").FindChild("Yes").FindChild("Text").GetComponent<Text>().font = fontset;
            die_menu.transform.FindChild("DieWarnig").FindChild("No").FindChild("Text").GetComponent<Text>().font = fontset;

            if (languageCode == "KR") {
                die_menu.transform.FindChild("DiePanel").FindChild("Text").GetComponent<Text>().text = "이어서 하시겠습니까?";
                die_menu.transform.FindChild("DiePanel").FindChild("Continue").FindChild("Text").GetComponent<Text>().text = "이어하기";
                die_menu.transform.FindChild("DiePanel").FindChild("Die").FindChild("Text").GetComponent<Text>().text = "죽음";
                die_menu.transform.FindChild("DieWarnig").FindChild("Text").GetComponent<Text>().text = "크리스탈을 제외한\n모든 저장 사항이 사라집니다.\n계속 진행하시겠습니까?";
                die_menu.transform.FindChild("DieWarning").FindChild("Yes").FindChild("Text").GetComponent<Text>().text = "예";
                die_menu.transform.FindChild("DieWarning").FindChild("No").FindChild("Text").GetComponent<Text>().text = "아니오";
            } else if (languageCode == "JP") {
                die_menu.transform.FindChild("DiePanel").FindChild("Text").GetComponent<Text>().text = "Continue?";
                die_menu.transform.FindChild("DiePanel").FindChild("Continue").FindChild("Text").GetComponent<Text>().text = "はい";
                die_menu.transform.FindChild("DiePanel").FindChild("Die").FindChild("Text").GetComponent<Text>().text = "いいえ";
                die_menu.transform.FindChild("DieWarnig").FindChild("Text").GetComponent<Text>().text = "クリスタルを除外した\n全てのセーブファイルがなくなります\n続きますか？";
                die_menu.transform.FindChild("DieWarnig").FindChild("Yes").FindChild("Text").GetComponent<Text>().text = "はい";
                die_menu.transform.FindChild("DieWarnig").FindChild("No").FindChild("Text").GetComponent<Text>().text = "いいえ";
            } else {
                die_menu.transform.FindChild("DiePanel").FindChild("Text").GetComponent<Text>().text = "Continue?";
                die_menu.transform.FindChild("DiePanel").FindChild("Continue").FindChild("Text").GetComponent<Text>().text = "Yes";
                die_menu.transform.FindChild("DiePanel").FindChild("Die").FindChild("Text").GetComponent<Text>().text = "Die";
                die_menu.transform.FindChild("DieWarnig").FindChild("Text").GetComponent<Text>().text = "All your progress\nwill be erased except crystals,\ncontinue?";
                die_menu.transform.FindChild("DieWarnig").FindChild("Yes").FindChild("Text").GetComponent<Text>().text = "Yes";
                die_menu.transform.FindChild("DieWarnig").FindChild("No").FindChild("Text").GetComponent<Text>().text = "No";
            }
            #endregion

            #region 체력 경고
            health_warning.transform.FindChild("Panel").FindChild("Text").GetComponent<Text>().font = fontset;
            health_warning.transform.FindChild("Panel").FindChild("Continue").FindChild("Text").GetComponent<Text>().font = fontset;

            if (languageCode == "KR") {
                health_warning.transform.FindChild("Panel").FindChild("Text").GetComponent<Text>().text = "두 번째 이어하기 부터는\n크리스탈을 소모합니다.";
                health_warning.transform.FindChild("Panel").FindChild("Continue").FindChild("Text").GetComponent<Text>().text = "확인";
            } else if (languageCode == "JP") {
                health_warning.transform.FindChild("Panel").FindChild("Text").GetComponent<Text>().text = "二つ目の続きからは\nクリスタルを消耗します";
                health_warning.transform.FindChild("Panel").FindChild("Continue").FindChild("Text").GetComponent<Text>().text = "Ok";
            } else {
                health_warning.transform.FindChild("Panel").FindChild("Text").GetComponent<Text>().text = "You need crystal after\nyour first revive";
                health_warning.transform.FindChild("Panel").FindChild("Continue").FindChild("Text").GetComponent<Text>().text = "Ok";
            }
            #endregion

            #region 크리스탈 패널
            crystal_panel.transform.FindChild("Layout").FindChild("S").FindChild("Text").GetComponent<Text>().font = fontset;
            crystal_panel.transform.FindChild("Layout").FindChild("M").FindChild("Text").GetComponent<Text>().font = fontset;
            crystal_panel.transform.FindChild("Layout").FindChild("L").FindChild("Text").GetComponent<Text>().font = fontset;

            if (languageCode == "KR") {
                crystal_panel.transform.FindChild("Layout").FindChild("S").FindChild("Text").GetComponent<Text>().text = "크리스탈 3개";
                crystal_panel.transform.FindChild("Layout").FindChild("M").FindChild("Text").GetComponent<Text>().text = "크리스탈 18개";
                crystal_panel.transform.FindChild("Layout").FindChild("L").FindChild("Text").GetComponent<Text>().text = "크리스탈 40개";
            } else if (languageCode == "JP") {
                crystal_panel.transform.FindChild("Layout").FindChild("S").FindChild("Text").GetComponent<Text>().text = "クリスタル 3個";
                crystal_panel.transform.FindChild("Layout").FindChild("M").FindChild("Text").GetComponent<Text>().text = "クリスタル 18個";
                crystal_panel.transform.FindChild("Layout").FindChild("L").FindChild("Text").GetComponent<Text>().text = "クリスタル 40個";
            } else {
                crystal_panel.transform.FindChild("Layout").FindChild("S").FindChild("Text").GetComponent<Text>().text = "Crystal x 3";
                crystal_panel.transform.FindChild("Layout").FindChild("M").FindChild("Text").GetComponent<Text>().text = "Crystal x 18";
                crystal_panel.transform.FindChild("Layout").FindChild("L").FindChild("Text").GetComponent<Text>().text = "Crystal x 40";
            }
            #endregion

            #region 자원 패널
            metalbio_panel.transform.FindChild("Layout").FindChild("S").FindChild("Text").GetComponent<Text>().font = fontset;
            metalbio_panel.transform.FindChild("Layout").FindChild("M").FindChild("Text").GetComponent<Text>().font = fontset;
            metalbio_panel.transform.FindChild("Layout").FindChild("L").FindChild("Text").GetComponent<Text>().font = fontset;
            metalbio_panel.transform.FindChild("Layout").FindChild("S").FindChild("Button").FindChild("Text").GetComponent<Text>().font = fontset;
            metalbio_panel.transform.FindChild("Layout").FindChild("M").FindChild("Button").FindChild("Text").GetComponent<Text>().font = fontset;
            metalbio_panel.transform.FindChild("Layout").FindChild("L").FindChild("Button").FindChild("Text").GetComponent<Text>().font = fontset;
            metalbio_panel.transform.FindChild("PurchaseCheck").FindChild("Panel").FindChild("YesBtn").FindChild("Text").GetComponent<Text>().font = fontset;
            metalbio_panel.transform.FindChild("PurchaseCheck").FindChild("Panel").FindChild("NoBtn").FindChild("Text").GetComponent<Text>().font = fontset;

            if (languageCode == "KR") {
                metalbio_panel.transform.FindChild("Layout").FindChild("S").FindChild("Text").GetComponent<Text>().text = "20개 구매";
                metalbio_panel.transform.FindChild("Layout").FindChild("M").FindChild("Text").GetComponent<Text>().text = "120개 구매";
                metalbio_panel.transform.FindChild("Layout").FindChild("L").FindChild("Text").GetComponent<Text>().text = "250개 구매";
                metalbio_panel.transform.FindChild("Layout").FindChild("S").FindChild("Button").FindChild("Text").GetComponent<Text>().text = "1 크리스탈";
                metalbio_panel.transform.FindChild("Layout").FindChild("M").FindChild("Button").FindChild("Text").GetComponent<Text>().text = "5 크리스탈";
                metalbio_panel.transform.FindChild("Layout").FindChild("L").FindChild("Button").FindChild("Text").GetComponent<Text>().text = "10 크리스탈";
                metalbio_panel.transform.FindChild("PurchaseCheck").FindChild("Panel").FindChild("YesBtn").FindChild("Text").GetComponent<Text>().text = "구매";
                metalbio_panel.transform.FindChild("PurchaseCheck").FindChild("Panel").FindChild("NoBtn").FindChild("Text").GetComponent<Text>().text = "취소";
            } else if (languageCode == "JP") {
                metalbio_panel.transform.FindChild("Layout").FindChild("S").FindChild("Text").GetComponent<Text>().text = "20個倶入";
                metalbio_panel.transform.FindChild("Layout").FindChild("M").FindChild("Text").GetComponent<Text>().text = "120個倶入";
                metalbio_panel.transform.FindChild("Layout").FindChild("L").FindChild("Text").GetComponent<Text>().text = "250個倶入";
                metalbio_panel.transform.FindChild("Layout").FindChild("S").FindChild("Button").FindChild("Text").GetComponent<Text>().text = "クリスタル 1個";
                metalbio_panel.transform.FindChild("Layout").FindChild("M").FindChild("Button").FindChild("Text").GetComponent<Text>().text = "クリスタル 5個";
                metalbio_panel.transform.FindChild("Layout").FindChild("L").FindChild("Button").FindChild("Text").GetComponent<Text>().text = "クリスタル 10個";
                metalbio_panel.transform.FindChild("PurchaseCheck").FindChild("Panel").FindChild("YesBtn").FindChild("Text").GetComponent<Text>().text = "倶入";
                metalbio_panel.transform.FindChild("PurchaseCheck").FindChild("Panel").FindChild("NoBtn").FindChild("Text").GetComponent<Text>().text = "キャンセル";
            } else {
                metalbio_panel.transform.FindChild("Layout").FindChild("S").FindChild("Text").GetComponent<Text>().text = "Buy 20";
                metalbio_panel.transform.FindChild("Layout").FindChild("M").FindChild("Text").GetComponent<Text>().text = "Buy 120";
                metalbio_panel.transform.FindChild("Layout").FindChild("L").FindChild("Text").GetComponent<Text>().text = "Buy 250";
                metalbio_panel.transform.FindChild("Layout").FindChild("S").FindChild("Button").FindChild("Text").GetComponent<Text>().text = "Crystal x 1";
                metalbio_panel.transform.FindChild("Layout").FindChild("M").FindChild("Button").FindChild("Text").GetComponent<Text>().text = "Crystal x 5";
                metalbio_panel.transform.FindChild("Layout").FindChild("L").FindChild("Button").FindChild("Text").GetComponent<Text>().text = "Crystal x 10";
                metalbio_panel.transform.FindChild("PurchaseCheck").FindChild("Panel").FindChild("YesBtn").FindChild("Text").GetComponent<Text>().text = "Buy";
                metalbio_panel.transform.FindChild("PurchaseCheck").FindChild("Panel").FindChild("NoBtn").FindChild("Text").GetComponent<Text>().text = "Cancel";
            }
            #endregion

            #region 리뷰 패널
            review_menu.transform.FindChild("ReviewPanel").FindChild("Text").GetComponent<Text>().font = fontset;
            review_menu.transform.FindChild("ReviewPanel").FindChild("Yes").FindChild("Text").GetComponent<Text>().font = fontset;
            review_menu.transform.FindChild("ReviewPanel").FindChild("No").FindChild("Text").GetComponent<Text>().font = fontset;
            review_menu.transform.FindChild("ReviewPanel").FindChild("Later").FindChild("Text").GetComponent<Text>().font = fontset;

            if (languageCode == "KR") {
                review_menu.transform.FindChild("ReviewPanel").FindChild("Text").GetComponent<Text>().text = "게임이 재미있으신가요?\n리뷰를 부탁드려요!";
                review_menu.transform.FindChild("ReviewPanel").FindChild("Yes").FindChild("Text").GetComponent<Text>().text = "좋아요";
                review_menu.transform.FindChild("ReviewPanel").FindChild("No").FindChild("Text").GetComponent<Text>().text = "싫어요";
                review_menu.transform.FindChild("ReviewPanel").FindChild("Later").FindChild("Text").GetComponent<Text>().text = "나중에";
            } else if (languageCode == "JP") {
                review_menu.transform.FindChild("ReviewPanel").FindChild("Text").GetComponent<Text>().text = "ゲームを楽しんでいますか？\n評価をお願いします！";
                review_menu.transform.FindChild("ReviewPanel").FindChild("Yes").FindChild("Text").GetComponent<Text>().text = "いいよ";
                review_menu.transform.FindChild("ReviewPanel").FindChild("No").FindChild("Text").GetComponent<Text>().text = "いやです";
                review_menu.transform.FindChild("ReviewPanel").FindChild("Later").FindChild("Text").GetComponent<Text>().text = "後で";
            } else {
                review_menu.transform.FindChild("ReviewPanel").FindChild("Text").GetComponent<Text>().text = "Are you having fun?\nRate this game please!";
                review_menu.transform.FindChild("ReviewPanel").FindChild("Yes").FindChild("Text").GetComponent<Text>().text = "Ok";
                review_menu.transform.FindChild("ReviewPanel").FindChild("No").FindChild("Text").GetComponent<Text>().text = "No";
                review_menu.transform.FindChild("ReviewPanel").FindChild("Later").FindChild("Text").GetComponent<Text>().text = "Later";
            }
            #endregion

            #region 크리스탈 지급 메뉴
            give_crystal_menu.transform.FindChild("GivePanel").FindChild("Text").GetComponent<Text>().font = fontset;
            give_crystal_menu.transform.FindChild("GivePanel").FindChild("SeeAD").FindChild("Text").GetComponent<Text>().font = fontset;

            if (languageCode == "KR") {
                give_crystal_menu.transform.FindChild("GivePanel").FindChild("Text").GetComponent<Text>().text = "높아지는 난이도를 대비하기 위해\n동영상을 보시면 크리스탈 2개를\n지급해드립니다.";
                give_crystal_menu.transform.FindChild("GivePanel").FindChild("SeeAD").FindChild("Text").GetComponent<Text>().text = "크리스탈 받기";
            } else if (languageCode == "JP") {
                give_crystal_menu.transform.FindChild("GivePanel").FindChild("Text").GetComponent<Text>().text = "上昇する難易度へ対するために\n広告を見るとクリスタル２個を\n支給します";
                give_crystal_menu.transform.FindChild("GivePanel").FindChild("SeeAD").FindChild("Text").GetComponent<Text>().text = "クリスタルをもらう";
            } else {
                give_crystal_menu.transform.FindChild("GivePanel").FindChild("Text").GetComponent<Text>().text = "Because of the game becoming harder,\nwe'll give you 2 crystals if you see AD";
                give_crystal_menu.transform.FindChild("GivePanel").FindChild("SeeAD").FindChild("Text").GetComponent<Text>().text = "Ok";
            }
            #endregion

            #region 버프 메뉴
            buff_panel.transform.FindChild("Panel").FindChild("Yes").FindChild("Text").GetComponent<Text>().font = fontset;
            buff_panel.transform.FindChild("Panel").FindChild("No").FindChild("Text").GetComponent<Text>().font = fontset;

            if (languageCode == "KR") {
                buff_panel.transform.FindChild("Panel").FindChild("Yes").FindChild("Text").GetComponent<Text>().text = "네";
                buff_panel.transform.FindChild("Panel").FindChild("No").FindChild("Text").GetComponent<Text>().text = "아니오";
            } else if (languageCode == "JP") {
                buff_panel.transform.FindChild("Panel").FindChild("Yes").FindChild("Text").GetComponent<Text>().text = "はい";
                buff_panel.transform.FindChild("Panel").FindChild("No").FindChild("Text").GetComponent<Text>().text = "いいえ";
            } else {
                buff_panel.transform.FindChild("Panel").FindChild("Yes").FindChild("Text").GetComponent<Text>().text = "Yes";
                buff_panel.transform.FindChild("Panel").FindChild("No").FindChild("Text").GetComponent<Text>().text = "No";
            }
            #endregion

            GameObject.Find("TalkCanvas").transform.FindChild("TalkTest").GetComponent<Text>().font = fontset;
            poisonUI = GameObject.Find("Player").transform.FindChild("PoisonUI").FindChild("PoisonedCanvas").gameObject;

            poisonUI.transform.FindChild("Title").GetComponent<Text>().font = fontset;
            poisonUI.transform.FindChild("Timer").GetComponent<Text>().font = fontset;
            if (languageCode == "KR") {
                poisonUI.transform.FindChild("Title").GetComponent<Text>().text = "중독됨";
                poisonUI.transform.FindChild("Timer").GetComponent<Text>().text = "해독중";
            } else if (languageCode == "JP") {
                poisonUI.transform.FindChild("Title").GetComponent<Text>().text = "中毒";
                poisonUI.transform.FindChild("Timer").GetComponent<Text>().text = "解毒中";
            } else {
                poisonUI.transform.FindChild("Title").GetComponent<Text>().text = "Poisoned";
                poisonUI.transform.FindChild("Timer").GetComponent<Text>().text = "Recovering";
            }

            poisonUI.transform.FindChild("RecoveryBtn").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/Game/UI/Btn_" + languageCode + "/Recover_Btn");
            sprState.pressedSprite = Resources.Load<Sprite>("Sprite/Game/UI/Btn_" + languageCode + "/Recover_Btn_Pressed");
            sprState.highlightedSprite = Resources.Load<Sprite>("Sprite/Game/UI/Btn_" + languageCode + "/Recover_Btn");
            sprState.disabledSprite = Resources.Load<Sprite>("Sprite/Game/UI/Btn_" + languageCode + "/Recover_Btn_Pressed");

            poisonUI.transform.FindChild("RecoveryBtn").GetComponent<Button>().spriteState = sprState;
        }
    }

    public void TranslationBtn() {
        GameObject.Find("MainMenu").transform.FindChild("LanguageChange").gameObject.SetActive(true);
    }

    public void Translation(int code) { //코드에 따라 languageCode 변경 0 = 한글 1 = 일본어 2 = 영어
        switch (code) {
            case 0:
                languageCode = "KR";
                break;
            case 1:
                languageCode = "JP";
                break;
            case 2:
                languageCode = "EN";
                break;
        }
        //저장
        file_manager.languageCode = languageCode;
        file_manager.SaveLanguageCode(languageCode);
        //씬 리로드
        Destroy(gameObject);
        Destroy(GameObject.Find("DataManager"));
        SceneManager.LoadScene(1);
    }
}
