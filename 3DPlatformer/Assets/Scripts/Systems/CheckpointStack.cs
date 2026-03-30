using System;
using UnityEngine;

namespace Platformer
{
    public class CheckpointStack<T>
    {
        private class Node
        {
            public T Value;
            public Node Next;

            public Node(T value, Node next)
            {
                Value =  value;
                Next = next;
            }
        }

        private Node top;
        public int Count { get; private set; }
        public bool IsEmpty => Count == 0;

        public void Push(T item)
        {
            top = new Node(item, top);
            Count++;
        }
        
        //Normal respawn
        public T Peek()
        {
            return top.Value;
        }
        
        //Revert to previous checkpoint
        public T Pop()
        {
            T value = top.Value;
            top = top.Next;
            Count--;
            return value;
        }

        public void ReplaceTop(T item)
        {
            if (IsEmpty)
            {
                Push(item);
                return;
            }
            top.Value = item;
        }        

        public void Clear()
        {
            top = null;
            Count = 0;
        }
        
    }
}





