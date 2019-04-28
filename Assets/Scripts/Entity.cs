using UnityEngine;

public enum Type { Plant, Horde };

public interface Entity
{
    Type getTag();
}
