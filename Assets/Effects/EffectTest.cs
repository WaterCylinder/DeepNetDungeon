using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTest : Effect
{
    public EffectTest(Entity entity) : base(entity){}

    protected override void Init(ref EffectTag tags, ref float processTime, ref int defaultLayer, ref float layerTime, ref int maxLayer)
    {   
        defaultLayer = 3;
    }
    protected override void OnProcess(){
        Debug.Log(ToString());
    }
}
