using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum Phase { Day, Night, Dawn };
public enum Weather { Sun, Clouds, Rain };

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

    [SerializeField] private Button advanceButton = null;
    [SerializeField] private TextMeshProUGUI buttonLabel = null;
    [SerializeField] private Image clockFill = null;

    private int width, height;
    private Entity[,] gameBoard;
    private Dictionary<Plant, uint> plantHistory;
    private bool weaponMode;

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
    private const float RESET_TIME = 3;

    /* Cycle-related data */
    private uint currentCycle;
    private uint goalCycle;

    /* A constant used anywhere, for graphical rendering */
    /* Assumes a 16 tile x 16 tile (800p x 800p) board */
    public const float TILE_SIZE = 0.500f;

    private Phase currentPhase;
    private Weather currentWeather;

    public static GameManager GetInstance()
    {
        return gameManager;
    }

    public Entity EntityAt(int x, int y) {
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
        /*else // no-op
        {

        }*/
/*        InitGame();*/
    }

    // Start is called before the first frame update
    private void Start()
    {
        /* Init data structures */
        width = 10;
        height = 10;
        gameBoard = new Entity[height, width];
        for (int r = 0; r < height; r++)
        {
            for (int c = 0; c < width; c++)
            {
                gameBoard[r, c] = new EnemyHorde();
            }
        }

        plantHistory = new Dictionary<Plant, uint>();

        /* Initialize at dawn. */
        currentPhase = Phase.Dawn;
        buttonLabel.text = "Finished\nPlanting";
        clockFill.gameObject.SetActive(false);
        currentWeather = Weather.Sun;
        currentCycle = 1;

        plantCount = 0;
        currentEnemyCount = 0;
        ERPCount = 0;
        seedCount = 1;
        hydration = 10; // on a scale 0 - 20?
        nutrients = 4.50f; // on a scale 0 - 10?
        remainingTime = RESET_TIME;
        weaponMode = false;
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

    /// <summary>
    /// Precondition: x and y are nonnegative integers that are valid game board indices
    /// Postcondition: processes the addition of this plant object,
    /// by adding it to the game board and also to the plant history data structure.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void AddPlant(uint x, uint y)
    {
        Plant Spawn = Instantiate(plantPrefab, new Vector3(x*TILE_SIZE, y*TILE_SIZE, 0), Quaternion.identity).GetComponent<Plant>();
        plantHistory.Add(Spawn, currentCycle);
        gameBoard[y,x] = Spawn;
    }

    /// <summary>
    /// To be called by a Plant, whenever it is extracted.
    /// </summary>
    /// <param name="extracted"></param>
    public void ExtractPlant(Plant extracted)
    {
        gameBoard[extracted.Y, extracted.X] = null;
        plantCount--;
        ERPCount++;

        /* TODO process the "Return," which means
         * increasing the Hydration and/or Nutrients globally. */
        uint daysAlive = currentCycle - plantHistory[extracted];
        hydration = Mathf.Max(20, hydration + (int)daysAlive);
        nutrients += Mathf.Max(10.0f, nutrients + (0.1f*daysAlive));
    }

    /// <summary>
    /// To be called by a Plant OR Pathogen object, whenever it kills a plant.
    /// </summary>
    public void KillPlant(int x, int y)
    {
        if (x < 0 || y < 0 || x >= width || y >= height)
        {
            throw new System.Exception();
        }
        plantCount--;
        Destroy((Plant)gameBoard[y, x]);
        gameBoard[y, x] = new EnemyHorde();
    }

    // Update is called once per frame
    private void Update()
    {
        switch (currentPhase)
        {
            case Phase.Dawn:

                break;
            case Phase.Day:
                
                break;
            case Phase.Night:
                remainingTime -= Time.deltaTime;
                clockFill.fillAmount = remainingTime / RESET_TIME;
                buttonLabel.text = remainingTime.ToString("f1");
                if (remainingTime <= 0)
                {
                    currentPhase = Phase.Dawn;
                    currentCycle ++;
                    clockFill.gameObject.SetActive(false);
                    advanceButton.enabled = true;
                    buttonLabel.text = "Finished\nPlanting";
                    return;
                }
                uint enemyLimit = (uint) (nutrients / 0.25f);
                if (currentEnemyCount < enemyLimit)
                {
                    if (remainingTime < 0.25 * RESET_TIME && 
                        remainingTime > 0.22*RESET_TIME)
                    {
                        for (uint iter = currentEnemyCount; iter < enemyLimit; iter++)
                        {
                            if (Random.Range(0.0f, 1.0f) < 0.50f)
                            {
                                Enemy next = (Instantiate(enemyPrefab, new Vector3(Random.Range(0.0f, TILE_SIZE * width),
                                0, 0), Quaternion.identity)).AddComponent<Enemy>();
                            } else
                            {
                                Enemy next = (Instantiate(enemyPrefab, new Vector3(0,
                                Random.Range(0.0f, TILE_SIZE*height), 0), Quaternion.identity)).AddComponent<Enemy>();
                            }
                        }
                        currentEnemyCount = enemyLimit;
/*                    } else if (remainingTime < 0.70f*RESET_TIME) {*/
                    } else if ((remainingTime > 0.70f * RESET_TIME) &&
                         (remainingTime < 0.75f * RESET_TIME))
                    {
                        float chance = Random.Range(0.0f, 1.0f);
                        if (chance > 0.60f)
                        {
                            Enemy next = (Instantiate(enemyPrefab,
                                new Vector3(Random.Range(0.0f, TILE_SIZE * width),
                                0, 0), Quaternion.identity)).AddComponent<Enemy>();
                        }
                    }
                }
                if (Input.GetKeyDown(KeyCode.A))
                {
                    weaponMode = true;
                }
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    weaponMode = false;
                }
                break;
            default:

                break;
        }

        dayLabel.text = "Day: " + currentCycle;
        plantLabel.text = "Plants on Field: " + plantCount;
        seedLabel.text = "Seeds: " + seedCount;
        ERPLabel.text = "[ERP]: " + ERPCount;
    }
}
