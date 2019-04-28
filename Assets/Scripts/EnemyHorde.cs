using System.Collections.Generic;

public class EnemyHorde : Entity
{
    private Dictionary<int, Enemy> horde = new Dictionary<int, Enemy>();

    private void Start()
    {

    }

    /// <summary>
    /// Adds specified Enemy to internal Dictionary, mapping ID to Enemy object.
    /// </summary>
    /// <param name="enemy"></param>
    public void AddEnemy(Enemy enemy)
    {
        horde.Add(enemy.Id, enemy);
    }

    /// <summary>
    /// Removes Enemy from internal Dictionary.
    /// </summary>
    /// <param name="enemy"></param>
    public void RemoveEnemy(Enemy enemy)
    {
        horde.Remove(enemy.Id);
    }

    public Type getTag()
    {
        return Type.Horde;
    }
}
