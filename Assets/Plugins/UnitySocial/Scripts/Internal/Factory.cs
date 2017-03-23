namespace UnitySocial
{
namespace Internal
{
    internal static class Factory
    {
        public static IQuickMatch quickMatch { get; private set; }
        public static CallbackManager callbackManager { get; private set; }

        public static void Initialize(IQuickMatch quickMatch)
        {
            Factory.quickMatch = quickMatch;

            Factory.callbackManager = new CallbackManager();
        }
    }
}
}
