using System;
using UnityEngine;

/// <summary>
/// A plate dispenser: it grows a stack of clean plates on its own, one every few seconds,
/// and hands the top one to the player on interaction.
///
/// The waiting plates are only a count plus visuals; the plate becomes a real
/// KitchenObject the moment it is picked up.
/// </summary>
public class PlatesCounter : BaseCounter
{
    /// <summary>Raised when a new plate is added to the waiting stack.</summary>
    public event EventHandler OnPlateSpawned;

    /// <summary>Raised when the player takes the top plate off the stack.</summary>
    public event EventHandler OnPlateRemoved;

    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;
    [SerializeField] private float spawnPlateTimerMax = 4f;
    [SerializeField] private int platesSpawnedAmountMax = 4;

    private float _spawnPlateTimer;
    private int _platesSpawnedAmount;

    private void Update()
    {
        if (_platesSpawnedAmount >= platesSpawnedAmountMax)
        {
            // The stack is full, so the timer stays where it is until a plate is taken.
            return;
        }

        _spawnPlateTimer += Time.deltaTime;

        if (_spawnPlateTimer < spawnPlateTimerMax)
        {
            return;
        }

        _spawnPlateTimer = 0f;
        _platesSpawnedAmount++;

        OnPlateSpawned?.Invoke(this, EventArgs.Empty);
    }

    public override void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {
            // Hands are full, there is nowhere to put the plate.
            return;
        }

        if (_platesSpawnedAmount <= 0)
        {
            return;
        }

        _platesSpawnedAmount--;

        KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);

        OnPlateRemoved?.Invoke(this, EventArgs.Empty);
    }
}
