using ShufflerPro.Client.Interfaces;

namespace ShufflerPro.Screens.Exceptions;

public interface IExceptionViewModelFactory : IFactory
{
    ExceptionViewModel Create(Exception exception);
}