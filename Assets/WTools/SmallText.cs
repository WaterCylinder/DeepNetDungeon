using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SmallText : MonoBehaviour
{
    public TextMeshProUGUI text;
    public string context{
        get{
            return text.text;
        }
        set{
            text.text = value;
        }
    }
}
