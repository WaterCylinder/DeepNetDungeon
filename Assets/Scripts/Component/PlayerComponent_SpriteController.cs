using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerComponent_SpriteController : MonoBehaviour
{
    public Player player;
    public Transform body;
    public Transform eye;
    public float eyeOffset = 2f;
    public Transform arrow;
    void Start(){
        if(!player){
            player = Game.instance.player;
        }
    }
    void Update(){
        eye.localPosition = player.attackToward.normalized * eyeOffset;
        if(player.attackToward != Vector2.zero){
            arrow.gameObject.SetActive(true);
            arrow.localEulerAngles = new Vector3(0, 0, Tool.RightAngle(player.attackToward));
        }else{
            arrow.gameObject.SetActive(false);
        }
    }
}
