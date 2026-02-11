namespace Pathfinding.Scripts
{
    using System.Collections.Generic;
    using System;

    public class PriorityQueue<T>
    {
        private struct Element
        {
            public T Item;
            public float Priority;
        }

        private List<Element> elements = new List<Element>();

        public int Count => elements.Count;

        public void Enqueue(T item, float priority)
        {
            elements.Add(new Element { Item = item, Priority = priority });
            int index = elements.Count - 1;
            
            while (index > 0)
            {
                int parentIndex = (index - 1) / 2;
                if (elements[index].Priority >= elements[parentIndex].Priority) break;

                Swap(index, parentIndex);
                index = parentIndex;
            }
        }

        public T Dequeue()
        {
            if (elements.Count == 0) throw new InvalidOperationException("Queue rỗng!");

            T bestItem = elements[0].Item;
            int lastIndex = elements.Count - 1;

            elements[0] = elements[lastIndex];
            elements.RemoveAt(lastIndex);
            lastIndex--;

            int index = 0;
            while (true)
            {
                int childIndex = index * 2 + 1; 
                if (childIndex > lastIndex) break;

                int rightChild = childIndex + 1; 
                if (rightChild <= lastIndex && elements[rightChild].Priority < elements[childIndex].Priority)
                {
                    childIndex = rightChild;
                }

                if (elements[index].Priority <= elements[childIndex].Priority) break;

                Swap(index, childIndex);
                index = childIndex;
            }

            return bestItem;
        }

        private void Swap(int i, int j)
        {
            var temp = elements[i];
            elements[i] = elements[j];
            elements[j] = temp;
        }

        public void Clear()
        {
            elements.Clear();
        }
    }
}