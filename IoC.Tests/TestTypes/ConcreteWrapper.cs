namespace IoC.Tests.TestTypes
{
    public class ConcreteWrapper<T> : IWrapper<T>
    {
        private readonly IObject _logger;

        public ConcreteWrapper(IObject logger)
        {
            _logger = logger;
        }
    }
}
