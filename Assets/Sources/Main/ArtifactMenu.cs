using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ArtifactMenu : MonoBehaviour {

    //참조
    private SystemDAO.FileManager file_manager;

    //데이터
    private GameDAO.ArtifactData artifact_data;
    private GameDAO.Artifact[] artifact_list = null;

    //설명
    private Image cliked_img;
    private Text description;

    //버튼들
    private Button[] artifact_btn = new Button[Define.artifact_num];
    private Image[] artifact_img = new Image[Define.artifact_num];
    private Image[] artifact_border = new Image[Define.artifact_num];

    //초기화
    void Awake () {
        //참조
        file_manager = SystemDAO.instance.file_manager;

        //데이터
        artifact_data = GameDAO.instance.artifact_data;

        //리스트
        artifact_list = artifact_data.artifcat;

        //설명
        cliked_img = transform.FindChild("Panel").transform.FindChild("ClickedImg").FindChild("Image").GetComponent<Image>();
        description = transform.FindChild("Panel").transform.FindChild("Description").FindChild("Text").GetComponent<Text>();

        //버튼
        for (int i = 0; i < Define.artifact_num; i++)
        {
            artifact_btn[i] = transform.FindChild("Panel").transform.FindChild("Buttons").FindChild("Artifact" + i).GetComponent<Button>();
            artifact_img[i] = transform.FindChild("Panel").transform.FindChild("Buttons").FindChild("Artifact" + i).FindChild("Image").GetComponent<Image>();
            artifact_border[i] = transform.FindChild("Panel").transform.FindChild("Borders").FindChild("Border" + i).GetComponent<Image>();
        }
    }

    //켜질때마다 체크
    void OnEnable()
    {
        //버튼 활성화 & 비활성화
        for(int i = 0; i < Define.artifact_num; i++)
        {
            if (i < artifact_data.get_artifact_list.ToArray().Length)
            {
                artifact_btn[i].interactable = true;
                artifact_img[i].gameObject.SetActive(true);
                artifact_border[i].gameObject.SetActive(artifact_list[i].activate);
            }
            else
            {
                artifact_btn[i].interactable = false;
                artifact_img[i].gameObject.SetActive(false);
                artifact_border[i].gameObject.SetActive(false);
            }
        }
    }

    //Resume
    public void ResumeBtn()
    {
        //아티펙트 세이브
        file_manager.SaveArtifact();
        //false로
        gameObject.SetActive(false);
    }

    //유물 활성화 & 비활성화
    public void ArtifactBtn(int _artifact_num)
    {
        //엑티브 여부 바꿈(true -> false / false -> true)
        if (artifact_list[_artifact_num].activate)
            artifact_list[_artifact_num].activate = false;
        else
            artifact_list[_artifact_num].activate = true;
        
        //테두리 갱신        
        artifact_border[_artifact_num].gameObject.SetActive(artifact_list[_artifact_num].activate);

        //설명 갱신
        cliked_img.gameObject.SetActive(true);
        description.gameObject.SetActive(true);
        if (artifact_list[_artifact_num].activate)
        {
            cliked_img.color = Color.white;
            cliked_img.sprite = artifact_data.sprites[_artifact_num];
            description.text = artifact_list[_artifact_num].description;
        }
        else
        {
            cliked_img.color = Color.clear;
            description.text = "";
        }
            
    }
}
