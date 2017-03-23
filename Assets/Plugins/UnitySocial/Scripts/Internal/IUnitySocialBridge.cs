namespace UnitySocial
{
namespace Internal
{
    internal interface IUnitySocialBridge
    {
        void SendMessage(string message);
        void SendMessage(string message, object content);
    }
}
}
