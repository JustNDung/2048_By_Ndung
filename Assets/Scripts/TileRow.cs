
using UnityEngine;

public class TileRow : MonoBehaviour
{
    public TileCell[] cells {get; private set;}
    public void Awake() {
        cells = GetComponentsInChildren<TileCell>();
        // Lấy tất cả components TileCell của TileRow
    }

}
