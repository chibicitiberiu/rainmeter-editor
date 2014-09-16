using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RainmeterStudio.Core.Model;

namespace RainmeterStudio.Tests.Model
{
    /// <summary>
    /// Tests the reference class
    /// </summary>
    [TestClass]
    public class ReferenceTest
    {
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            Directory.SetCurrentDirectory(TestContext.DeploymentDirectory);
        }

        /// <summary>
        /// Tests the constructors of the reference class
        /// </summary>
        [TestMethod]
        public void ReferenceConstructorTest()
        {
            Reference root = new Reference("root", "D:\\Data\\Project", ReferenceTargetKind.Directory);
            Reference file = new Reference("f ile__asdf.txt", ReferenceTargetKind.File);
                        
            // Test root
            Assert.AreEqual("root", root.Name);
            Assert.IsNull(root.Parent);
            Assert.AreEqual("root", root.QualifiedName);
            Assert.AreEqual("D:\\Data\\Project", root.StoragePath);
            Assert.IsTrue(Enumerable.Repeat("root", 1).SequenceEqual(root.QualifiedParts));
            Assert.AreEqual(ReferenceTargetKind.Directory, root.TargetKind);

            // Test file
            Assert.AreEqual("f ile__asdf.txt", file.Name);
            Assert.IsNull(file.Parent);
            Assert.AreEqual("f ile__asdf.txt", file.QualifiedName);
            Assert.IsNull(file.StoragePath);
            Assert.IsTrue(Enumerable.Repeat("f ile__asdf.txt", 1).SequenceEqual(file.QualifiedParts));
            Assert.AreEqual(ReferenceTargetKind.File, file.TargetKind);
        }

        /// <summary>
        /// Tests parenting in the reference class
        /// </summary>
        [TestMethod]
        public void ReferenceParentingTest()
        {
            Reference root = new Reference(String.Empty, "D:\\Data\\Project");
            Reference folder1 = new Reference("folder1", ReferenceTargetKind.Directory);
            Reference folder2 = new Reference("folder 2", ReferenceTargetKind.Directory);
            Reference file1 = new Reference("file1", ReferenceTargetKind.File);
            Reference file2 = new Reference("file2.txt", ReferenceTargetKind.File);
            Reference file3 = new Reference("file 3.png", ReferenceTargetKind.File);

            root.Add(folder1);
            root.Add(file3);
            folder1.Add(file1);
            folder1.Add(folder2);
            folder2.Add(file2);

            Assert.IsNull(root.Parent);
            Assert.AreEqual(2, root.ChildrenDictionary.Count);
            Assert.AreEqual(root, folder1.Parent);
            Assert.AreEqual(root, file3.Parent);

            Assert.AreEqual(2, folder1.ChildrenDictionary.Count);
            Assert.AreEqual(folder1, folder2.Parent);
            Assert.AreEqual(folder1, file1.Parent);

            Assert.IsNotNull(folder2.Children.FirstOrDefault(x => x == file2));

            // Unparent something
            file3.Unparent();
            Assert.IsNull(file3.Parent);
            Assert.IsNull(root.Children.FirstOrDefault(x => x == file3));
            Assert.AreEqual(1, root.ChildrenDictionary.Count);

            // Remove something
            root.Remove(folder1);
            Assert.IsNull(folder1.Parent);
            Assert.AreEqual(0, root.ChildrenDictionary.Count);
        }

