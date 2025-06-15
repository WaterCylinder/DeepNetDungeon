using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    public static GameObject _prefab;
    public static GameObject prefab => _prefab ? _prefab : _prefab = AssetManager.Load<GameObject>("EnemyGenerator");
    public string enemyName;
    public GameObject enemy;
    public Room room;
    [Tooltip("是否立即生成")]
    public bool generateImmediately = false;
    [SerializeField]
    private GameObject enemyPrefab{
        get{
            if(enemy == null){
                enemy = GameManager.instance.game.GetEntity(enemyName);
            }
            return enemy;
        }
    }

    void Awake(){
        if(room == null)
            room = GameManager.instance.game.room;
    }

    void Start(){
        if(generateImmediately)
            Generate();
    }

    public Enemy Generate(){
        Vector2 pos = transform.position;
        Debug.Log($"生成Enemy: {(enemy == null ? enemyName : enemy.name)}");
        Destroy(gameObject);
        try{
            Enemy enm = room == null ? 
                (Enemy)GameManager.instance.EntityCreate(enemyPrefab, pos)
                : room.CreatEnemy(enemyName, pos);
            return enm;
        }catch(System.Exception e){
            Debug.LogWarning(e.Message);
            return null;
        }
    }

}
