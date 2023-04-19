using static VoronoiBoard;
using UnityEngine;
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
            expand();
        }
        data[size] = value;
        int index = size;

        // swim new value until valid
        while (data[index].CompareTo(data[(index - 1) / 2]) < 0)
        {
            T temp = data[index];
            data[index] = data[(index - 1) / 2];
            data[(index - 1) / 2] = temp;
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

            //T? child1 = (index * 2) + 1 < size ? data[(index * 2) + 1] : null;
            //T? child2 = (index * 2) + 2 < size ? data[(index * 2) + 2] : null;
            int index1 = (index * 2) + 1;
            int index2 = (index * 2) + 2;
            bool valid1 = index1 >= size || data[index1] == null;
            bool valid2 = index2 >= size || data[index2] == null;
            int greatestChildIndex; // = child1.CompareTo(child2) < 0 ? (index * 2) + 1 : (index * 2) + 2;
            if (valid1 && valid2)
                break;
            else if (valid1)
                greatestChildIndex = index2;
            else if (valid2)
                greatestChildIndex = index1;
            else
                greatestChildIndex = data[index1].CompareTo(data[index2]) < 0 ? index1 : index2;
            if (data[index].CompareTo(data[greatestChildIndex]) > 0)
            {
                T temp = data[index];
                data[index] = data[greatestChildIndex];
                data[greatestChildIndex] = temp;
                index = greatestChildIndex;
            }
            else
            {
                break;
            }
        }

        return value;
    }



    public void clear()
    {
        size = 0;
    }

    public int Length()
    {
        return size;
    }

    private void expand()
    {
        maxSize *= 2;
        T[] newData = new T[maxSize];
        data.CopyTo(newData, 0);
        data = newData;
    }
}
