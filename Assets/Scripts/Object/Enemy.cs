using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    protected override void EntityAwake(){
        base.EntityAwake();
        entityTags.Add(EntityTag.Enemy);
    }
}
