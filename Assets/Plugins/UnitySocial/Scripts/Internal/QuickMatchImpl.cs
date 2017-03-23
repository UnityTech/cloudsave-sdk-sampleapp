namespace UnitySocial
{
namespace Internal
{
    internal class QuickMatchImpl : IQuickMatch
    {
        private const string kStartQuickMatchEventName = "startQuickMatch";

        private IUnitySocialBridge m_UnitySocialBridge;

        public QuickMatchImpl(IUnitySocialBridge unitySocialBridge)
        {
            m_UnitySocialBridge = unitySocialBridge;
        }

        #region IQuickMatch
        public void Start()
        {
            m_UnitySocialBridge.SendMessage(kStartQuickMatchEventName);
        }

        public void Start(string challengeTemplateId)
        {
            m_UnitySocialBridge.SendMessage(kStartQuickMatchEventName,
                new QuickMatchStartContent { templateId = challengeTemplateId });
        }

        #endregion

        public struct QuickMatchStartContent
        {
            [Tools.JsonProperty("template_id")]
            public string templateId;
        }
    }
}
}
