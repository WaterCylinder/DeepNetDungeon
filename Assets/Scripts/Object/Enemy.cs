using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{   
    public Room room;
    protected override void EntityAwake(){
        base.EntityAwake();
        entityTags.Add(EntityTag.Enemy);
    }
    protected override void EntityOnDead(){
        base.EntityOnDead();
        room.enemies.Remove(this);
    }
}
