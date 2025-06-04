using TMPro;
using UnityEngine;

public class SavePanel : MonoBehaviour
{
    public int index;

    public TextMeshProUGUI saveInfo;

    void Start(){
        SaveData data = SaveManager.GetSave(index);
        if(data.isEmpty){
            saveInfo.text = Global.ReadUIText("save_empty");
        }else{
            saveInfo.text = string.Format(Global.ReadUIText("save_info"), data.name, data.createTime);
        }
    }
}
