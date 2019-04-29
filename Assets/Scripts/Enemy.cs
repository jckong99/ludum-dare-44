using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 0f;

    private static int idSource = 0;
    private Vector3 targetPosition, previousPosition;

    /// <summary>
    /// Unique Enemy ID number; IDs are assigned incrementally from 0.
    /// </summary>
    public int Id { get; private set; }

    private void Start()
    {
        Id = idSource;
        idSource++;
    }

    private void Update()
    {
        UpdateGridPosition();
    }

    // Moves Enemy in straight line towards intermedate targetPosition; updates relevant EnemyHorde references when necessary
    private void UpdateGridPosition()
    {
        GameManager gameManager = GameManager.GetInstance();
        IEntity entity;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if ((transform.position - previousPosition).sqrMagnitude >= Mathf.Pow(0.5f * GameManager.TILE_SIZE, 2))
        {
            int posX = (int)targetPosition.x;
            int posY = (int)targetPosition.y;
            if ((entity = gameManager.EntityAt(posX, posY)).GetTag() == Type.Plant)
            {
                gameManager.KillPlant(posX, posY);
            }
            ((EnemyHorde)entity).AddEnemy(this);
            if ((entity = gameManager.EntityAt(posX, posY)).GetTag() == Type.Horde)
            {
                ((EnemyHorde)entity).RemoveEnemy(this);
            }
        }
    }
}
