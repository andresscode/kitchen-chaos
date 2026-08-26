using System;
using UnityEngine;

/// <summary>
/// Fries whatever is placed on it, one recipe stage at a time: a raw patty becomes
/// cooked, a cooked patty left alone becomes burned. Each stage is a separate
/// FryingRecipeSO, chained by feeding the previous stage's output back in as input.
/// </summary>
public class StoveCounter : BaseCounter, IHasProgress
{
    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned
    }

    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;

    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }

    private State _state = State.Idle;
    private FryingRecipeSO _fryingRecipe;
    private float _fryingTimer;

    private void Update()
    {
        // A null recipe means nothing is cooking: the counter is empty, holds something
        // unfryable, or the object has reached the end of its recipe chain.
        if (_fryingRecipe == null)
        {
            return;
        }

        _fryingTimer += Time.deltaTime;

        NotifyProgressChange(_fryingTimer / _fryingRecipe.fryingTimerMax);

        if (_fryingTimer < _fryingRecipe.fryingTimerMax)
        {
            return;
        }

        KitchenObjectSO output = _fryingRecipe.output;

        GetKitchenObject().DestroySelf();
        KitchenObject.SpawnKitchenObject(output, this);

        // Frying -> Fried -> Burned. Anything past Fried has nowhere left to go.
        SetState(_state == State.Frying ? State.Fried : State.Burned);
        StartCooking(output);
    }

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            if (!player.HasKitchenObject())
            {
                return;
            }

            KitchenObjectSO input = player.GetKitchenObject().GetKitchenObjectSO();

            if (!TryGetFryingRecipe(input, out FryingRecipeSO _))
            {
                Debug.Log($"{input.objectName} cannot be fried");
                return;
            }

            // Player drops what they carry onto this counter.
            player.GetKitchenObject().SetKitchenObjectParent(this);
            SetState(State.Frying);
            StartCooking(input);

            return;
        }

        if (player.HasKitchenObject())
        {
            Debug.Log($"{name} already holds {GetKitchenObject().GetKitchenObjectSO().objectName}");
            return;
        }

        // Player picks up what sits on this counter, cooked, raw or burned.
        GetKitchenObject().SetKitchenObjectParent(player);
        StopCooking();
    }

    /// <summary>
    /// Arms the timer for the next stage of <paramref name="input"/>, or stops
    /// cooking when that object has no further recipe.
    /// </summary>
    private void StartCooking(KitchenObjectSO input)
    {
        _fryingTimer = 0f;

        if (!TryGetFryingRecipe(input, out _fryingRecipe))
        {
            NotifyProgressChange(0f);
            return;
        }

        if (_fryingRecipe.fryingTimerMax <= 0f)
        {
            Debug.LogError($"{_fryingRecipe.input.objectName} fryingTimerMax must be greater than 0");
            _fryingRecipe = null;
            NotifyProgressChange(0f);
            return;
        }

        NotifyProgressChange(0f);
    }

    private void StopCooking()
    {
        _fryingRecipe = null;
        _fryingTimer = 0f;
        SetState(State.Idle);
        NotifyProgressChange(0f);
    }

    private bool TryGetFryingRecipe(KitchenObjectSO input, out FryingRecipeSO recipe)
    {
        foreach (FryingRecipeSO candidate in fryingRecipeSOArray)
        {
            if (candidate != null && candidate.input == input)
            {
                recipe = candidate;
                return true;
            }
        }

        recipe = null;
        return false;
    }

    public State GetState()
    {
        return _state;
    }

    private void SetState(State newState)
    {
        _state = newState;

        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
        {
            state = _state
        });
    }

    private void NotifyProgressChange(float newValue)
    {
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = newValue
        });
    }
}
