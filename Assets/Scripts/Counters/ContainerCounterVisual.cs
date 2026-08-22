using System;
using UnityEngine;

/// <summary>
/// Plays the lid open/close animation on the container counter every time the player
/// grabs an object out of it.
/// </summary>
[RequireComponent(typeof(Animator))]
public class ContainerCounterVisual : MonoBehaviour
{
    private static readonly int OpenCloseTrigger = Animator.StringToHash("OpenClose");

    [SerializeField] private ContainerCounter containerCounter;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        containerCounter.OnPlayerGrabbedObject += ContainerCounter_OnPlayerGrabbedObject;
    }

    private void OnDisable()
    {
        containerCounter.OnPlayerGrabbedObject -= ContainerCounter_OnPlayerGrabbedObject;
    }

    private void ContainerCounter_OnPlayerGrabbedObject(object sender, EventArgs e)
    {
        _animator.SetTrigger(OpenCloseTrigger);
    }
}
