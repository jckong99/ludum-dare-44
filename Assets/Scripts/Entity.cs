using UnityEngine;

public enum Type { Plant, Horde };

public abstract class Entity : MonoBehaviour
{
    public Type tag;
}
