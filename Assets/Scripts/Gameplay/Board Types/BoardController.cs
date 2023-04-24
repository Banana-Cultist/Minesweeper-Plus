using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public interface TileDelegate
{
    bool isPaused();
    void click(TileController tile);
    void flag(TileController tile);
    float getLineWidth();
}

public interface BoardTypeDelegate
{
    TileController createTile();
}

public class BoardController : MonoBehaviour, TileDelegate, BoardTypeDelegate
{
    private RectTransform bounds;
    public GameObject tilePrefab;
    private List<TileController> tiles = new List<TileController>();
    public TextMeshProUGUI hud;
    public int minesLeft;
    public int totalMines;
    public int cleared;
    public float startTime;
    public bool isFirst = true;
    public bool paused = false;
    public BoardDelegate menuDelegate;

    // Start is called before the first frame update
    void Start()
    {
        bounds = GetComponent<RectTransform>();
        int w = 25;
        //tiles = GridBoard.initialize(this, w, w, bounds, 135f, 0);
        tiles = VoronoiBoard.initialize(this, w*w, bounds, 30);
        placeMines(w * w / 5);
        InitializeBoard();
        colorBoard();
        startTime = Time.time;
    }

    public void resetBoard()
    {
        foreach (TileController tile in tiles)
        {
            tile.ResetValues();
        }
        minesLeft = totalMines;
        cleared = 0;
        isFirst = true;
        startTime = Time.time;
        paused = false;
        placeMines(totalMines);
        foreach(TileController tile in tiles)
        {
            tile.UpdateLabel();
        }
    }

    public TileController createTile() {
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

    private void placeMines(int mines)
    {
        totalMines = mines;
        minesLeft = mines;
        List<TileController> available = new List<TileController>(tiles);
        placeMines(mines, available);
    }

    private void placeMines(int mines, TileController clearance)
    {
        totalMines = mines;
        minesLeft = mines;
        foreach(TileController tile in tiles)
        {
            tile.ResetValues();
        }
        List<TileController> available = new List<TileController>(tiles);
        available.Remove(clearance);
        foreach(TileController neighbor in clearance.adjacents)
        {
            available.Remove(neighbor);
        }

        placeMines(mines, available);

        foreach(TileController tile in tiles)
        {
            tile.UpdateLabel();
        }
    }

    private void placeMines(int mines, List<TileController> available)
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

    Color hexcode(string hexcode)
    {
        int r = int.Parse(hexcode.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        int g = int.Parse(hexcode.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        int b = int.Parse(hexcode.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        float den = 255f * 3;
        return new Color(r / den, g / den, b / den, 1);
    }

    void colorBoard()
    {
        Color[] colors = new Color[] {
            hexcode("537c78"),
            hexcode("7ba591"),
            hexcode("cc222b"),
            hexcode("f15b4c"),
            hexcode("faa41b"),
            hexcode("ffd45b"),
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
        
    }

    public void click(TileController tile)
    {
        Debug.Log("Click detected");

        if (PlayerPrefs.GetInt("CleanOpening", 0) == 1 && isFirst)
        {
            placeMines(totalMines, tile);
        }
        isFirst = false;

        if (tile.isMine && !tile.flagged)
        {
            menuDelegate.gameCompleted(false);
            return;
        }

        if (!tile.cleared && !tile.flagged)
        {
            if (tile.adjacentMines == 0)
            {
                HashSet<TileController> visited = new HashSet<TileController>();
                List<TileController> toClear = new List<TileController> { tile };
                while (toClear.Count != 0)
                {
                    TileController current = toClear[0];
                    toClear.RemoveAt(0);
                    visited.Add(current);
                    current.Unflag();
                    current.Clear();
                    cleared++;
                    if (current.adjacentMines == 0)
                    {
                        foreach (TileController neighbor in current.adjacents)
                        {
                            if (!neighbor.cleared && !toClear.Contains(neighbor)) toClear.Add(neighbor);
                        }
                    }
                }
            }
            else
            {
                tile.Clear();
                cleared++;
            }
        }
        else if (!tile.flagged && PlayerPrefs.GetInt("ClearChord", 0) == 1)
        {
            int adjacentFlags = 0;
            List<TileController> nonFlagged = new List<TileController>();
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
                    click(nonCleared);
                }
            }
        }

        if (tiles.Count - cleared == totalMines)
        {
            menuDelegate.gameCompleted(true);
        }
    }

    public void flag(TileController tile)
    {
        Debug.Log("Flag detected");
        if (!tile.cleared)
        {
            if (PlayerPrefs.GetInt("MineSweeping", 0) == 1)
            {
                if (tile.isMine)
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
                            click(neighbor);
                        }
                    }
                    click(tile);
                }
                else
                {
                    menuDelegate.gameCompleted(false);
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
        else if (PlayerPrefs.GetInt("FlagChord", 0) == 1)
        {
            int adjacentNonCleared = 0;
            List<TileController> nonFlagged = new List<TileController>();
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
                    if (PlayerPrefs.GetInt("MineSweeping", 0) == 1)
                    {
                        flag(nonCleared);
                    }
                    else
                    {
                        nonCleared.Flag();
                        minesLeft--;
                    }
                }
            }
        }
    }

    public bool isPaused()
    {
        return paused;
    }

    public float getLineWidth()
    {
        return bounds.sizeDelta.magnitude / Mathf.Sqrt(tiles.Count) / 20;
    }
}
