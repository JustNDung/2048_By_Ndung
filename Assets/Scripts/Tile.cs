using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public TileState state {get; private set;}
    public TileCell cell {get; private set;}
    public int number {get; private set;}
    public bool locked {get; set;}
    private Image background;
    private TextMeshProUGUI text;
    public void Awake() {
        background = GetComponent<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void SetState(TileState state, int number) {
        this.state = state;
        this.number = number;

        background.color = state.backgroundColor;
        text.color = state.textColor;
        text.text = number.ToString();
        Debug.Log(background.color);
    
    }

    public void Spawn(TileCell cell) {
        if (this.cell != null) {
            this.cell.tile = null;
        }
        this.cell = cell;
        this.cell.tile = this;
        transform.position = cell.transform.position;
    }

    public void MoveTo(TileCell cell) {
        if (this.cell != null) {
            this.cell.tile = null;
        }
        // Nếu ô hiện tại this.cell đang liên kết với 1 tile thì cho cái tile = null để ngắt liên kết
        this.cell = cell;
        // Cập nhật ô hiện tại bằng ô đích đến
        this.cell.tile = this;
        // Cập nhật tile của ô đích chính bằng tile của ô hiện tại

        StartCoroutine(Animate(cell.transform.position, false));
    } 

    public void Merge(TileCell cell) {
        if (this.cell != null) {
            this.cell.tile = null;
        }

        this.cell = null;
        cell.tile.locked = true;

        StartCoroutine(Animate(cell.transform.position, true));
    }

    private IEnumerator Animate(Vector3 to, bool merging) {
        float elapsed = 0f;
        float duration = 0.1f;

        Vector3 from = transform.position;
        while (elapsed < duration) {
            transform.position = Vector3.Lerp(from, to, elapsed / duration);
            // Nội suy tuyến tính giữa 2 điểm from và to dựa trên giá trị t
            // Mỗi khung hình vị trí của đối tượng được cập nhật để di chuyển mượt mà từ vị trí from đến to
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = to;
        if (merging) {
            Destroy(gameObject);
        }
    }
}
