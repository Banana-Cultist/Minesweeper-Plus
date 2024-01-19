using System;

public class PriorityQueue<T> where T : IComparable
{
    private T[] data;
    private int maxSize = 8;
    private int size = 0;

    public PriorityQueue()
    {
        data = new T[maxSize];
    }

    public void Insert(T value)
    {
        // insert new value at end of list
        if (size > maxSize / 2)
        {
            Expand();
        }
        data[size] = value;
        int index = size;

        // swim new value until valid
        while (data[index].CompareTo(data[(index - 1) / 2]) < 0)
        {
            Swap(ref data[(index - 1) / 2], ref data[index]);
            index = (index - 1) / 2;
        }
        size++;
    }

    public T Pull()
    {
        // pop root node and replace with last node
        T value = data[0];
        int index = 0;
        data[index] = data[--size];

        // sink new root node until valid
        while (true)
        {
            int index1 = (index * 2) + 1;
            int index2 = (index * 2) + 2;
            bool invalid1 = index1 >= size || data[index1] == null;
            bool invalid2 = index2 >= size || data[index2] == null;

            int greatestChildIndex;
            if (invalid1 && invalid2) break;
            if (invalid1) greatestChildIndex = index2;
            else if (invalid2) greatestChildIndex = index1;
            else greatestChildIndex = data[index1].CompareTo(data[index2]) < 0 ? index1 : index2;

            if (data[index].CompareTo(data[greatestChildIndex]) > 0)
            {
                Swap(ref data[greatestChildIndex], ref data[index]);
                index = greatestChildIndex;
            }
            else
            {
                break;
            }
        }

        return value;
    }

    private void Swap(ref T a, ref T b)
    {
        (a, b) = (b, a);
    }

    public void Clear()
    {
        size = 0;
    }

    public int Length()
    {
        return size;
    }

    private void Expand()
    {
        maxSize *= 2;
        T[] newData = new T[maxSize];
        data.CopyTo(newData, 0);
        data = newData;
    }
}
