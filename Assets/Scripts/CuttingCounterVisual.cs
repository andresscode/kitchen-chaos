using System;
using UnityEngine;

/// <summary>
/// Plays the cutting animation every time the player presets
/// the interact alternative button.
/// </summary>
[RequireComponent(typeof(Animator))]
public class CuttingCounterVisual: MonoBehaviour
{
    private static readonly int CUT = Animator.StringToHash("Cut");

    [SerializeField] private CuttingCounter cuttingCounter;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        cuttingCounter.OnCut += CuttingContainer_OnCut;
    }

    private void OnDisable()
    {
        cuttingCounter.OnCut -= CuttingContainer_OnCut;
    }

    private void CuttingContainer_OnCut(object sender, EventArgs e)
    {
        _animator.SetTrigger(CUT);
    }
}
