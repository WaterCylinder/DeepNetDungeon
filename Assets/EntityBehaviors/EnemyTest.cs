using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class EnemyTest : Enemy
{   
    public string displayText = "..";
    protected override void EntityStart(){
        displayText = "Enemy Test!";
    }

    protected override void EntityUpdate(){
        base.EntityUpdate();
        Attack();
    }

    protected override void EntityAttack(){
        base.EntityAttack();
        Bullet.CreateBullet(this, transform.position, Vector2.left, 8f);
    }

    void OnGUI(){
        GUILayout.Label(displayText, new GUIStyle(GUI.skin.label) { fontSize = 24 });
    }

}
