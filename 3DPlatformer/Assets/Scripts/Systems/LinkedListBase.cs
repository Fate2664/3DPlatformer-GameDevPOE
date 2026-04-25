using UnityEngine;

namespace Platformer
{
    public class Node<T>
    {
        public T Data { get; set; }
        public Node<T> Next { get; internal set; }

        public Node(T data)
        {
            this.Data = data;
        }
    }
    
    public class LinkedListBase<T>
    {
        public Node<T> First { get; private set; }
        public Node<T> Last { get; private set; }
        public int Count { get; private set; }

        public void AddFirst(Node<T> newNode)
        {
            //Check if list is empty
            if (this.First == null)
            {
                this.First = newNode;
                this.Last = newNode;
            }
            else
            {
                newNode.Next = this.First;
                this.First = newNode;
            }
            this.Count++;
        }

        public void AddLast(Node<T> newNode)
        {
            //Check if list is empty
            if (this.First == null)
            {
                this.First = newNode;
                this.Last = newNode;
            }
            else
            {
                this.Last.Next = newNode;
                Last = newNode;
            }
            Count++;
        }

        public void AddAfter(Node<T> newNode, Node<T> existingNode)
        {
            //Check if existing not is last so we don't add after last node
            if (this.Last == existingNode) 
                Last = newNode;
            
            newNode.Next = existingNode.Next;
            existingNode.Next = newNode;
            this.Count++;
        }

        public Node<T> Find(T target)
        {
            Node<T> currentNode = First;
            while (currentNode != null && !currentNode.Data.Equals(target))
            {
                currentNode = currentNode.Next;
            }
            return currentNode;
        }

        public void RemoveFirst()
        {
            if (First == null || this.Count == 0)
                return;
            
            First = First.Next;
            this.Count--;
        }

        public void Remove(Node<T> removingNode)
        {
            if (First == null || this.Count == 0)
                return;

            if (this.First == removingNode)
            {
                this.RemoveFirst();
                return;
            }

            Node<T> previous = First;
            Node<T> current = previous.Next;

            while (current != null && current != removingNode)
            {
                //move to the next node
                previous = current;
                current = previous.Next;
            }
            
            //remove it
            if (current != null)
            {
                previous.Next = current.Next;
                this.Count--;
            }
        }
        
    }
    
}
