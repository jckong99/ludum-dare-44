﻿using UnityEngine;

public class Plant : MonoBehaviour, IEntity
{
    [SerializeField] private ParticleSystem deathEffect1 = null;
    [SerializeField] private ParticleSystem deathEffect2 = null;

    public uint X { get; set; }
    public uint Y { get; set; }

    public Type GetTag()
    {
        return Type.Plant;
    }

    private void OnDisable()
    {
        Instantiate(deathEffect1, transform.position, Quaternion.identity);
        Instantiate(deathEffect2, transform.position, Quaternion.identity);
    }
}
