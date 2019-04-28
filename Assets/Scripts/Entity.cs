using UnityEngine;

public enum Type { Plant, Horde };

public abstract class Entity : MonoBehaviour
{
    public new Type tag;
}
