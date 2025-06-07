using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLoader : MonoBehaviour
{   
    public string dbname;
    public string spriteName;
    public SpriteRenderer sp;
    void Start(){
        if(sp == null){
            sp = GetComponent<SpriteRenderer>();
            if(sp == null){
                sp = gameObject.AddComponent<SpriteRenderer>();
            }
        }
    }
    void Update(){
        Sprite s = SpriteManager.GetSprite(dbname, spriteName);
        if(s == null){
            sp.sprite = s;
            Destroy(this);
        }
    }

}