        /// <summary>
        /// Tests qualified names in the reference class
        /// </summary>
        [TestMethod]
        public void ReferenceQualifiedNameTest()
        {
            Reference root = new Reference(String.Empty, "D:\\Data\\Project");
            Reference folder1 = new Reference("folder1", ReferenceTargetKind.Directory);
            Reference folder2 = new Reference("folder 2", ReferenceTargetKind.Directory);
            Reference file1 = new Reference("file1", ReferenceTargetKind.File);
            Reference file2 = new Reference("file2.txt", ReferenceTargetKind.File);
            Reference file3 = new Reference("file 3.png", ReferenceTargetKind.File);

            root.Add(folder1);
            root.Add(file3);
            folder1.Add(file1);
            folder1.Add(folder2);
            folder2.Add(file2);

            // Test qualified names
            Assert.AreEqual(String.Empty, root.QualifiedName);
            Assert.AreEqual("/folder1", folder1.QualifiedName);
            Assert.AreEqual("/folder1/folder 2", folder2.QualifiedName);
            Assert.AreEqual("/folder1/file1", file1.QualifiedName);
            Assert.AreEqual("/folder1/folder 2/file2.txt", file2.QualifiedName);
            Assert.AreEqual("/file 3.png", file3.QualifiedName);

            // Test qualified parts
            Assert.IsTrue(new[] { String.Empty }
                .SequenceEqual(root.QualifiedParts));

            Assert.IsTrue(new[] { String.Empty, "folder1" }
                .SequenceEqual(folder1.QualifiedParts));

            Assert.IsTrue(new[] { String.Empty, "folder1", "folder 2" }
                .SequenceEqual(folder2.QualifiedParts));

            Assert.IsTrue(new[] { String.Empty, "folder1", "file1" }
                .SequenceEqual(file1.QualifiedParts));

            Assert.IsTrue(new[] { String.Empty, "folder1", "folder 2", "file2.txt" }
                .SequenceEqual(file2.QualifiedParts));

            Assert.IsTrue(new[] { String.Empty, "file 3.png" }
                .SequenceEqual(file3.QualifiedParts));
        }

        /// <summary>
        /// Tests the 'get reference' extension methods
        /// </summary>
        [TestMethod]
        public void ReferenceGetReferenceTest()
        {
            Reference root = new Reference(String.Empty, "D:\\Data\\Project");
            Reference folder1 = new Reference("folder1", ReferenceTargetKind.Directory);
            Reference folder2 = new Reference("folder 2", ReferenceTargetKind.Directory);
            Reference file1 = new Reference("file1", ReferenceTargetKind.File);
            Reference file2 = new Reference("file2.txt", ReferenceTargetKind.File);
            Reference file3 = new Reference("file 3.png", ReferenceTargetKind.File);
            Reference notInTree = new Reference("file4.txt", ReferenceTargetKind.File);

            root.Add(folder1);
            root.Add(file3);
            folder1.Add(file1);
            folder1.Add(folder2);
            folder2.Add(file2);

            // Test 'get reference' method
            Assert.AreEqual(root, root.GetReference(""));
            Assert.AreEqual(folder2, root.GetReference("/folder1/folder 2"));
            Assert.AreEqual(file3, file2.GetReference("/file 3.png"));
            Assert.AreEqual(file2, file2.GetReference("/folder1/folder 2/file2.txt"));
            Assert.AreEqual(file2, file3.GetReference("/folder1/folder 2/file2.txt"));

            try
            {
                file3.GetReference("/file 3.png/some nonexistant file");
                Assert.Fail();
            }
            catch (ArgumentException)
            {
            }

            // Test 'try get reference' method
            Reference res;

            Assert.IsTrue(root.TryGetReference("", out res));
            Assert.AreEqual(root, res);

            Assert.IsTrue(root.TryGetReference("/folder1/folder 2", out res));
            Assert.AreEqual(folder2, res);

            Assert.IsTrue(file2.TryGetReference("/file 3.png", out res));
            Assert.AreEqual(file3, res);

            Assert.IsTrue(file2.TryGetReference("/folder1/folder 2/file2.txt", out res));
            Assert.AreEqual(file2, res);

            Assert.IsTrue(file3.TryGetReference("/folder1/folder 2/file2.txt", out res));
            Assert.AreEqual(file2, res);

            Assert.IsFalse(root.TryGetReference("/file 3.png/some nonexistant file", out res));

            // Test 'tree contains' method
            Assert.IsTrue(file2.TreeContains(root));
            Assert.IsTrue(root.TreeContains(file3));
            Assert.IsFalse(root.TreeContains(notInTree));
            Assert.IsFalse(notInTree.TreeContains(root));
        }

        /// <summary>
        /// Tests the collection notify behavior
        /// </summary>
        [TestMethod]
        public void ReferenceCollectionChangedTest()
        {
            NotifyCollectionChangedEventArgs args = null;

            // Initialize
            Reference root = new Reference(String.Empty, "D:\\Data\\Project");
            Reference folder1 = new Reference("folder1", ReferenceTargetKind.Directory);
            Reference file1 = new Reference("file1.txt", ReferenceTargetKind.File);
            Reference file2 = new Reference("file2.txt", ReferenceTargetKind.File);
            Reference file3 = new Reference("file3.txt", ReferenceTargetKind.File);

            root.CollectionChanged += new NotifyCollectionChangedEventHandler((sender, e) => args = e);

            // Add item
            args = null;
            root.Add(folder1);

            Assert.IsNotNull(args);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, args.Action);
            Assert.AreEqual(1, args.NewItems.Count);
            Assert.AreEqual(folder1, args.NewItems[0]);
            Assert.IsNull(args.OldItems);

