using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    void FixedUpdate()
    {   
        if(GameManager.G_player == null){
            return;
        }
        Vector2 pos = GameManager.G_playerPos;
        pos = Vector2.Lerp(transform.position, pos, 0.1f);
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
    }
}
