using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MapDebuger : MonoBehaviour
{
    public Map map;
    public RectTransform layout;
    public GameObject start;
    public GameObject boss;
    public GameObject end;
    public GameObject normal;
    public float roomSize = 32;
    public bool isDisplay = false;
    void Update(){
        if(map.state == MapState.MapCreatorDone && !isDisplay){
            for(int i = 0; i < map.width; i++){
                for(int j = 0; j < map.height; j++){
                    if(map.map[i, j] == MapFlag.Empty)continue;
                    GameObject obj = null;
                    switch(map.map[i, j]){
                        case MapFlag.Start:
                            obj = Instantiate(start);
                        break;
                        case MapFlag.Boss:
                            obj = Instantiate(boss);
                        break;
                        case MapFlag.End:
                            obj = Instantiate(end);
                        break;
                        case MapFlag.Normal:
                            obj = Instantiate(normal);
                        break;
                    }
                    if(obj == null)break;
                    obj.transform.SetParent(layout.transform);
                    obj.transform.localPosition = new Vector3(-j * roomSize, i * roomSize, 0);
                }
            }
            layout.localPosition = new Vector3(map.width * roomSize / 2, -map.height * roomSize / 2, 0);
            Destroy(start);
            Destroy(boss);
            Destroy(end);
            Destroy(normal);
            isDisplay = true;
        }
    }
}
