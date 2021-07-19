using UnityEngine;
using System.Collections;

public class Boundary : MonoBehaviour {
    
    public enum Direction
    {
        Left,
        Right
    }
    public Direction direction = Direction.Left;

    void OnCollisionEnter2D(Collision2D _col)
    {
        if (_col.transform.CompareTag("Player"))
        {
            float move_x = 0;
            float move_y = _col.transform.localPosition.y;
            float move_z = _col.transform.localPosition.z;

            if (direction == Direction.Left)
                move_x = Define.real_point.max;
            else
                move_x = Define.real_point.min;

            _col.transform.localPosition = new Vector3(move_x, move_y, move_z);
        }
    }
}
