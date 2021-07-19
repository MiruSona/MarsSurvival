using UnityEngine;
using System.Collections;

public class Weapon : SingleTon<Weapon> {

    //참조
    private GameDAO.WeaponData weapon_data;
    private SpriteRenderer sprite_renderer;

    //초기화
    void Awake() {
        weapon_data = GameDAO.instance.player_data.weapon_data;
        sprite_renderer = GetComponent<SpriteRenderer>();

        gameObject.SetActive(false);
    }

    //스프라이트 변화
    void OnEnable()
    {
        UpdateShape();
    }

    //모양 변경
    public void UpdateShape()
    {
        //스프라이트 변경
        sprite_renderer.sprite = weapon_data.sprites[weapon_data.step];
    }
}
