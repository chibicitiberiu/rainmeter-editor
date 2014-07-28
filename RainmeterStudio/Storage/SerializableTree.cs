using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using RainmeterStudio.Model;

namespace RainmeterStudio.Storage
{
    /// <summary>
    /// A special type of tree that implements a very small subset of tree operations, and can be serialized
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SerializableTree<T>
    {
        /// <summary>
        /// Gets or sets the attached data
        /// </summary>
        [XmlElement("data")]
        public T Data { get; set; }

        /// <summary>
        /// Gets or sets the list of children
        /// </summary>
        [XmlArray("children"), XmlArrayItem("child")]
        public List<SerializableTree<T>> Children { get; set; }

        /// <summary>
        /// Initializes the serializable tree
        /// </summary>
        public SerializableTree()
        {
            Children = new List<SerializableTree<T>>();
            Data = default(T);
        }

        /// <summary>
        /// Initializes the serializable tree with specified data
        /// </summary>
        /// <param name="data">Data</param>
        public SerializableTree(T data)
        {
            Children = new List<SerializableTree<T>>();
            Data = data;
        }
    }

    /// <summary>
    /// Extension methods for converting to and from serializable trees
    /// </summary>
    public static class SerializableTreeExtensions
    {
        /// <summary>
        /// Converts tree into a serializable tree
        /// </summary>
        /// <typeparam name="T">Data type of tree</typeparam>
        /// <param name="root">Root node</param>
        /// <returns>Serializable tree</returns>
        public static SerializableTree<T> AsSerializableTree<T>(this Tree<T> root)
        {
            // Convert current node
            SerializableTree<T> sRoot = new SerializableTree<T>(root.Data);

            // Add children
            foreach (var child in root.Children)
                sRoot.Children.Add(AsSerializableTree(child));

            // Return root
            return sRoot;
        }

        /// <summary>
        /// Converts serializable tree into a tree
        /// </summary>
        /// <typeparam name="T">Data type of tree</typeparam>
        /// <param name="root">Root node</param>
        /// <returns>Tree</returns>
        public static Tree<T> AsTree<T>(this SerializableTree<T> root)
        {
            // Convert current node
            Tree<T> sRoot = new Tree<T>(root.Data);

            // Add children
            foreach (var child in root.Children)
                sRoot.Add(AsTree(child));

            // Return root
            return sRoot;
        }
    }
}
