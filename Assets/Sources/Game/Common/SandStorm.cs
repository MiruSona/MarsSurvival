using UnityEngine;
using System.Collections;

public class SandStorm : MonoBehaviour {

    //참조
    private SystemDAO.StormData storm_data;
    private SpriteRenderer sprite_renderer;
    private GameObject storm_particle;
    private ParticleSystem storm_particle_system;

    //알파값 조절
    private Color sprite_color = Color.white;
    private float alpha_max = 0.9f;

    //초기화
    void Start () {
        storm_data = SystemDAO.instance.storm_data;
        sprite_renderer = GetComponent<SpriteRenderer>();
        storm_particle = transform.FindChild("StormParticle").gameObject;
        storm_particle_system = storm_particle.GetComponent<ParticleSystem>();

        sprite_color.a = 0;
    }
	
    //모레폭풍 불게
	void Update () {

        if (storm_data.enable)
        {
            //파티클 true(시작)
            if (!storm_particle.activeSelf)
                storm_particle.SetActive(true);            

            //모레폭풍 투명도
            if (sprite_renderer.color.a < alpha_max)
            {
                sprite_color.a += 0.01f;
                sprite_renderer.color = sprite_color;
            }

            if(sprite_renderer.color.a >= alpha_max)
            {
                sprite_color.a = alpha_max;
                sprite_renderer.color = sprite_color;
            }
        }
        else
        {
            //파티클 멈춤
            if (storm_particle.activeSelf)
                storm_particle_system.Stop();

            //모레폭풍 투명도
            if (sprite_renderer.color.a > 0f)
            {
                sprite_color.a -= 0.01f;
                sprite_renderer.color = sprite_color;
            }

            if (sprite_renderer.color.a < 0f)
            {
                sprite_color.a = 0f;
                sprite_renderer.color = sprite_color;
            }
        }

        //모레폭풍 멈췄으면 false
        if (storm_particle_system.isStopped)
            storm_particle.SetActive(false);
	}
}
