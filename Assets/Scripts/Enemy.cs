using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 0f;

    private GameManager gameManager;
    private static int idSource = 0;
    private Vector3 target, previous, intermediate;
    private bool crossedTileLine = false, moving = true;

    /// <summary>
    /// Unique Enemy ID number; IDs are assigned incrementally from 0.
    /// </summary>
    public int Id { get; private set; }

    private void Awake()
    {
        gameManager = GameManager.GetInstance();
    }

    private void Start()
    {
        Id = idSource;
        idSource++;

        previous = transform.position;
        target = gameManager.GetNearestPlantPosition((int)previous.x, (int)previous.y);
        if (target.x < 0 || target.y < 0 || target.z < 0)
        {
            moving = false;
        }
        SetIntermediateTarget();
    }

    private void Update()
    {
        if (moving)
        {
            UpdateGridPosition();
        }
    }

    /// <summary>
    /// Set final destination of this Enemy in terms of gameBoard array coordinates.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void SetTarget(int x, int y)
    {
        target.x = x;
        target.y = y;
    }

    // Determine intermediate target to move to based on current position and target; returns previous intermediate
    private Vector3 SetIntermediateTarget()
    {
        Vector3 oldIntermediate = intermediate;

        /*if (Mathf.Abs(target.x - previous.x) >= Mathf.Abs(target.y - previous.y))
        {
            intermediate = new Vector3(previous.x + (target.x - previous.x > 0 ? 1 : -1), previous.y, 0);
        }
        else
        {
            intermediate = new Vector3(previous.x, previous.y + (target.y - previous.y > 0 ? 1 : -1), 0);
        }*/
        float newX = previous.x + (target.x - previous.x > 0 ? 1 : -1);
        float newY = previous.y + (target.y - previous.y > 0 ? 1 : -1);

        if (Mathf.Abs(target.x - previous.x) > Mathf.Abs(target.y - previous.y))
        {
            intermediate = new Vector3(newX, previous.y, 0);
        }
        else if (Mathf.Abs(target.x - previous.x) < Mathf.Abs(target.y - previous.y))
        {
            intermediate = new Vector3(previous.x, newY, 0);
        }
        else
        {
            intermediate = (Random.Range(0f, 1f) < 0.5 ? new Vector3(newX, previous.y, 0) : new Vector3(previous.x, newY, 0));
        }
        Debug.Log("\noldIntermediate: " + oldIntermediate + ", intermediate: " + intermediate + ", target: " + target);

        return oldIntermediate;
    }

    // Moves Enemy in straight line towards intermedate target; updates relevant EnemyHorde references when necessary
    private void UpdateGridPosition()
    {
        IEntity entity;

        transform.position = Vector3.MoveTowards(transform.position, intermediate, moveSpeed * Time.deltaTime);

        if (!crossedTileLine && (transform.position - previous).sqrMagnitude >= Mathf.Pow(0.5f * GameManager.TILE_SIZE, 2))
        {
            int posX = (int)intermediate.x;
            int posY = (int)intermediate.y;
            int prevX = (int)previous.x;
            int prevY = (int)previous.y;

            if (gameManager.EntityAt(posX, posY).GetTag() == Type.Plant)
            {
                gameManager.KillPlant(posX, posY);
            }
            ((EnemyHorde)gameManager.EntityAt(posX, posY)).AddEnemy(this);
            if ((entity = gameManager.EntityAt(prevX, prevY)).GetTag() == Type.Horde)
            {
                ((EnemyHorde)gameManager.EntityAt(prevX, prevY)).RemoveEnemy(this);
            }

            crossedTileLine = true;
        }

        if (transform.position == intermediate)
        {
            if (transform.position == target)
            {
                target = gameManager.GetNearestPlantPosition((int)previous.x, (int)previous.y);
                if (target.x < 0 || target.y < 0 || target.z < 0)
                {
                    moving = false;
                }
            }

            previous = SetIntermediateTarget();
            Debug.Log("previous: " + previous);

            crossedTileLine = false;
        }
    }
}
