using System.Collections.Generic;
using System.Net;

namespace UnitySocialCloudSave.Client
{
    internal class SuperAgent
    {
        public string AgentName { get; private set; }

        private SuperAgent()
        {
            AgentName = AssemblyInfo.Title + "/" + AssemblyInfo.Version;
        }

        public static SuperAgentRequest Get(string url)
        {
            return new SuperAgentRequest(new SuperAgent(), "GET", url);
        }

        public static SuperAgentRequest Post(string url)
        {
            return new SuperAgentRequest(new SuperAgent(), "POST", url);
        }

        public static SuperAgentRequest Delete(string url)
        {
            return new SuperAgentRequest(new SuperAgent(), "DELETE", url);
        }
    }

    internal class SuperAgentRequest
    {
        public SuperAgent SuperAgent { get; private set; }

        public string Method { get; private set; }

        public string Url { get; private set; }

        private readonly List<KeyValuePair<string, string>> _queries = new List<KeyValuePair<string, string>>();

        public List<KeyValuePair<string, string>> Queries
        {
            get { return _queries; }
        }

        private readonly Dictionary<string,string> _headers = new Dictionary<string,string>();

        public Dictionary<string, string> Headers
        {
            get { return _headers; }
        }

        public byte[] Payload { get; private set; }

        public CloudSaveCallback<SuperAgentResponse> Callback { get; private set; }

        public SuperAgentRequest(SuperAgent superAgent, string method, string url)
        {
            SuperAgent = superAgent;
            Method = method;
            Url = url;
        }

        public SuperAgentRequest Query(string key, string value)
        {
            _queries.Add(new KeyValuePair<string, string>(key, value));
            return this;
        }

        public SuperAgentRequest Set(string key, string value)
        {
            _headers[key.ToLowerInvariant()] = value;
            return this;
        }

        public SuperAgentRequest Send(byte[] payload)
        {
            Payload = payload;
            return this;
        }

        public void End(CloudSaveCallback<SuperAgentResponse> callback)
        {
            Callback = callback;
            WebRequestQueue.Instance.Enqueue(this);
        }
    }

    internal class SuperAgentResponse
    {
        public HttpStatusCode HttpStatusCode { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public byte[] Body { get; set; }
    }
}