﻿using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum Phase { Day, Night, Dawn };
public enum Weather { Sun, Clouds, Rain };
public enum Weapon { Red, Green, Blue };

public class GameManager : MonoBehaviour
{
    private static GameManager gameManager;

    [SerializeField] private GameObject plantPrefab = null;
    [SerializeField] private GameObject enemyPrefab = null;
    [SerializeField] private GameObject[] tilePrefabs = null;

    [SerializeField] private TextMeshProUGUI dayLabel = null;
    [SerializeField] private TextMeshProUGUI plantLabel = null;
    [SerializeField] private TextMeshProUGUI seedLabel = null;
    [SerializeField] private TextMeshProUGUI ERPLabel = null;
    [SerializeField] private TextMeshProUGUI weaponLabel = null;
    [SerializeField] private TextMeshProUGUI weaponNo1 = null;
    [SerializeField] private TextMeshProUGUI weaponNo2 = null;
    [SerializeField] private TextMeshProUGUI weaponNo3 = null;

    [SerializeField] private GameObject weaponREffect = null;
    [SerializeField] private GameObject weaponGEffect = null;
    [SerializeField] private GameObject weaponBEffect = null;

    [SerializeField] private Button advanceButton = null;
    [SerializeField] private TextMeshProUGUI buttonLabel = null;
    [SerializeField] private Image clockFill = null;

    [SerializeField] private GameObject menu = null;

    private bool inGame = false;

    private const int BOARD_WIDTH = 9, BOARD_HEIGHT = 9;
    private IEntity[,] gameBoard;
    private Tile[,] tileBoard;
    private Dictionary<Plant, uint> plantHistory;
    private List<Enemy> activeEnemies;
    private Weapon weapon; /* false for red, true for blue */

    /* GLOBAL STATS */
    /* The only one we should need to share with the user is the SeedCount. */
    private uint plantCount;
    private uint ERPCount;
    private uint seedCount;
    private int hydration;
    private float nutrients;

    private uint currentEnemyCount;

    /* Time-related data */
    private float remainingTime;
    private const float RESET_TIME = 10;

    /* Cycle-related data */
    private uint currentCycle;
    private uint goalCycle;

    /* A constant used anywhere, for graphical rendering */
    /* Assumes a 9 tile x 9 tile (900p x 900p) board */
    public const float TILE_SIZE = 1.000f;

    private const int SEED_RATIO = 2; // 2 seeds extracted per plant

    private Phase currentPhase;
    private Weather currentWeather;

    public static GameManager GetInstance()
    {
        return gameManager;
    }

