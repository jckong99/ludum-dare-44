using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Phase { Day, Night, Dawn };
public enum Weather { Sun, Clouds, Rain };

public class GameManager : MonoBehaviour
{
    private static GameManager gameManager;
    [SerializeField] private GameObject plantPrefab = null;

    private int width, height;
    private IEntity[,] gameBoard;
    private Dictionary<Plant, uint> plantHistory;

    /* GLOBAL STATS */
    /* The only one we should need to share with the user is the SeedCount. */
    private uint plantCount;
    private uint ERPCount;
    private uint seedCount;
    private int hydration;
    private float nutrients;
    private float remainingTime;
    private const float RESET_TIME = 300;
    private uint currentCycle;
    private uint goalCycle;
    public const float TILE_SIZE = 0.500f;

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
        } else if (gameManager != this)
        {
            Destroy(gameManager);
        }
        /*else // no-op
        {

        }*/
        DontDestroyOnLoad(gameManager);
/*        InitGame();*/
    }

    // Start is called before the first frame update
    private void Start()
    {
        /* Init data structures */
        width = 10;
        height = 10;
        gameBoard = new IEntity[height, width];
        for (int r=0; r < height; r ++)
        {
            for (int c = 0; c < width; c++)
            {
 /*               gameBoard[r,c] = Instantiate(plantPrefab, new Vector3(c * TILE_SIZE, r * TILE_SIZE, 0), Quaternion.identity);*/
            }
        }

        plantHistory = new Dictionary<Plant, uint>();

        /* Initialize at dawn. */
        currentPhase = Phase.Dawn;
        currentWeather = Weather.Sun;

        plantCount = 0;
        ERPCount = 0;
        seedCount = 1;
        hydration = 10; // on a scale 0 - 20?
        nutrients = 4.50f; // on a scale 0 - 10?
        remainingTime = RESET_TIME;
    }

    /* To be called from the Button object */
    public void AdvancePhase()
    {
        if (currentPhase == Phase.Dawn)
        {
            currentPhase = Phase.Day;
        } else if (currentPhase == Phase.Day)
        {
            currentPhase = Phase.Night;
        } /*else if (RemainingTime <= 0)
        {
            CurrentPhase = Phase.Dawn;
            RemainingTime = ResetTime;
        }*/ // should not need this...
    }

    /* Precondition: x and y are nonnegative integers that are valid
     * game board indices
     * Postcondition: processes the addition of this plant object,
     * by adding it to the game board and also to the plant history 
     * data structure. */
    public void AddPlant(uint x, uint y)
    {
        Plant Spawn/* = Instantiate(plantPrefab, new Vector3(x*TILE_SIZE, y*TILE_SIZE, 0), Quaternion.identity)*/;
        plantHistory.Add(Spawn, currentCycle);
        gameBoard[y,x] = Spawn;
    }

    /* To be called by a Plant, whenever it is extracted. */
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

    /* To be called by a Plant OR Pathogen object, whenever it kills a plant. */
    public void KillPlant(Plant debilitated)
    {
        gameBoard[debilitated.Y, debilitated.X] = null;
        plantCount--;
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
                if (remainingTime <= 0)
                {
                    AdvancePhase();
                }
                break;
            default:

                break;
        }
    }
}
