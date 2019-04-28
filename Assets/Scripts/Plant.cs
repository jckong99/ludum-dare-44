using UnityEngine;

public class Plant : Entity
{
    public int X { get; set; }
    public int Y { get; set; }

    /* NOW OBSOLETE because the days alive are managed by
     * the plantHistory data structure in GameManager. */
/*  public int DaysAlive { get; set; }*/

    private void Start()
    {
        this.tag = Type.Plant;
    }

    private void Update()
    {
        
    }
}