            args = null;
            folder1.Add(file1);
            Assert.IsNull(args);

            args = null;
            root.Add(file2);
            root.Add(file3);
            Assert.IsNotNull(args);

            // Remove
            args = null;
            root.Remove(file3);
            Assert.IsNotNull(root);
            Assert.AreEqual(NotifyCollectionChangedAction.Remove, args.Action);
            Assert.IsNull(args.NewItems);
            Assert.AreEqual(1, args.OldItems.Count);
            Assert.AreEqual(file3, args.OldItems[0]);

            // Unparent
            args = null;
            file2.Unparent();
            Assert.IsNotNull(args);
            Assert.AreEqual(NotifyCollectionChangedAction.Remove, args.Action);
            Assert.IsNull(args.NewItems);
            Assert.AreEqual(1, args.OldItems.Count);
            Assert.AreEqual(file2, args.OldItems[0]);

            // Clear
            args = null;
            root.Clear();
            Assert.IsNotNull(args);
            Assert.AreEqual(NotifyCollectionChangedAction.Reset, args.Action);
            Assert.IsNull(args.NewItems);
            Assert.IsNull(args.OldItems);
        }

        /// <summary>
        /// Tests the property notify behavior
        /// </summary>
        [TestMethod]
        public void ReferencePropertyChangedTest()
        {
            List<string> propNames = new List<string>();

            // Initialize
            Reference root = new Reference("Project", "D:\\Data\\Project", ReferenceTargetKind.Project);
            Reference folder1 = new Reference("folder1", ReferenceTargetKind.Directory);
            Reference file1 = new Reference("file1.txt", ReferenceTargetKind.File);

            file1.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler((sender, e) => propNames.Add(e.PropertyName));

            // Parent
            propNames.Clear();
            folder1.Add(file1);
            Assert.AreEqual(2, propNames.Count);
            Assert.IsTrue(propNames.Contains("Parent"));
            Assert.IsTrue(propNames.Contains("QualifiedName"));

            // Storage path
            propNames.Clear();
            file1.StoragePath = "D:\\Data\\Project\\folder1\\file1.txt";
            Assert.AreEqual(1, propNames.Count);
            Assert.AreEqual("StoragePath", propNames[0]);

            // Target kind
            propNames.Clear();
            file1.TargetKind = ReferenceTargetKind.None;
            Assert.AreEqual(1, propNames.Count);
            Assert.AreEqual("TargetKind", propNames[0]);

            // Name
            propNames.Clear();
            file1.Name = "file10.txt";
            Assert.AreEqual(2, propNames.Count);
            Assert.IsTrue(propNames.Contains("Name"));
            Assert.IsTrue(propNames.Contains("QualifiedName"));

            // Qualified name propagation
            propNames.Clear();
            root.Add(folder1);
            Assert.AreEqual(1, propNames.Count);
            Assert.IsTrue(propNames.Contains("QualifiedName"));
        }

        /// <summary>
        /// Tests the automatic detection of target kinds
        /// </summary>
        [TestMethod]
        public void ReferenceTargetKindTest()
        {
            // Create some files and folders
            Directory.CreateDirectory("folder1");
            File.Create("file1.txt").Close();
            File.Create("file2.rsproj").Close();
            File.Create("folder1/file3").Close();

            Reference folder1 = new Reference("folder1", "folder1");
            Reference file1 = new Reference("file1.txt", "file1.txt");
            Reference file2 = new Reference("file2.rsproj", "file2.rsproj");
            Reference file3 = new Reference("file3", "folder1/file3");
            Reference file4 = new Reference("file4", "file4.txt");

            Assert.AreEqual(ReferenceTargetKind.Directory, folder1.TargetKind);
            Assert.AreEqual(ReferenceTargetKind.File, file1.TargetKind);
            Assert.AreEqual(ReferenceTargetKind.Project, file2.TargetKind);
            Assert.AreEqual(ReferenceTargetKind.File, file3.TargetKind);
            Assert.AreEqual(ReferenceTargetKind.None, file4.TargetKind);
        }
    }
}
