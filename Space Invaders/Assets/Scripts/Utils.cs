﻿using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public const string TagBackground = "Circular_Background";
    public const string TagPlayer = "Player";
    public const string TagAstroid = "Astroid";

    private static Vector3 asteroidDirection;

    // returns the game's radius, though gameBackground should be the Circular_Background
    public static float getGameBoundaryRadius(GameObject gameBackground)
    {
        if (gameBackground == null) { Debug.LogError("(" + typeof(Utils).Name + "): No game background component provided."); throw new MissingComponentException(); } 
        SphereCollider bgSphere = gameBackground.GetComponent<SphereCollider>();
        if (bgSphere == null) { Debug.LogError("(" + typeof(Utils).Name + "): Missing SphereCollider for bg component."); throw new MissingComponentException(); }

        float scale = getUniformScale(gameBackground);
        float localRadius = bgSphere.radius;
        return scale * localRadius;
    }
    public static float getBackgroundRadius(GameObject gameBackground)
    {
        return getUniformScale(gameBackground) / 2.0f;
    }

    public static void setAsteroidDirection(Vector3 direction)
    {
        asteroidDirection = direction.normalized;
    }
    public static Vector3 getAsteroidDirection() => asteroidDirection;
    public static Vector3 getRandomDirection()=> new Vector3(
            (Random.value + 0.01f) * (Random.value < 0.5 ? 1 : -1),
            (Random.value + 0.01f) * (Random.value < 0.5 ? 1 : -1),
            (Random.value + 0.01f) * (Random.value < 0.5 ? 1 : -1)
        ).normalized;

    [System.Serializable]
    public class NonEqualScaleProvidedException : System.Exception
    {
        public NonEqualScaleProvidedException() { }
        public NonEqualScaleProvidedException(string message) : base(message) { }
        public NonEqualScaleProvidedException(string message, System.Exception inner) : base(message, inner) { }
        protected NonEqualScaleProvidedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    private static float getUniformScale(GameObject _object)
    {
        if (_object == null) { Debug.LogError("(class=" + typeof(Utils).Name + ", GameObject=" + _object.name + "): No _object component provided."); throw new MissingComponentException(); }
        float x = _object.transform.lossyScale.x;
        float y = _object.transform.lossyScale.y;
        float z = _object.transform.lossyScale.z;
        if (x != y || x != z) throw new NonEqualScaleProvidedException("(class=" + typeof(Utils).Name + ", GameObject=" + _object.name + "): x = " + x + ", y = " + y + ", z = " + z);
        float scale = x;
        return scale;
    }
}

public class ImmutableDoublyLinkedList<T>
{
    private int currentItem;
    private readonly int itemsSize;
    private List<T> list;

    public ImmutableDoublyLinkedList(int index = 0, params T[] items)
    {
        list = new List<T>();
        currentItem = index;
        itemsSize = items.Length;
        for (int i = 0; i < itemsSize; ++i)
        {
            list.Add(items[i]);
        }
    }

    public T GetValue()
    {
        return list[currentItem];
    }

    public void Next()
    {
        ++currentItem;
        if (currentItem == itemsSize)
        {
            currentItem = 0;
        }
    }

}
