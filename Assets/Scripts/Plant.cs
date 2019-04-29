using UnityEngine;

public class Plant : MonoBehaviour, IEntity
{
    public int X { get; set; }
    public int Y { get; set; }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public Type GetTag()
    {
        return Type.Plant;
    }
}
