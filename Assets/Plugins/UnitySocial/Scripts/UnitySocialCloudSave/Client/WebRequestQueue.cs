using System.Collections.Generic;

namespace UnitySocialCloudSave.Client
{
    internal class WebRequestQueue
    {
        public static readonly WebRequestQueue Instance = new WebRequestQueue();

        private readonly Queue<object> _requests = new Queue<object>();

        private WebRequestQueue()
        {
        }

        public void Enqueue(object request)
        {
            _requests.Enqueue(request);
        }

        public object Dequeue()
        {
            return _requests.Count > 0 ? _requests.Dequeue() : null;
        }
    }
}