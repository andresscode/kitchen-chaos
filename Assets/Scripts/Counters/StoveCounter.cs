using System;
using UnityEngine;

/// <summary>
/// Fries whatever is placed on it, one recipe stage at a time: a raw patty becomes
/// cooked, a cooked patty left alone becomes burned. Each stage is a separate
/// FryingRecipeSO, chained by feeding the previous stage's output back in as input.
///
/// Progress is kept on the object itself (see FryableProgress), so a part-cooked
/// patty can be carried away and finished later, on this stove or another one.
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

        // The freshly spawned object carries no progress, so the next stage starts at 0.
        StartCooking();
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
            StartCooking();

            return;
        }

        if (player.HasKitchenObject())
        {
            Debug.Log($"{name} already holds {GetKitchenObject().GetKitchenObjectSO().objectName}");
            return;
        }

        // Player picks up what sits on this counter, cooked, raw or burned.
        SaveProgressToKitchenObject();
        GetKitchenObject().SetKitchenObjectParent(player);
        StopCooking();
    }

    public State GetState()
    {
        return _state;
    }

    /// <summary>
    /// Arms the timer for the object currently on the counter, resuming from whatever
    /// progress that object was carrying, or stops cooking when it has no further recipe.
    /// </summary>
    private void StartCooking()
    {
        KitchenObject kitchenObject = GetKitchenObject();
        KitchenObjectSO input = kitchenObject.GetKitchenObjectSO();

        _fryingTimer = 0f;
        SetState(ResolveState(input));

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

        if (kitchenObject.TryGetComponent(out FryableProgress fryable))
        {
            _fryingTimer = fryable.ProgressNormalized * _fryingRecipe.fryingTimerMax;
        }

        NotifyProgressChange(_fryingTimer / _fryingRecipe.fryingTimerMax);
    }

    private void StopCooking()
    {
        _fryingRecipe = null;
        _fryingTimer = 0f;
        SetState(State.Idle);
        NotifyProgressChange(0f);
    }

    /// <summary>
    /// Hands the in-flight timer back to the object so it can be resumed elsewhere.
    /// </summary>
    private void SaveProgressToKitchenObject()
    {
        if (_fryingRecipe == null)
        {
            return;
        }

        if (GetKitchenObject().TryGetComponent(out FryableProgress fryable))
        {
            fryable.ProgressNormalized = _fryingTimer / _fryingRecipe.fryingTimerMax;
        }
    }

    /// <summary>
    /// Works out which stage of the chain an object sits at, purely from the recipe data,
    /// so a part-cooked object placed on a cold stove lands in the right state.
    /// </summary>
    private State ResolveState(KitchenObjectSO input)
    {
        if (TryGetFryingRecipe(input, out FryingRecipeSO recipe))
        {
            // Something comes after this stage's output, so this is still the frying
            // stage; otherwise this is the last stage and the object is about to burn.
            return TryGetFryingRecipe(recipe.output, out FryingRecipeSO _) ? State.Frying : State.Fried;
        }

        // No recipe takes this object as input: it is either the burned end of a chain
        // or something that was never fryable to begin with.
        return IsFryingOutput(input) ? State.Burned : State.Idle;
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

    private bool IsFryingOutput(KitchenObjectSO kitchenObjectSO)
    {
        foreach (FryingRecipeSO candidate in fryingRecipeSOArray)
        {
            if (candidate != null && candidate.output == kitchenObjectSO)
            {
                return true;
            }
        }

        return false;
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
