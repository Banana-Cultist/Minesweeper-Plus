using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public interface ITileDelegate
{
    bool IsPaused();
    void Click(TileController tile);
    void Flag(TileController tile);
    float GetLineWidth();

    void SetClickPosition(Vector3 pos);
}

public interface IBoardTypeDelegate
{
    TileController CreateTile();
}

public class BoardController : MonoBehaviour, ITileDelegate, IBoardTypeDelegate
{
    private RectTransform bounds;
    public GameObject tilePrefab;
    private List<TileController> tiles = new();
    public TextMeshProUGUI hud;
    public int minesLeft;
    public int totalMines;
    public int cleared;
    public float startTime;
    public bool isFirst = true;
    public bool paused = false;
    public BoardDelegate menuDelegate;
    public TileAnimationController tileAnimator;

    private List<TileController> clearingGroup = new();

    private Vector2 clearingGroupPosition;

    // Start is called before the first frame update
    void Start()
    {
        bounds = GetComponent<RectTransform>();
        int w = 10;
        //tiles = GridBoard.initialize(this, w, w, bounds, 135f, 0);
        tiles = VoronoiBoard.Initialize(this, w*w, bounds, 30);
        totalMines = w * w / 5;
        ResetBoard();
        InitializeBoard();
        ColorBoard();
        // PlaceMines(w * w / 5);
        // startTime = Time.time;
    }

    public void ResetBoard()
    {
        foreach (TileController tile in tiles)
        {
            tile.Reset();
        }
        
        minesLeft = totalMines;
        cleared = 0;
        isFirst = true;
        startTime = Time.time;
        paused = false;
        PlaceMines(totalMines);
        foreach(TileController tile in tiles)
        {
            tile.UpdateLabel();
            tile.image.ShowNumber();
        }

        tileAnimator.Reset();
    }

    public TileController CreateTile() {
        TileController tile = Instantiate(
            tilePrefab,
            transform.position, //+ new Vector3(-bounds.rect.width / 2, -bounds.rect.height / 2, 0),
            transform.rotation,
            transform
        ).GetComponent<TileController>();
        tile.tileDelegate = this;

        return tile;
    }

    public void InitializeBoard()
    {
        foreach (TileController tile in tiles)
        {
            tile.UpdateLabel();
            tile.UpdateShape();
        }
    }

    private void PlaceMines(int mines)
    {
        totalMines = mines;
        minesLeft = mines;
        List<TileController> available = new(tiles);
        PlaceMines(mines, available);
    }

    private void PlaceMines(int mines, TileController clearance)
    {
        totalMines = mines;
        minesLeft = mines;

        foreach (TileController tile in tiles) {
            tile.adjacentMines = 0;
            tile.isMine = false;
        }

        List<TileController> available = new(tiles);
        available.Remove(clearance);
        foreach(TileController neighbor in clearance.adjacents)
        {
            available.Remove(neighbor);
        }

        PlaceMines(mines, available);

        foreach(TileController tile in tiles)
        {
            tile.UpdateLabel();
        }
    }

    private void PlaceMines(int mines, List<TileController> available)
    {
        for (int i = 0; i < mines; i++)
        {
            TileController random = available[Random.Range(0, available.Count)];
            available.Remove(random);
            random.isMine = true;
            foreach (TileController neighbor in random.adjacents)
            {
                neighbor.adjacentMines++;
            }
        }
    }

