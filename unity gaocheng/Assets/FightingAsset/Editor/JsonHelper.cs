using UnityEngine;

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        string wrappedJson = "{ \"items\": " + json + "}";
        return JsonUtility.FromJson<Wrapper<T>>(wrappedJson).items;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] items;
    }
}