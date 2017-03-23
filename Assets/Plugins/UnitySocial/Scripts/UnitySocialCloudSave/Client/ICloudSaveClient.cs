namespace UnitySocialCloudSave.Client
{
    internal interface ICloudSaveClient
    {
        /// <summary>
        /// Pull records from server. Callback will be invoked when response come back.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        void PullRecordsAsync(PullRecordsRequest request, CloudSaveCallback<PullRecordsResponse> callback);


        /// <summary>
        /// Push records to remote server. Callback will be invoked when response come back.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        void PushRecordsAsync(PushRecordsRequest request, CloudSaveCallback<PushRecordsResponse> callback);
    }
}