using System.Collections.Generic;

public class Utils
{
    public const string tagBackground = "Circular_Background";
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