    private Color Hexcode(string hexcode)
    {
        int r = int.Parse(hexcode.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        int g = int.Parse(hexcode.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        int b = int.Parse(hexcode.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        float den = 255f * 3;
        return new Color(r / den, g / den, b / den, 1);
    }

    void ColorBoard()
    {
        Color[] colors = new Color[] {
            Hexcode("537c78"),
            Hexcode("7ba591"),
            Hexcode("cc222b"),
            Hexcode("f15b4c"),
            Hexcode("faa41b"),
            Hexcode("ffd45b"),
        };

        // use a biased wave function collapse algorithm to color tiles
        Hashtable colorPossibilities = new();

        foreach (TileController tile in tiles)
        {
            colorPossibilities.Add(tile, new List<Color>(colors));
        }

        Hashtable colorDistribution = new();
        foreach (Color color in colors)
        {
            colorDistribution.Add(color, 0);
        }

        for (int i = 0; i < tiles.Count; i++)
        {
            List<TileController>[] entropy = new List<TileController>[colors.Length + 1];
            foreach (DictionaryEntry entry in colorPossibilities)
            {
                int potentialColors = (entry.Value as List<Color>).Count;
                TileController tile = entry.Key as TileController;
                if (entropy[potentialColors] == null)
                {
                    entropy[potentialColors] = new() { tile };
                }
                else
                {
                    entropy[potentialColors].Add(tile);
                }
            }

            TileController selected = null;
            for(int j = 0; j < entropy.Length; j++)
            {
                if (entropy[j] == null) continue;
                selected = entropy[j][Random.Range(0, entropy[j].Count)];
                break;
            }

            List<Color> possibilities = colorPossibilities[selected] as List<Color>;
            possibilities.Sort((Color a, Color b) =>
            {
                int distributionA = (int) colorDistribution[a];
                int distributionB = (int) colorDistribution[a];
                if (distributionA == distributionB) return Random.Range(-10, 10);
                return distributionB - distributionA;
            });

            Color collapsed = possibilities[0];
            selected.fillColor = collapsed;
            selected.UpdateColor();
            colorPossibilities.Remove(selected);
            foreach(TileController neighbor in selected.adjacents)
            {
                List<Color> neighborPossibilities = colorPossibilities[neighbor] as List<Color>;
                if (neighborPossibilities == null) continue;
                neighborPossibilities.Remove(collapsed);
                if (neighborPossibilities.Count == 0)
                {
                    Debug.Log("Fuck");
                    colorPossibilities.Remove(neighbor);
                }
            }
            colorDistribution[collapsed] = (int) colorDistribution[collapsed] + 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (paused || isFirst)
        {
            startTime += Time.deltaTime;
        }
        
        
        hud.text = "<color=\"blue\">" + Mathf.RoundToInt(Time.time - startTime) + "</color> / " +
                       "<color=\"red\">" + minesLeft + "</color>";

        if (clearingGroup.Count > 0) {
            tileAnimator.ClearingAnimation(clearingGroup, clearingGroupPosition);
        }
        // foreach (TileController tile in clearingGroup)
        // {
        //     // tile.Clear();
        // }
        clearingGroup.Clear();
    }

    public void Click(TileController tile)
    {
        Debug.Log("Click detected");

        if (Settings.GetToggle(Toggles.CLEAN_OPENING) && isFirst)
        {
            Debug.Log("First Click; Replacing Mines");
            PlaceMines(totalMines, tile);
            isFirst = false;
        }

        if (tile.isMine && !tile.flagged)
        {
            menuDelegate.GameCompleted(false);
            return;
        }

        if (!tile.cleared && !tile.flagged)
        {
            tile.cleared = true;
            if (tile.adjacentMines == 0)
            {
                ClearEmpty(tile);
            }
            else
            {
                clearingGroup.Add(tile);
                cleared++;
            }
        }
        else if (!tile.flagged && Settings.GetToggle(Toggles.CLEAR_CHORD))
        {
            ClearChord(tile);
        }

        if (tiles.Count - cleared == totalMines)
        {
            foreach(TileController remaining in tiles) {
                if (!remaining.cleared) {
                    if (remaining.isMine) {
                        remaining.Flag();
                    } else {
                        clearingGroup.Add(remaining);
                    }
                }
            }
            menuDelegate.GameCompleted(true);
        }
    }

    private void ClearEmpty(TileController tile)
    {
        HashSet<TileController> visited = new();
        List<TileController> toClear = new() { tile };
        while (toClear.Count > 0)
        {
            TileController current = toClear[0];
            toClear.RemoveAt(0);
            visited.Add(current);

            // current.Unflag();

            clearingGroup.Add(current);

            cleared++;
            if (current.adjacentMines == 0)
            {
                foreach (TileController neighbor in current.adjacents)
                {
                    if (!neighbor.cleared &&
                        !toClear.Contains(neighbor) &&
                        !visited.Contains(neighbor))
                    {
                        toClear.Add(neighbor);
                    }
                }
            }
        }
    }

    private void ClearChord(TileController tile)
    {
        int adjacentFlags = 0;
        List<TileController> nonFlagged = new();
        foreach (TileController neighbor in tile.adjacents)
        {
            if (neighbor.flagged)
            {
                adjacentFlags++;
            }
            else if (!neighbor.cleared)
            {
                nonFlagged.Add(neighbor);
            }
        }

        if (adjacentFlags >= tile.adjacentMines)
        {
            foreach (TileController nonCleared in nonFlagged)
            {
                Click(nonCleared);
            }
        }
    }

    public void Flag(TileController tile)
    {
        Debug.Log("Flag detected");
        if (!tile.cleared)
        {
            if (Settings.GetToggle(Toggles.MINE_SWEEPING))
            {
                if (tile.isMine)
                {
                    RemoveMine(tile);
                }
                else
                {
                    menuDelegate.GameCompleted(false);
                }
            }
            else
            {
                if (tile.flagged)
                {
                    tile.Unflag();
                    minesLeft++;
                }
                else
                {
                    tile.Flag();
                    minesLeft--;
                }
            }
        }
        else if (Settings.GetToggle(Toggles.FLAG_CHORD))
        {
            FlagChord(tile);
        }
    }

    private void RemoveMine(TileController tile)
    {
        tile.isMine = false;
        minesLeft--;
        cleared--;
        tile.UpdateLabel();
        foreach (TileController neighbor in tile.adjacents)
        {
            neighbor.adjacentMines -= 1;
            neighbor.UpdateLabel();
            if (neighbor.cleared && neighbor.adjacentMines == 0)
            {
                Click(neighbor);
            }
        }
        Click(tile);
    }

    private void FlagChord(TileController tile)
    {
        int adjacentNonCleared = 0;
        List<TileController> nonFlagged = new();
        foreach (TileController neighbor in tile.adjacents)
        {
            if (!neighbor.cleared)
            {
                adjacentNonCleared++;
                if (!neighbor.flagged)
                {
                    nonFlagged.Add(neighbor);
                }
            }
        }
        if (adjacentNonCleared <= tile.adjacentMines)
        {
            foreach (TileController nonCleared in nonFlagged)
            {
                if (Settings.GetToggle(Toggles.MINE_SWEEPING))
                {
                    Flag(nonCleared);
                }
                else
                {
                    nonCleared.Flag();
                    minesLeft--;
                }
            }
        }
    }

    public void Reveal()
    {
        foreach (TileController tile in tiles)
        {
            tile.Reveal();
        }
    }

    public bool IsPaused()
    {
        return paused;
    }

    public float GetLineWidth()
    {
        return bounds.sizeDelta.magnitude / Mathf.Sqrt(tiles.Count) / 20;
    }

    public void SetClickPosition(Vector3 pos)
    {
        clearingGroupPosition = pos;
    }
}