    public IEntity EntityAt(int x, int y) {
        return gameBoard[y, x];
    }

    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = this;
            DontDestroyOnLoad(gameManager);
        } else if (gameManager != this)
        {
            Destroy(gameManager);
        }
    }

    /// <summary>
    /// To be called from the Button object
    /// </summary>
    public void AdvancePhase()
    {
        if (currentPhase == Phase.Dawn)
        {
            currentPhase = Phase.Day;
            buttonLabel.text = "Advance\nto\nNight";
        } else if (currentPhase == Phase.Day)
        {
            currentPhase = Phase.Night;
            remainingTime = RESET_TIME;
            advanceButton.enabled = false;
            clockFill.fillAmount = 1f;
            clockFill.gameObject.SetActive(true);
            buttonLabel.text = remainingTime.ToString("f1");
        } /*else if (RemainingTime <= 0)
        {
            currentPhase = Phase.Dawn;
            remainingTime = ResetTime;
        }*/ // should not need this...
    }

    public Phase GetPhase()
    {
        return currentPhase;
    }

    /// <summary>
    /// Precondition: x and y are nonnegative integers that are valid game board indices.
    /// Postcondition: processes the addition of this plant object,
    /// by adding it to the game board and also to the plant history data structure.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void AddPlant(uint x, uint y)
    {
        if (seedCount > 0)
        {
            if (gameBoard[y, x].GetTag() == Type.Plant)
            {

            }
            else if (((EnemyHorde)gameBoard[y, x]).Size() == 0)
            {
                Plant spawn = Instantiate(plantPrefab, new Vector3(x * TILE_SIZE, y * TILE_SIZE, 0), Quaternion.identity).GetComponent<Plant>();
                spawn.X = x;
                spawn.Y = y;
                plantHistory.Add(spawn, currentCycle);
                gameBoard[y, x] = spawn;
                plantCount++;
                seedCount--;
            }
        }
    }

    /// <summary>
    /// To be called by a Plant, whenever it is extracted.
    /// </summary>
    /// <param name="extracted"></param>
    public void ExtractPlant(Plant extracted)
    {
        plantCount--;
        ERPCount++;

        /* TODO process the "Return," which means
         * increasing the Hydration and/or Nutrients globally.
         * Another TODO: figure out seed yield and return it to player */
        uint daysAlive = currentCycle - plantHistory[extracted];
        hydration = Mathf.Max(20, hydration + (int)daysAlive);
        nutrients += Mathf.Max(10.0f, nutrients + (0.1f*daysAlive));
        plantHistory.Remove(extracted);
        gameBoard[extracted.Y, extracted.X] = new EnemyHorde();
        Destroy(extracted.gameObject);
        seedCount += SEED_RATIO;
    }

    /// <summary>
    /// To be called by a Plant OR Pathogen object, whenever it kills a plant.
    /// </summary>
    public void KillPlant(int x, int y)
    {
        if (x < 0 || y < 0 || x >= BOARD_WIDTH || y >= BOARD_HEIGHT)
        {
            throw new System.Exception();
        }
        plantCount--;
        plantHistory.Remove((Plant)gameBoard[y, x]);
        Destroy(((Plant)gameBoard[y, x]).gameObject);
        gameBoard[y, x] = new EnemyHorde();
    }

    /// <summary>
    /// Returns a Vector3 storing the x and y coordinates of the nearest Plant
    /// relative to the x and y arguments.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>Vector3 with all components negative if no Plants on gameBoard</returns>
    public Vector3 GetNearestPlantPosition(int x, int y)
    {
        Vector3 nearest = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        foreach (Plant p in plantHistory.Keys)
        {
            if (Mathf.Abs(p.X - x) + Mathf.Abs(p.Y - y) < Mathf.Abs(nearest.x - x) + Mathf.Abs(nearest.y - y))
            {
                nearest = new Vector3(p.X, p.Y, 0);
            }
        }

        return nearest;
    }

    private void Update()
    {
        if (inGame)
        {
            switch (currentPhase)
            {
                case Phase.Dawn:

                    break;
                case Phase.Day:

                    break;
                case Phase.Night:
                    /* Update time, clock, and text */
                    remainingTime -= Time.deltaTime;
                    clockFill.fillAmount = remainingTime / RESET_TIME;
                    buttonLabel.text = remainingTime.ToString("f1");
                    /* Advance to next phase (Dawn if seeds are available, Day otherwise) at Night's end */
                    if (remainingTime <= 0)
                    {
                        ClearTileHighlights();

                        if (seedCount > 0)
                        {
                            currentPhase = Phase.Dawn;
                            buttonLabel.text = "Finished\nPlanting";
                        }
                        else
                        {
                            currentPhase = Phase.Day;
                            buttonLabel.text = "Advance\nto\nNight";
                        }
                        currentCycle++;
                        clockFill.gameObject.SetActive(false);
                        advanceButton.enabled = true;
                        return;
                    }
                    //uint enemyLimit = (uint) (nutrients / 0.25f);
                    uint enemyLimit = 1;
                    if (currentEnemyCount < enemyLimit)
                    {
                        if (remainingTime < 0.25 * RESET_TIME &&
                            remainingTime > 0.22 * RESET_TIME)
                        {
                            for (uint iter = currentEnemyCount; iter < enemyLimit; iter++)
                            {
                                int nX;
                                int nY;
                                if (Random.Range(0.0f, 1.0f) < 0.50f)
                                {
                                    nX = Random.Range(0, (int)TILE_SIZE * BOARD_WIDTH);
                                    nY = 0;
                                }
                                else
                                {
                                    nX = 0;
                                    nY = Random.Range(0, (int)TILE_SIZE * BOARD_HEIGHT);
                                }
                                Enemy next = Instantiate(enemyPrefab, new Vector3(nX,
                                    nY, 0), Quaternion.identity).GetComponent<Enemy>();
                                activeEnemies.Add(next);
                                if (gameBoard[nY, nX].GetTag() == Type.Plant)
                                {
                                    KillPlant(nX, nY);
                                }
                                ((EnemyHorde)gameBoard[nY, nX]).AddEnemy(next);
                            }
                            currentEnemyCount = enemyLimit;
                            /*                    } else if (remainingTime < 0.70f*RESET_TIME) {*/
                        } /*else if ((remainingTime > 0.70f * RESET_TIME) &&
                         (remainingTime < 0.75f * RESET_TIME))
                    {
                        float chance = Random.Range(0.0f, 1.0f);
                        if (chance > 0.60f)
                        {
                            float nX;
                            Enemy next = Instantiate(enemyPrefab,
                                new Vector3((nX = Random.Range(0.0f, TILE_SIZE * width)),
                                0, 0), Quaternion.identity).GetComponent<Enemy>();
                            activeEnemies.Add(next);
                            if (gameBoard[0, (int)nX].GetTag() == Type.Plant)
                            {
                                KillPlant((int)nX, 0);
                            }
                            ((EnemyHorde)gameBoard[0, (int)nX]).AddEnemy(next);
                        }
                    }*/
                    }
                    if (Input.GetMouseButtonUp(0))
                    {
                        
                    }
                    break;
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                weapon = Weapon.Red;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                weapon = Weapon.Green;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                weapon = Weapon.Blue;
            }

            dayLabel.text = "Day: " + currentCycle;
            plantLabel.text = "Plants on Field: " + plantCount;
            seedLabel.text = "Seeds: " + seedCount;
            ERPLabel.text = "[ERP]: " + ERPCount;
            switch (weapon)
            {
                case Weapon.Red:
                    weaponLabel.text = "<color=red>Red</color>";
                    weaponNo1.fontSize = 48;
                    weaponNo2.fontSize = 32;
                    weaponNo3.fontSize = 32;
                    break;
                case Weapon.Green:
                    weaponLabel.text = "<color=green>Green</color>";
                    weaponNo1.fontSize = 32;
                    weaponNo2.fontSize = 48;
                    weaponNo3.fontSize = 32;
                    break;
                case Weapon.Blue:
                    weaponLabel.text = "<color=blue>Blue</color>";
                    weaponNo1.fontSize = 32;
                    weaponNo2.fontSize = 32;
                    weaponNo3.fontSize = 48;
                    break;
            }
        }
    }

    public void OnTileClick(uint tileX, uint tileY)
    {
        if (inGame)
        {
            switch (currentPhase)
            {
                case Phase.Dawn:
                    AddPlant(tileX, tileY);
                    break;
                case Phase.Day:
                    if (gameBoard[tileY, tileX].GetTag() == Type.Plant)
                    {
                        ExtractPlant((Plant)gameBoard[tileY, tileX]);
                    }
                    break;
                case Phase.Night:
                    switch (weapon)
                    {
                        case Weapon.Red:
                            Instantiate(weaponREffect, new Vector3(tileX * TILE_SIZE, tileY * TILE_SIZE, 0f), Quaternion.identity);
                            break;
                        case Weapon.Green:
                            Instantiate(weaponGEffect, new Vector3(tileX * TILE_SIZE, tileY * TILE_SIZE, 0f), Quaternion.identity);
                            break;
                        case Weapon.Blue:
                            Instantiate(weaponBEffect, new Vector3(tileX * TILE_SIZE, tileY * TILE_SIZE, 0f), Quaternion.identity);
                            break;
                    }
                    break;
            }
        }
    }

    public void Play()
    {
        InitGame();
        StartCoroutine(MoveMenu(1));
        inGame = true;
    }

    public void Quit()
    {
        StartCoroutine(MoveMenu(-1));
        ClearGameBoard();
        ClearTileHighlights();
        inGame = false;
    }

    private IEnumerator MoveMenu(int direction)
    {
        RectTransform rectTransform = menu.GetComponent<RectTransform>();
        float targetY = direction > 0 ? 1700f : 0f, speed = 1700f;

        while (rectTransform.anchoredPosition.y != targetY)
        {
            rectTransform.anchoredPosition = Vector2.MoveTowards(rectTransform.anchoredPosition, new Vector2(0, targetY) * direction, Time.deltaTime * speed);
            yield return new WaitForSeconds(0.01f);
        }
    }

    private void ClearGameBoard()
    {
        foreach (Plant p in plantHistory.Keys)
        {
            Destroy(p.gameObject);
        }
        for (int i = activeEnemies.Count - 1; activeEnemies.Count > 0; i--)
        {
            Destroy(activeEnemies[i].gameObject);
            activeEnemies.RemoveAt(i);
        }
    }

    private void ClearTileHighlights()
    {
        for (int r = 0; r < BOARD_HEIGHT; r++)
        {
            for (int c = 0; c < BOARD_WIDTH; c++)
            {
                tileBoard[r, c].Highlight(false);
            }
        }
    }

    private void InitGame()
    {
        /* Init data structures */
        gameBoard = new IEntity[BOARD_HEIGHT, BOARD_WIDTH];
        tileBoard = new Tile[BOARD_HEIGHT, BOARD_WIDTH];
        for (uint r = 0; r < BOARD_HEIGHT; r++)
        {
            for (uint c = 0; c < BOARD_WIDTH; c++)
            {
                /* 50% chance for plain tile, 25% chance each for other types */
                int rand = Random.Range(0, 4);
                int tileIndex = rand >= tilePrefabs.Length ? 0 : rand;
                Tile tile = Instantiate(tilePrefabs[tileIndex], new Vector3(c * TILE_SIZE, r * TILE_SIZE, 0f), Quaternion.identity).GetComponent<Tile>();
                tile.SetPosition(c, r);
                tileBoard[r, c] = tile;

                gameBoard[r, c] = new EnemyHorde();
            }
        }
        for (int r = 0; r < BOARD_HEIGHT; r++)
        {
            for (int c = 0; c < BOARD_WIDTH; c++)
            {
                // Above
                if (r > 0)
                {
                    tileBoard[r, c].AddAdjacentTile(tileBoard[r - 1, c]);
                }
                // Below
                if (r < BOARD_HEIGHT - 1)
                {
                    tileBoard[r, c].AddAdjacentTile(tileBoard[r + 1, c]);
                }
                // Left
                if (c > 0)
                {
                    tileBoard[r, c].AddAdjacentTile(tileBoard[r, c - 1]);
                }
                // Right
                if (c < BOARD_WIDTH - 1)
                {
                    tileBoard[r, c].AddAdjacentTile(tileBoard[r, c + 1]);
                }
            }
        }

        plantHistory = new Dictionary<Plant, uint>();
        activeEnemies = new List<Enemy>();

        /* Initialize at dawn. */
        currentPhase = Phase.Dawn;
        buttonLabel.text = "Finished\nPlanting";
        clockFill.gameObject.SetActive(false);
        currentWeather = Weather.Sun;
        currentCycle = 1;

        plantCount = 0;
        currentEnemyCount = 0;
        ERPCount = 0;
        seedCount = 2;
        hydration = 10; // on a scale 0 - 20?
        nutrients = 4.50f; // on a scale 0 - 10?
        remainingTime = RESET_TIME;
        weapon = Weapon.Red;
    }
}
