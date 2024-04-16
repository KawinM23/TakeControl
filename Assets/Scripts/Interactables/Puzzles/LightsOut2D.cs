using UnityEngine;

public class LightsOut2D : MonoBehaviour
{

    [SerializeField] private int _gridSize = 5; // Adjust this value to change the grid size
    [SerializeField] private LightsOut2DTile _tilePrefab;
    [SerializeField] private Color _onColor;
    [SerializeField] private Color _offColor;
    [SerializeField] private Color _rewardColor;

    private LightsOut2DTile[] tiles;
    [SerializeField] private bool[] lights;
    private bool rewardEarned;

    private void Start()
    {
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        tiles = new LightsOut2DTile[_gridSize];
        lights = new bool[_gridSize];

        float startX = -(_gridSize - 1) / 2f;
        float yOffset = _tilePrefab.transform.position.y;

        for (int i = 0; i < _gridSize; i++)
        {
            LightsOut2DTile tile = Instantiate(_tilePrefab, new Vector3(startX + i, yOffset, 0), Quaternion.identity);
            tile.SetIndex(i);
            tiles[i] = tile;
            lights = RandomInitialLights(_gridSize);
        }

        // The representative file should not be in game
        _tilePrefab.gameObject.SetActive(false);
    }

    private void Update()
    {
        UpdateTilesColor();
        CheckWinCondition();
    }

    private void UpdateTilesColor()
    {
        for (int i = 0; i < _gridSize; i++)
        {
            SpriteRenderer renderer = tiles[i].GetComponent<SpriteRenderer>();
            renderer.color = lights[i] ? _onColor : _offColor;
        }
    }

    public void ToggleLights(int index)
    {
        // if already got reward, dont change anything
        if (rewardEarned)
        {
            return;
        }
        SoundManager.Instance.PlayDing(2, 5); // hardcode for unchanging pitch

        lights[index] = !lights[index];
        SpriteRenderer renderer = tiles[index].GetComponent<SpriteRenderer>();
        renderer.color = lights[index] ? _onColor : _offColor;

        if (index > 0)
        {
            lights[index - 1] = !lights[index - 1];
            SpriteRenderer prevRenderer = tiles[index - 1].GetComponent<SpriteRenderer>();
            prevRenderer.color = lights[index - 1] ? _onColor : _offColor;
        }

        if (index < _gridSize - 1)
        {
            lights[index + 1] = !lights[index + 1];
            SpriteRenderer nextRenderer = tiles[index + 1].GetComponent<SpriteRenderer>();
            nextRenderer.color = lights[index + 1] ? _onColor : _offColor;

        }
    }

    private void CheckWinCondition()
    {
        bool allLightsOn = true;
        bool allLightsOff = true;

        foreach (bool light in lights)
        {
            if (light)
            {
                allLightsOff = false;
            }
            else
            {
                allLightsOn = false;
            }
        }

        if (allLightsOn)
        {
            if (!rewardEarned)
            {
                SoundManager.Instance.PlayMagicCoin();
                if (gameObject.TryGetComponent(out DropItem dropItem)) dropItem.DropCurrency();
            }
            rewardEarned = true;
            foreach (LightsOut2DTile tile in tiles)
            {
                SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();
                renderer.color = _rewardColor;
            }

        }
    }

    private bool[] RandomInitialLights(int length)
    {
        // Initially, set all to true to make the puzzle solvable
        bool[] lights = new bool[length];

        for (int i = 0; i < length; i++)
        {
            lights[i] = true;
        }

        // Randomly toggle lights
        for (int i = 0; i < length; i++)
        {
            if (Random.value < 0.5f)
            {
                ToggleInitialLights(i, lights);
            }
        }

        return lights;
    }

    private void ToggleInitialLights(int index, bool[] lights)
    {
        lights[index] = !lights[index];
        if (index > 0)
        {
            lights[index - 1] = !lights[index - 1];
        }

        if (index < _gridSize - 1)
        {
            lights[index + 1] = !lights[index + 1];
        }
    }
}


