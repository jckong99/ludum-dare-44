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

        if ((transform.position - previousPosition).sqrMagnitude >= Mathf.Pow(0.5 * gameManager.TILE_SIZE, 2))
        {
            if ((entity = gameManager.EntityAt(targetPosition.x, targetPosition.y)) is EnemyHorde)
                ((EnemyHorde)entity).AddEnemy(this);
            if ((entity = gameManager.EntityAt(previousPosition.x, previousPosition.y)) is EnemyHorde)
                ((EnemyHorde)entity).RemoveEnemy(this);
        }
    }
}
