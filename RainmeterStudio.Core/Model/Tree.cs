using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RainmeterStudio.Core.Model
{
    public class Tree<T> : IList<Tree<T>>
    {
        public T Data { get; set; }

        public ObservableCollection<Tree<T>> Children { get; private set; }

        public Tree()
        {
            Children = new ObservableCollection<Tree<T>>();
            Data = default(T);
        }

        public Tree(T data)
        {
            Children = new ObservableCollection<Tree<T>>();
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

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        public void TreeExpand(bool p)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Extension methods for trees
    /// </summary>
    public static class TreeExtensions
    {
        /// <summary>
        /// Tree traversal orders
        /// </summary>
        public enum TreeTraversalOrder
        {
            BreadthFirst,
            DepthFirst,
            DepthFirstPreOrder = DepthFirst,
            DepthFirstPostOrder
        }

        /// <summary>
        /// Traverses a tree
        /// </summary>
        /// <typeparam name="T">Tree data type</typeparam>
        /// <param name="root">Root node of tree</param>
        /// <param name="order">Traversal order</param>
        /// <returns>An enumeration of the nodes in the specified traverse order</returns>
        public static IEnumerable<Tree<T>> Traverse<T>(this Tree<T> root, TreeTraversalOrder order = TreeTraversalOrder.BreadthFirst)
        {
            if (order == TreeTraversalOrder.BreadthFirst)
                return TraverseBF(root);

            else return TraverseDF(root, order);
        }

        private static IEnumerable<Tree<T>> TraverseDF<T>(this Tree<T> root, TreeTraversalOrder order)
        {
            // Preorder - return root first
            if (order == TreeTraversalOrder.DepthFirstPreOrder)
                yield return root;

            // Return children
            foreach (var child in root.Children)
                foreach (var node in TraverseDF(child, order))
                    yield return node;

            // Postorder - return root last
            if (order == TreeTraversalOrder.DepthFirstPostOrder)
                yield return root;
        }

        private static IEnumerable<Tree<T>> TraverseBF<T>(this Tree<T> root)
        {
            // Create a queue containing the root
            Queue<Tree<T>> queue = new Queue<Tree<T>>();
            queue.Enqueue(root);

            // While there are elements in the queue
            while (queue.Count > 0)
            {
                // Return next node in tree
                var node = queue.Dequeue();
                yield return node;

                // Enqueue node's children
                foreach (var child in node.Children)
                    queue.Enqueue(child);
            }
        }

        /// <summary>
        /// Applies an action to every node of the tree
        /// </summary>
        /// <typeparam name="T">Tree data type</typeparam>
        /// <param name="root">Root node of tree</param>
        /// <param name="action">Action to apply</param>
        /// <param name="order">Traversal order</param>
        public static void Apply<T>(this Tree<T> root, Action<Tree<T>> action, TreeTraversalOrder order = TreeTraversalOrder.BreadthFirst)
        {
            // Safety check
            if (action == null)
                return;

            // Apply action
            foreach (var node in Traverse(root, order))
                action(node);
        }

        /// <summary>
        /// Applies an action to every node of the tree
        /// </summary>
        /// <typeparam name="T">Tree data type</typeparam>
        /// <param name="root">Root node of tree</param>
        /// <param name="action">Action to apply</param>
        /// <param name="order">Traversal order</param>
        public static void ApplyToData<T>(this Tree<T> root, Action<T> action, TreeTraversalOrder order = TreeTraversalOrder.BreadthFirst)
            where T : class
        {
            // Safety check
            if (action == null)
                return;

            // Apply action
            foreach (var node in Traverse(root, order))
                action(node.Data);
        }

        /// <summary>
        /// Rebuilds the tree by applying the specified transform function
        /// </summary>
        /// <typeparam name="T">Data type of tree</typeparam>
        /// <typeparam name="TResult">Data type of rebuilt tree</typeparam>
        /// <param name="root">The root node</param>
        /// <param name="transformFunction">The transform function</param>
        /// <returns>The transformed tree</returns>
        public static Tree<TResult> Transform<T, TResult>(this Tree<T> root, Func<Tree<T>, Tree<TResult>> transformFunction)
        {
            // Safety check
            if (transformFunction == null)
                throw new ArgumentNullException("Transform function cannot be null.");

            // Build root
            Tree<TResult> resRoot = transformFunction(root);

            // Add children
            foreach (var node in root.Children)
                resRoot.Children.Add(Transform(node, transformFunction));

            // Return
            return resRoot;
        }

        /// <summary>
        /// Rebuilds the tree by applying the specified transform function
        /// </summary>
        /// <typeparam name="T">Data type of tree</typeparam>
        /// <typeparam name="TResult">Data type of rebuilt tree</typeparam>
        /// <param name="root">The root node</param>
        /// <param name="transformFunction">The transform function</param>
        /// <returns>The transformed tree</returns>
        public static Tree<TResult> TransformData<T, TResult>(this Tree<T> root, Func<T, TResult> transformFunction)
        {
            // Safety check
            if (transformFunction == null)
                throw new ArgumentNullException("Transform function cannot be null.");

            // Build root
            Tree<TResult> resRoot = new Tree<TResult>(transformFunction(root.Data));

            // Add children
            foreach (var node in root.Children)
                resRoot.Children.Add(TransformData(node, transformFunction));

            // Return
            return resRoot;
        }
    }
}
