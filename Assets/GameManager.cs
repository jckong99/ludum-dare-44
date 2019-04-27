using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Phase { Day, Night, Dawn };
public enum Weather { Sun, Clouds, Rain };

public class GameManager : MonoBehaviour
{
    private static GameManager gameManager;

    private int Width, Height;
    private IEntity[,] GameBoard;

    /* GLOBAL STATS */
    /* The only one we should need to share with the user is the SeedCount. */
    private uint PlantCount;
    private uint ERPCount;
    private uint SeedCount;
    private int Hydration;
    private float Nutrients;
    private float RemainingTime;
    private const float RESET_TIME = 300;
    private uint CurrentCycle;
    private uint GoalCycle;
    public const float TILE_SIZE = 0.500f;


    private Phase CurrentPhase;
    private Weather CurrentWeather;

    public static GameManager GetInstance()
    {
        return gameManager;
    }

    public IEntity EntityAt(int x, int y) {
        return GameBoard[y, x];
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
    void Start()
    {
        Width = 10;
        Height = 10;
        GameBoard = new IEntity[Height, Width];

        /* Initialize at dawn. */
        CurrentPhase = Phase.Dawn;
        CurrentWeather = Weather.Sun;

        PlantCount = 0;
        ERPCount = 0;
        SeedCount = 1;
        Hydration = 10; // on a scale 0 - 20?
        Nutrients = 4.50f; // on a scale 0 - 10?
        RemainingTime = RESET_TIME;
    }

    /* To be called from the Button object */
    void AdvancePhase()
    {
        if (CurrentPhase == Phase.Dawn)
        {
            CurrentPhase = Phase.Day;
        } else if (CurrentPhase == Phase.Day)
        {
            CurrentPhase = Phase.Night;
        } /*else if (RemainingTime <= 0)
        {
            CurrentPhase = Phase.Dawn;
            RemainingTime = ResetTime;
        }*/ // should not need this...
    }

    /* To be called by a Plant, whenever it is extracted. */
    public void ExtractPlant(Plant extracted)
    {
        GameBoard[extracted.getY(), extracted.getX()] = null;
        PlantCount--;
        ERPCount++;

        /* TODO process the "Return," which means
         * increasing the Hydration and/or Nutrients globally. */
        Hydration = Mathf.Max(20, Hydration + extracted.DaysAlive);
        Nutrients += Mathf.Max(10.0, Nutrients + (0.1*extracted.DaysAlive));
    }

    /* To be called by a Plant OR Pathogen object, whenever it kills a plant. */
    public void KillPlant(Plant debilitated)
    {
        GameBoard[debilitated.getY(), debilitated.getX()] = null;
        PlantCount--;
    }

    // Update is called once per frame
    void Update()
    {
        switch (CurrentPhase)
        {
            case Phase.Dawn:

                break;
            case Phase.Day:
                
                break;
            case Phase.Night:
                RemainingTime -= Time.deltaTime;
                if (RemainingTime <= 0)
                {
                    AdvancePhase();
                }
                break;
            default:

                break;
        }
    }
}
