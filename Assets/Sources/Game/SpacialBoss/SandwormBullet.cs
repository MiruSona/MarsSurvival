using UnityEngine;
using System.Collections;

public class SandwormBullet : SPBossBullet {

    //랜덤값
    private const float y_pos_min = -0.2f;
    private const float y_pos_max = 0.2f;
    private const float gravity_min = 0f;
    private const float gravity_max = 2f;

    public override void Shoot(Vector2 _direction)
    {
        //랜덤값
        float gravity_random = Random.Range(gravity_min, gravity_max);
        float y_pos_random = Random.Range(y_pos_min, y_pos_max);

        //초기화 위치로 이동
        transform.localPosition = reload_pos;
        gameObject.SetActive(true);
        _direction.y += y_pos_random;
        rb2d.gravityScale = gravity_random;
        rb2d.AddForce(_direction * speed);
    }
}
