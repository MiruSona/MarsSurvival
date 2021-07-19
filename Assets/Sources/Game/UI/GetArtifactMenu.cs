using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GetArtifactMenu : MonoBehaviour {

    //참조
    private Image artifact_img;
    private GameDAO.ArtifactData artifact_data;
    private SystemDAO.FileManager file_manager;

    //초기화
	void Awake () {
        artifact_img = transform.FindChild("Artifact").GetComponent<Image>();
        artifact_data = GameDAO.instance.artifact_data;
        file_manager = SystemDAO.instance.file_manager;
    }

    //enable 시 획득한 아티펙트 표시
    void OnEnable()
    {
        if(file_manager.clear_num <= Define.artifact_num)
            artifact_img.sprite = artifact_data.sprites[file_manager.clear_num - 1];
        else
            gameObject.SetActive(false);
    }

    //아무데나 누르면 꺼짐
    public void Resume()
    {
        gameObject.SetActive(false);
    }
}
