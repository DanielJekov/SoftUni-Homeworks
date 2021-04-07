using System;

namespace CustomArray
{
    public class Array
    {

        public Node First { get; set; }
        public Node Last { get; set; }

        public void AddFirst(Node newHead)
        {
            if (this.First == null)
            {
                this.First = newHead;
                this.Last = newHead;
            }
            else 
            {
                newHead.Next = this.First;
                this.First.Previous = newHead;
                this.First = newHead;
            }
        }
    }
}
