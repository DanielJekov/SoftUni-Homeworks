using System;
using System.Collections;
using System.Collections.Generic;

namespace CustomLinkedList
{
    public class LinkedList
    {
        public Node Head { get; set; }

        public Node Tail { get; set; }

        public bool IsReversed { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newHead"></param>
        public void AddFirst(Node newHead)
        {
            if (this.Head == null)
            {
                this.Head = newHead;
                this.Tail = newHead;
            }
            else
            {
                newHead.Next = this.Head;
                this.Head.Previous = newHead;
                this.Head = newHead;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newTail"></param>
        public void AddLast(Node newTail)
        {
            if (Tail == null)
            {
                this.Tail = newTail;
                this.Head = newTail;
            }
            else
            {
                newTail.Previous = this.Tail;
                this.Tail.Next = newTail;
                this.Tail = newTail;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Node RemoveFirst()
        {
            var oldHead = this.Head;
            this.Head = this.Head.Next;
            this.Head.Previous = null;
            return oldHead;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Node RemoveLast()
        {
            var oldTail = this.Tail;
            this.Tail = this.Tail.Previous;
            this.Tail.Next = null;
            return oldTail;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void ForEach(Action<Node> action)
        {
            Node currentNode = this.Head;
            while (currentNode != null)
            {
                action(currentNode);
                currentNode = currentNode.Next;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public void PrintList()
        {
            Node currentNode = this.Head;
            while (currentNode != null)
            {
                Console.WriteLine(currentNode.Value);
                currentNode = currentNode.Next;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ForEachPrintList()
        {
            this.ForEach(node => Console.WriteLine(node.Value));
        }

        /// <summary>
        /// 
        /// </summary>
        public void ReversePrintList()
        {
            Node currentNode = this.Tail;
            while (currentNode != null)
            {
                Console.WriteLine(currentNode.Value);
                currentNode = currentNode.Previous;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Node[] ToArray()
        {
            List<Node> list = new List<Node>();
            this.ForEach(node => list.Add(node));
            return list.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Remove(int value)
        {
            Node currentNode = this.Head;
            while (currentNode != null)
            {
                if (currentNode.Value == value)
                {
                    currentNode.Previous.Next = currentNode.Next;
                    currentNode.Next.Previous = currentNode.Previous;
                    return true;
                }
                currentNode = currentNode.Next;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(int value)
        {
            bool isFound = false;
            this.ForEach(node =>
            {
                if (node.Value == value)
                {
                    isFound = true;
                }
            });

            return isFound; 
        }

        public void Reverse()
        {
            var oldHead = this.Head;
            this.Head = this.Tail;
            this.Tail = this.Head;
        }
    }
}
