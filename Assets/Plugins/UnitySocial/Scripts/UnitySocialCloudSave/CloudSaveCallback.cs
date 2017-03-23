using System;

namespace UnitySocialCloudSave
{
    public delegate void CloudSaveCallback<TResult>(Exception err, TResult result);
}