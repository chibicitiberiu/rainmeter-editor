using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RainmeterStudio.Model;

namespace RainmeterStudio.Utils
{
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
        public static Tree<TResult> Transform<T,TResult>(this Tree<T> root, Func<Tree<T>, Tree<TResult>> transformFunction)
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
