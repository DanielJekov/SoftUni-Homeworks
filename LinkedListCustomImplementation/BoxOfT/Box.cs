using System;
using System.Collections.Generic;

namespace BoxOfT
{
    public class Box<T>
    {
        private Queue<T> elements;

        public Box()
        {
            this.elements = new Queue<T>();
        }

        public int Count => elements.Count;

        public void Add(T element)
        {
            elements.Enqueue(element);
        }

        public T Remove()
        {
            return elements.Dequeue();
        }

        public void PrintAll()
        {
            while (elements.Count != 0)
            {
                Console.WriteLine(elements.Dequeue());
            }
        }
    }
}
