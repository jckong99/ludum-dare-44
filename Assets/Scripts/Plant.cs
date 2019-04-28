using UnityEngine;

public class Plant : MonoBehaviour, Entity
{
    public int X { get; set; }
    public int Y { get; set; }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public Type getTag()
    {
        return Type.Plant;
    }
}
