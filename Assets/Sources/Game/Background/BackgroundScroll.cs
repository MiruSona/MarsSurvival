using UnityEngine;
using System.Collections;

public class BackgroundScroll : MonoBehaviour {

    //참조
    private Transform player_transform;
    private Material material;

    //위치
    public enum Position
    {
        Forward,
        Backward
    }
    public Position position = Position.Forward;

    //움직일 값
    private Vector2 offset = Vector2.zero;
    
    // 초기화
    void Start () {
        //참조
        player_transform = GameObject.FindGameObjectWithTag("Player").transform;
        material = GetComponent<Renderer>().material;
    }

    // 이동(스크롤)
    void Update() {

        switch (position)
        {
            case Position.Forward:
                offset.x = player_transform.localPosition.x / 100f;
                break;

            case Position.Backward:
                offset.x = player_transform.localPosition.x / 200f;
                break;
        }

        material.mainTextureOffset = offset;
    }
}
