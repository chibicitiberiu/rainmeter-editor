using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RainmeterEditor.Model
{
    public class Tree<T>
    {
        [XmlElement("data")]
        public T Data { get; set; }

        [XmlArray("children"), XmlArrayItem("child")]
        public List<Tree<T>> Children { get; set; }

        public Tree()
        {
            Children = new List<Tree<T>>();
            Data = default(T);
        }

        public Tree(T data)
        {
            Children = new List<Tree<T>>();
            Data = data;
        }

        public int IndexOf(Tree<T> item)
        {
            return Children.IndexOf(item);
        }

        public void Insert(int index, Tree<T> item)
        {
            Children.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            Children.RemoveAt(index);
        }

        public Tree<T> this[int index]
        {
            get
            {
                return Children[index];
            }
            set
            {
                Children[index] = value;
            }
        }

        public void Add(Tree<T> item)
        {
            Children.Add(item);
        }

        public void Clear()
        {
            Children.Clear();
        }

        public bool Contains(Tree<T> item)
        {
            return Children.Contains(item);
        }

        public void CopyTo(Tree<T>[] array, int arrayIndex)
        {
            Children.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return Children.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(Tree<T> item)
        {
            return Children.Remove(item);
        }

        public IEnumerator<Tree<T>> GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return Children.IndexOf(new Tree<T>(item));
        }

        public void Insert(int index, T item)
        {
            Children.Insert(index, new Tree<T>(item));
        }

        public void Add(T item)
        {
            Children.Add(new Tree<T>(item));
        }

        public bool Contains(T item)
        {
            return Children.Contains(new Tree<T>(item));
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            foreach (var node in Children)
                array[arrayIndex++] = node.Data;
        }

        public bool Remove(T item)
        {
            return Children.Remove(new Tree<T>(item));
        }

        public override bool Equals(object obj)
        {
            Tree<T> other = obj as Tree<T>;
            
            // Types are different, so not equal
            if (other == null)
                return false;

            // Compare data
            if (!object.Equals(Data, other.Data))
                return false;

            // Compare children array
            return Children.SequenceEqual(other.Children);
        }

        public override int GetHashCode()
        {
            int hash = ((Data == null) ? 0 : Data.GetHashCode());

            foreach (var c in Children)
                hash = hash * 7 + c.GetHashCode();

            return hash;
        }
    }
}
