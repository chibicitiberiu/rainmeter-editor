using System;
using System.Collections.Generic;
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
        /// <summary>
        /// Tests the constructors of the reference class
        /// </summary>
        [TestMethod]
        public void ReferenceConstructorTest()
        {
            Reference root = new Reference("root", "D:\\Data\\Project", Reference.ReferenceTargetKind.Directory);
            Reference file = new Reference("f ile__asdf.txt", Reference.ReferenceTargetKind.File);
                        
            // Test root
            Assert.AreEqual("root", root.Name);
            Assert.IsNull(root.Parent);
            Assert.AreEqual("root", root.QualifiedName);
            Assert.AreEqual("D:\\Data\\Project", root.StoragePath);
            Assert.IsTrue(Enumerable.Repeat("root", 1).SequenceEqual(root.QualifiedParts));
            
            // Test file
            Assert.AreEqual("f ile__asdf.txt", file.Name);
            Assert.IsNull(file.Parent);
            Assert.AreEqual("f ile__asdf.txt", file.QualifiedName);
            Assert.IsNull(file.StoragePath);
            Assert.IsTrue(Enumerable.Repeat("f ile__asdf.txt", 1).SequenceEqual(file.QualifiedParts));
        }

        /// <summary>
        /// Tests parenting in the reference class
        /// </summary>
        [TestMethod]
        public void ReferenceParentingTest()
        {
            Reference root = new Reference(String.Empty, "D:\\Data\\Project");
            Reference folder1 = new Reference("folder1", Reference.ReferenceTargetKind.Directory);
            Reference folder2 = new Reference("folder 2", Reference.ReferenceTargetKind.Directory);
            Reference file1 = new Reference("file1", Reference.ReferenceTargetKind.File);
            Reference file2 = new Reference("file2.txt", Reference.ReferenceTargetKind.File);
            Reference file3 = new Reference("file 3.png", Reference.ReferenceTargetKind.File);

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
            Reference folder1 = new Reference("folder1", Reference.ReferenceTargetKind.Directory);
            Reference folder2 = new Reference("folder 2", Reference.ReferenceTargetKind.Directory);
            Reference file1 = new Reference("file1", Reference.ReferenceTargetKind.File);
            Reference file2 = new Reference("file2.txt", Reference.ReferenceTargetKind.File);
            Reference file3 = new Reference("file 3.png", Reference.ReferenceTargetKind.File);

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
            Reference folder1 = new Reference("folder1", Reference.ReferenceTargetKind.Directory);
            Reference folder2 = new Reference("folder 2", Reference.ReferenceTargetKind.Directory);
            Reference file1 = new Reference("file1", Reference.ReferenceTargetKind.File);
            Reference file2 = new Reference("file2.txt", Reference.ReferenceTargetKind.File);
            Reference file3 = new Reference("file 3.png", Reference.ReferenceTargetKind.File);

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
        }
    }
}
