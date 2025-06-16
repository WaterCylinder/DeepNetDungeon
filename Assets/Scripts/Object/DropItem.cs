using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    private static GameObject _prefab;
    public static GameObject prefab => _prefab ? _prefab : _prefab = AssetManager.Load<GameObject>("DropItem");
    public Item item;
    public void Init(){
        name = item.itemName;
        GetComponent<SpriteRenderer>().sprite = item.GetSprite();
    } 
    public void Pick(Entity entity, Bag bag){
        Debug.Log($"{entity.name}拾取了{item.itemName}");
        item.Pick(entity);
        bag.AddItem(item.itemName);
        GameManager.instance.ItemRemove(this);
        Destroy(gameObject);
    }
    void OnCollisionEnter2D(Collision2D other){
        if(other?.gameObject.GetComponent<Entity>() && other?.gameObject.GetComponent<Bag>()){
            Pick(other.gameObject.GetComponent<Entity>(), other.gameObject.GetComponent<Bag>());
        }
    }
}
