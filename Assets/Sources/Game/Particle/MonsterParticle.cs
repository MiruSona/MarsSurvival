using UnityEngine;
using System.Collections;
using System;

public class MonsterParticle : ParticleCommon {

    //참조
    private AudioSource audio_source;

    //disable로 죽었는지 체크
    [System.NonSerialized]
    public bool die_with_disable = false;

    //상속
    //초기화
    protected override void ChildInit()
    {
        audio_source = GetComponent<AudioSource>();

        //disable로 죽은게 아니면 사운드 플레이
        if (!die_with_disable)
            if (!audio_source.isPlaying)
                audio_source.Play();
    }
}
