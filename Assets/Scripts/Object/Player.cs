using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{   
    public Vector2 attackToward;
    public Bag bag;
    protected override void EntityAwake(){
        Game.instance.player = this;
        entityTags.Add(EntityTag.Player);
        bag = GetComponent<Bag>();
    }
    
    protected override void EntityUpdate(){
        if(GameManager.CanOpera){
            toward = InputManager.MoveToward();
            attackToward = InputManager.AttackToward();
            if(attackToward != Vector2.zero){
                Attack();
            }
        }
    }

    protected override void EntityAttack(){
        Bullet but = Bullet.CreateBullet(this, transform.position, attackToward, 8f);
        but.bulletTags.Add(BulletTag.Player);
        but.BulletInit(new Damage(10, 0.2f, 1.5f), this, 2, BulletTag.Player,
            EnterEvent:(but,e)=>{
                Debug.Log(e.name);
            }
        );
    }
}
