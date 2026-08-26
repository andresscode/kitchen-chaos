using System;

/// <summary>
/// Implemented by anything that drives a ProgressBarUI (cutting, frying, ...).
/// </summary>
public interface IHasProgress
{
    event EventHandler<OnProgressChangedEventArgs> OnProgressChanged;

    class OnProgressChangedEventArgs : EventArgs
    {
        public float progressNormalized;
    }
}
