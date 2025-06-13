
using UnityEngine;

public class Block : MonoBehaviour
{
    private static GameObject _wallPrefab;
    public static GameObject wallPrefab => _wallPrefab ? _wallPrefab : AssetManager.Load<GameObject>("Wall");
}
