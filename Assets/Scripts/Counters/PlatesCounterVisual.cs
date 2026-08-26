using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shows the plates waiting on the PlatesCounter as a physical stack: one visual is
/// added on top for every plate spawned, and the top one is removed when a plate is
/// picked up. These are cosmetic only, the real plate KitchenObject is spawned by
/// PlatesCounter straight into the player's hands.
/// </summary>
public class PlatesCounterVisual : MonoBehaviour
{
    [SerializeField] private PlatesCounter platesCounter;
    [SerializeField] private Transform counterTopPoint;
    [SerializeField] private Transform plateVisualPrefab;
    [SerializeField] private float plateOffsetY = 0.1f;

    private readonly List<Transform> _plateVisualTransformList = new();

    private void OnEnable()
    {
        platesCounter.OnPlateSpawned += PlatesCounter_OnPlateSpawned;
        platesCounter.OnPlateRemoved += PlatesCounter_OnPlateRemoved;
    }

    private void OnDisable()
    {
        platesCounter.OnPlateSpawned -= PlatesCounter_OnPlateSpawned;
        platesCounter.OnPlateRemoved -= PlatesCounter_OnPlateRemoved;
    }

    private void PlatesCounter_OnPlateSpawned(object sender, EventArgs e)
    {
        Transform plateVisualTransform = Instantiate(plateVisualPrefab, counterTopPoint);

        plateVisualTransform.localPosition = new Vector3(0f, plateOffsetY * _plateVisualTransformList.Count, 0f);

        _plateVisualTransformList.Add(plateVisualTransform);
    }

    private void PlatesCounter_OnPlateRemoved(object sender, EventArgs e)
    {
        if (_plateVisualTransformList.Count == 0)
        {
            return;
        }

        int topIndex = _plateVisualTransformList.Count - 1;
        Transform plateVisualTransform = _plateVisualTransformList[topIndex];

        _plateVisualTransformList.RemoveAt(topIndex);
        Destroy(plateVisualTransform.gameObject);
    }
}
