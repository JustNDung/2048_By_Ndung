
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
    public GameManager gameManager;
    public Tile tilePrefab;
    public TileState[] tileStates;
    private TileGrid grid;
    private List<Tile> tiles;
    private bool waiting;
    public void Awake() {
        grid = GetComponentInChildren<TileGrid>();
        tiles = new List<Tile>(16);
    }
    
    public void ClearBoard() {
        foreach (var cell in grid.cells) {
            cell.tile = null;
        }
        foreach (var tile in tiles) {
            Destroy(tile.gameObject);
        }
        tiles.Clear();
    }
    public void CreateTile() {
        Tile tile = Instantiate(tilePrefab, grid.transform);
        tile.SetState(tileStates[0], 2);
        tile.Spawn(grid.GetRandomEmptyCell());
        tiles.Add(tile);
    }
    public void Update() {
        if (!waiting) {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
                MoveTiles(Vector2Int.up, 0, 1, 1, 1);
            } else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
                MoveTiles(Vector2Int.down, 0, 1, grid.height - 2, -1);
            } else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
                MoveTiles(Vector2Int.left, 1, 1, 0, 1);
            } else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
                MoveTiles(Vector2Int.right, grid.width - 2, -1, 0, 1);
            }
        }
    }
    private void MoveTiles(Vector2Int direction, int startX, int incrementX, int startY, int incrementY) {
        bool changed = false; 
        for (int x = startX; x >= 0 && x < grid.width; x += incrementX) {
            for (int y = startY; y >= 0 && y < grid.height; y += incrementY) {
                TileCell cell = grid.GetCell(x, y);

                if (cell.occupied) {
                    changed |= MoveTile(cell.tile, direction);
                    // Ghi nhận rằng đã có ít nhất 1 ô di chuyển bằng toán tử or gán
                }
            }
        }

        if (changed) {
            StartCoroutine(WaitForChange());
        }
        // Nếu có thay đổi gọi coroutin WaitForChange() để chờ hoàn ảnh hoàn tất
    }

    private bool MoveTile(Tile tile, Vector2Int direction) {
        TileCell newCell = null;
        // Biến newCell lưu trữ ô hợp lệ cuối cùng mà ô hiện tại có thể di chuyển tới
        TileCell adjacent = grid.GetAdjacentCell(tile.cell, direction);

        while (adjacent != null) {
            if (adjacent.occupied) {
                // Merging
                if (CanMerge(tile, adjacent.tile)) {
                    Merge(tile, adjacent.tile);
                    return true;
                }
                break;
            }

            newCell = adjacent;
            adjacent = grid.GetAdjacentCell(adjacent, direction);
            // Dừng vòng lặp khi đi đến biên lưới (adjacent = null)
        }
        // Tìm ô xa nhất có thể di chuyển đến

        if (newCell != null) {
            tile.MoveTo(newCell);
            // Di chuyển ô tile đến vị trí của ô hợp lệ newCell
            waiting = true;
            // Kích hoạt trạng thái chờ, ngăn người chơi nhập lệnh mới khi hoạt ảnh di chuyển
            return true;
            // Báo hiệu ô đã di chuyển thành công
        }
        return false;
    }

    private bool CanMerge(Tile a, Tile b) {
        return a.number == b.number && !b.locked;
    }

    private void Merge(Tile a, Tile b) {
        tiles.Remove(a);
        a.Merge(b.cell);

        int index = Mathf.Clamp(IndexOf(b.state) + 1, 0, tileStates.Length - 1);
        // Gán giá trị cho index = IndexOf(b.state) + 1 giới hạn trong khoảng 0 -> tileStates.Length - 1
        int number = b.number * 2;
        b.SetState(tileStates[index], number); 

        gameManager.IncreaseScore(number);

    }

    private int IndexOf(TileState state) {
        for (int i = 0; i < tileStates.Length; i++) {
            if (state == tileStates[i]) {
                return i;
            }
        }
        return -1;
    }
    private IEnumerator WaitForChange() {
        waiting = true;
        yield return new WaitForSeconds(0.1f);
        waiting = false;
        foreach (var tile in tiles) {
            tile.locked = false;
        }
        // Create new tile
    
        if (tiles.Count != grid.size) {
            CreateTile();
        }
        // Check game over
        if (CheckForGameOver()) {
            gameManager.GameOver();
        }


    }
    /*
    waiting đóng vai trò như 1 cơ chế khoá (block) để đảm bảo rằng :
    1. Hoạt ảnh của 1 lần di chuyển phải hoàn tất trc khi thực hiện hoạt ảnh tiếp theo
    2. Chỉ khi mọi ô đã di chuyển xong, trò chơi mới tạo ô mới hoặc kiểm tra trạng thái kết thúc
    3. Giao diện và logic luôn đồng bộ
    (duraion của animaiton phải bằng với thời gian trong WaitForSecond)
    */

    private bool CheckForGameOver() {
        if (tiles.Count != grid.size) {
            return false;
        }
        foreach (var tile in tiles) {
            TileCell up = grid.GetAdjacentCell(tile.cell, Vector2Int.up);
            TileCell down = grid.GetAdjacentCell(tile.cell, Vector2Int.down);
            TileCell left = grid.GetAdjacentCell(tile.cell, Vector2Int.left);
            TileCell right = grid.GetAdjacentCell(tile.cell, Vector2Int.right);
            if (up != null && CanMerge(tile, up.tile)) {
                return false;
            }
            if (down != null && CanMerge(tile, down.tile)) {
                return false;
            }
            if (left != null && CanMerge(tile, left.tile)) {
                return false;
            }
            if (right != null && CanMerge(tile, right.tile)) {
                return false;
            }

        }
        return true;
    }
    
}
