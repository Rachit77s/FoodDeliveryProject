namespace FoodDeliveryPolaris.Exceptions;

/// <summary>
/// Exception thrown when attempting to create an entity that already exists.
/// </summary>
public class DuplicateEntityException : Exception
{
    public DuplicateEntityException(string message) : base(message)
    {
    }

    public DuplicateEntityException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}
