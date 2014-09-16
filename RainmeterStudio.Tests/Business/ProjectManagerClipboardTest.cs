using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RainmeterStudio.Core.Model;

namespace RainmeterStudio.Tests.Business
{
    /// <summary>
    /// Tests the project item operations for the project manager
    /// </summary>
    /// <remarks>
    /// Initial project structure:
    ///   root
    ///   +  folder1
    ///   |  +  sub1
    ///   |  |  +  file2.png
    ///   |  |  +  file3.txt
    ///   |  +  file1.txt
    ///   +  folder2
    ///   +  file1.txt
    ///   +  sub1
    /// </remarks>
    [TestClass]
    public class ProjectManagerProjectItemOperationsTest : ProjectManagerTestBase
    {
        private Reference Root
        {
            get
            {
                return ProjectManager.ActiveProject.Root;
            }
        }

        /// <summary>
        /// Sets up test
        /// </summary>
        public override void OnInitialize()
        {
            // Create a new project
            string projectName = "test";
            string projectPath = projectName + ".rsproj";

            ProjectManager.CreateProject(projectName, projectPath, ProjectTemplate);

            // Create a project structure
            var root = ProjectManager.ActiveProject.Root;
            ProjectManager.CreateFolder("folder1", root);
            ProjectManager.CreateFolder("folder2", root);
            ProjectManager.CreateFolder("sub1", root.Children[0]);
            ProjectManager.CreateFolder("sub1", root);

            File.Create("file1.txt").Close();
            File.Create("folder1\\file1.txt").Close();
            File.Create("folder1\\sub1\\file2.png").Close();
            File.Create("folder1\\sub1\\file3.txt").Close();

            root.Add(new Reference("file1.txt", "file1.txt"));
            root.Children[0].Add(new Reference("file1.txt", "folder1\\file1.txt"));
            root.Children[0].Children[0].Add(new Reference("file2.png", "folder1\\sub1\\file2.png"));
            root.Children[0].Children[0].Add(new Reference("file3.txt", "folder1\\sub1\\file3.txt"));

        }

        #region Cut & paste for files

        [TestMethod]
        public void ProjectManagerCutPasteFileTest()
        {
            var folder1 = Root.GetReference("test/folder1");
            var folder2 = Root.GetReference("test/folder2");
            var file1 = Root.GetReference("test/folder1/file1.txt");
            var file2 = Root.GetReference("test/folder1/sub1/file2.png");

            // Initial state
            Assert.AreEqual(2, folder1.Count);
            Assert.AreEqual(0, folder2.Count);
            Assert.AreEqual(folder1, file1.Parent);

            // Cut
            ProjectManager.ProjectItemCutClipboard(file1);

            // The item should be in the clipboard, but state unchanged
            Assert.IsTrue(ProjectManager.HaveProjectItemInClipboard());
            Assert.AreEqual(2, folder1.Count);
            Assert.AreEqual(0, folder2.Count);
            Assert.AreEqual(folder1, file1.Parent);

            // Paste in folder 2
            ProjectManager.ProjectItemPasteClipboard(folder2);
            Assert.IsFalse(ProjectManager.HaveProjectItemInClipboard());
            Assert.AreEqual(1, folder1.Count);
            Assert.AreEqual(1, folder2.Count);
            Assert.AreEqual(folder2, file1.Parent);

            // Cut and paste in root
            ProjectManager.ProjectItemCutClipboard(file2);
            ProjectManager.ProjectItemPasteClipboard(Root);

            Assert.AreEqual(Root, file2.Parent);
        }

        [TestMethod]
        public void ProjectManagerCutPasteFileNameConflictTest()
        {
            var folder1 = Root.GetReference("test/folder1");
            var file1 = Root.GetReference("test/folder1/file1.txt");

            // Initial state
            Assert.AreEqual(2, folder1.Count);
            Assert.AreEqual(4, Root.Count);
            Assert.AreEqual(folder1, file1.Parent);

            // Cut
            ProjectManager.ProjectItemCutClipboard(file1);

            // The item should be in the clipboard, but state unchanged
            Assert.IsTrue(ProjectManager.HaveProjectItemInClipboard());
            Assert.AreEqual(2, folder1.Count);
            Assert.AreEqual(4, Root.Count);
            Assert.AreEqual(folder1, file1.Parent);

            // Paste in root
            try
            {
                ProjectManager.ProjectItemPasteClipboard(Root);
                Assert.Fail("File should already exist, should not overwrite.");
            }
            catch (IOException)
            {
            }

            // Item shouldn't be in clipboard any more, but state should remain the same
            Assert.IsTrue(ProjectManager.HaveProjectItemInClipboard());
            Assert.AreEqual(2, folder1.Count);
            Assert.AreEqual(4, Root.Count);
            Assert.AreEqual(folder1, file1.Parent);
        }

        [TestMethod]
        public void ProjectManagerCutPasteFileSameDirectoryTest()
        {
            var folder1 = Root.GetReference("test/folder1");
            var file1 = Root.GetReference("test/folder1/file1.txt");

            // Initial state
            Assert.AreEqual(2, folder1.Count);
            Assert.AreEqual(folder1, file1.Parent);

            // Cut
            ProjectManager.ProjectItemCutClipboard(file1);

            // The item should be in the clipboard, but state unchanged
            Assert.IsTrue(ProjectManager.HaveProjectItemInClipboard());
            Assert.AreEqual(2, folder1.Count);
            Assert.AreEqual(folder1, file1.Parent);

            // Paste in root
            ProjectManager.ProjectItemPasteClipboard(folder1);

            // Item shouldn't be in clipboard any more, but state should remain the same
            Assert.IsFalse(ProjectManager.HaveProjectItemInClipboard());
            Assert.AreEqual(2, folder1.Count);
            Assert.AreEqual(folder1, file1.Parent);
        }

        #endregion

        #region Cut & paste for directories

        [TestMethod]
        public void ProjectManagerCutPasteDirectoryTest()
        {
            var folder1 = Root.GetReference("test/folder1");
            var folder2 = Root.GetReference("test/folder2");
            var sub1 = Root.GetReference("test/folder1/sub1");

            // Initial state
            Assert.AreEqual(2, folder1.Count);
            Assert.AreEqual(0, folder2.Count);
            Assert.AreEqual(2, sub1.Count);
            Assert.IsFalse(sub1.Children.Any(x => !File.Exists(x.StoragePath)));
            Assert.AreEqual(folder1, sub1.Parent);
            Assert.AreEqual("test/folder1/sub1/file2.png", sub1.Children[0].QualifiedName);
            Assert.AreEqual("folder1\\sub1\\file2.png", sub1.Children[0].StoragePath);

            // Cut
            ProjectManager.ProjectItemCutClipboard(sub1);

            // The item should be in the clipboard, but state unchanged
            Assert.IsTrue(ProjectManager.HaveProjectItemInClipboard());
            Assert.AreEqual(2, folder1.Count);
            Assert.AreEqual(0, folder2.Count);
            Assert.AreEqual(2, sub1.Count);
            Assert.IsFalse(sub1.Children.Any(x => !File.Exists(x.StoragePath)));
            Assert.AreEqual(folder1, sub1.Parent);

            // Paste in folder 2
            ProjectManager.ProjectItemPasteClipboard(folder2);
            Assert.IsFalse(ProjectManager.HaveProjectItemInClipboard());
            Assert.AreEqual(1, folder1.Count);
            Assert.AreEqual(1, folder2.Count);
            Assert.AreEqual(2, sub1.Count);
            Assert.IsFalse(sub1.Children.Any(x => !File.Exists(x.StoragePath)));
            Assert.AreEqual(folder2, sub1.Parent);
            Assert.AreEqual("test/folder2/sub1/file2.png", sub1.Children[0].QualifiedName);
            Assert.AreEqual("folder2\\sub1\\file2.png", sub1.Children[0].StoragePath);
        }

        [TestMethod]
        public void ProjectManagerCutPasteDirectoryNameConflictTest()
        {
            var folder1 = Root.GetReference("test/folder1");
            var sub1 = Root.GetReference("test/folder1/sub1");

            // Initial state
            Assert.AreEqual(2, folder1.Count);
            Assert.AreEqual(4, Root.Count);
            Assert.AreEqual(2, sub1.Count);
            Assert.IsFalse(sub1.Children.Any(x => !File.Exists(x.StoragePath)));
            Assert.AreEqual(folder1, sub1.Parent);
            Assert.AreEqual("test/folder1/sub1/file2.png", sub1.Children[0].QualifiedName);
            Assert.AreEqual("folder1\\sub1\\file2.png", sub1.Children[0].StoragePath);

            // Cut
            ProjectManager.ProjectItemCutClipboard(sub1);

            // The item should be in the clipboard, but state unchanged
            Assert.IsTrue(ProjectManager.HaveProjectItemInClipboard());
            Assert.AreEqual(2, folder1.Count);
            Assert.AreEqual(4, Root.Count);
            Assert.AreEqual(2, sub1.Count);
            Assert.IsFalse(sub1.Children.Any(x => !File.Exists(x.StoragePath)));
            Assert.AreEqual(folder1, sub1.Parent);

            // Paste in root
            try
            {
                ProjectManager.ProjectItemPasteClipboard(Root);
                Assert.Fail("Directory already exists, should not overwrite.");
            }
            catch (IOException)
            {
            }

            // Object remains in clipboard, state unchanged
            Assert.IsTrue(ProjectManager.HaveProjectItemInClipboard());
            Assert.AreEqual(2, folder1.Count);
            Assert.AreEqual(4, Root.Count);
            Assert.AreEqual(2, sub1.Count);
            Assert.IsFalse(sub1.Children.Any(x => !File.Exists(x.StoragePath)));
            Assert.AreEqual(folder1, sub1.Parent);
            Assert.AreEqual("test/folder1/sub1/file2.png", sub1.Children[0].QualifiedName);
            Assert.AreEqual("folder1\\sub1\\file2.png", sub1.Children[0].StoragePath);
        }

        [TestMethod]
        public void ProjectManagerCutPasteDirectorySameDirectoryTest()
        {
            var folder1 = Root.GetReference("test/folder1");
            var sub1 = Root.GetReference("test/folder1/sub1");

            // Initial state
            Assert.AreEqual(2, folder1.Count);
            Assert.AreEqual(2, sub1.Count);
            Assert.IsFalse(sub1.Children.Any(x => !File.Exists(x.StoragePath)));
            Assert.AreEqual(folder1, sub1.Parent);
            Assert.AreEqual("test/folder1/sub1/file2.png", sub1.Children[0].QualifiedName);
            Assert.AreEqual("folder1\\sub1\\file2.png", sub1.Children[0].StoragePath);

            // Cut
            ProjectManager.ProjectItemCutClipboard(sub1);

            // The item should be in the clipboard, but state unchanged
            Assert.IsTrue(ProjectManager.HaveProjectItemInClipboard());
            Assert.AreEqual(2, folder1.Count);
            Assert.AreEqual(2, sub1.Count);
            Assert.IsFalse(sub1.Children.Any(x => !File.Exists(x.StoragePath)));
            Assert.AreEqual(folder1, sub1.Parent);

            // Paste in folder 2
            try
            {
                ProjectManager.ProjectItemPasteClipboard(folder1);
                Assert.Fail("Directory is same.");
            }
            catch (IOException)
            {
            }

            Assert.IsTrue(ProjectManager.HaveProjectItemInClipboard());
            Assert.AreEqual(2, folder1.Count);
            Assert.AreEqual(2, sub1.Count);
            Assert.IsFalse(sub1.Children.Any(x => !File.Exists(x.StoragePath)));
            Assert.AreEqual(folder1, sub1.Parent);
            Assert.AreEqual("test/folder1/sub1/file2.png", sub1.Children[0].QualifiedName);
            Assert.AreEqual("folder1\\sub1\\file2.png", sub1.Children[0].StoragePath);
        }

        #endregion

        #region Copy & paste for files

        [TestMethod]
        public void ProjectManagerCopyPasteFileTest()
        {
            var folder1 = Root.GetReference("test/folder1");
            var folder2 = Root.GetReference("test/folder2");
            var file1 = Root.GetReference("test/folder1/file1.txt");

            // Initial state
            Assert.AreEqual(2, folder1.Count);
            Assert.AreEqual(0, folder2.Count);
            Assert.AreEqual(folder1, file1.Parent);

            // Copy
            ProjectManager.ProjectItemCopyClipboard(file1);

            // The item should be in the clipboard, but state unchanged
            Assert.IsTrue(ProjectManager.HaveProjectItemInClipboard());
            Assert.AreEqual(2, folder1.Count);
            Assert.AreEqual(0, folder2.Count);
            Assert.AreEqual(folder1, file1.Parent);

            // Paste in folder 2
            ProjectManager.ProjectItemPasteClipboard(folder2);
            Assert.IsTrue(ProjectManager.HaveProjectItemInClipboard());
            Assert.AreEqual(2, folder1.Count);
            Assert.AreEqual(1, folder2.Count);
            Assert.AreEqual(folder1, file1.Parent);
        }

        [TestMethod]
        public void ProjectManagerCopyPasteFileNameConflictTest()
        {
            Reference folder1 = Root.GetReference("test/folder1");
            Reference file1 = Root.GetReference("test/folder1/file1.txt");
            Reference copy;

            // Initial state
            Assert.AreEqual(2, folder1.Count);
            Assert.AreEqual(4, Root.Count);
            Assert.AreEqual(folder1, file1.Parent);

            // Copy
            ProjectManager.ProjectItemCopyClipboard(file1);

            // The item should be in the clipboard, but state unchanged
            Assert.IsTrue(ProjectManager.HaveProjectItemInClipboard());
            Assert.AreEqual(2, folder1.Count);
            Assert.AreEqual(4, Root.Count);
            Assert.AreEqual(folder1, file1.Parent);

            // Paste in root
            ProjectManager.ProjectItemPasteClipboard(Root);

            Assert.IsTrue(ProjectManager.HaveProjectItemInClipboard());
            Assert.AreEqual(2, folder1.Count);
            Assert.AreEqual(5, Root.Count);
            Assert.AreEqual(folder1, file1.Parent);
            Assert.IsTrue(Root.TryGetReference("test/file1_2.txt", out copy));

            // Paste again in root
            ProjectManager.ProjectItemPasteClipboard(Root);

            Assert.IsTrue(ProjectManager.HaveProjectItemInClipboard());
            Assert.AreEqual(2, folder1.Count);
            Assert.AreEqual(6, Root.Count);
            Assert.AreEqual(folder1, file1.Parent);
            Assert.IsTrue(Root.TryGetReference("test/file1_3.txt", out copy));
        }

        [TestMethod]
        public void ProjectManagerCopyPasteFileSameDirectoryTest()
        {
            var folder1 = Root.GetReference("test/folder1");
            var file1 = Root.GetReference("test/folder1/file1.txt");

            // Initial state
            Assert.AreEqual(2, folder1.Count);
            Assert.AreEqual(folder1, file1.Parent);

            // Copy
            ProjectManager.ProjectItemCopyClipboard(file1);

            // The item should be in the clipboard, but state unchanged
            Assert.IsTrue(ProjectManager.HaveProjectItemInClipboard());
            Assert.AreEqual(2, folder1.Count);
            Assert.AreEqual(folder1, file1.Parent);

            // Paste in root
            ProjectManager.ProjectItemPasteClipboard(folder1);

            Assert.IsTrue(ProjectManager.HaveProjectItemInClipboard());
            Assert.AreEqual(3, folder1.Count);
            Assert.AreEqual(folder1, file1.Parent);
            Assert.IsTrue(Root.TryGetReference("test/folder1/file1_2.txt", out file1));

            // Past again in root
            ProjectManager.ProjectItemPasteClipboard(folder1);

            Assert.IsTrue(ProjectManager.HaveProjectItemInClipboard());
            Assert.AreEqual(4, folder1.Count);
            Assert.AreEqual(folder1, file1.Parent);
            Assert.IsTrue(Root.TryGetReference("test/folder1/file1_3.txt", out file1));
        }

        #endregion

        #region Copy & paste for directories

        [TestMethod]
        public void ProjectManagerCopyPasteDirectoryTest()
        {
            var folder1 = Root.GetReference("test/folder1");
            var folder2 = Root.GetReference("test/folder2");
            var sub1 = Root.GetReference("test/folder1/sub1");

            // Initial state
            Assert.AreEqual(2, folder1.Count);
            Assert.AreEqual(0, folder2.Count);
            Assert.AreEqual(2, sub1.Count);
            Assert.IsFalse(sub1.Children.Any(x => !File.Exists(x.StoragePath)));
            Assert.AreEqual(folder1, sub1.Parent);
            Assert.AreEqual("test/folder1/sub1/file2.png", sub1.Children[0].QualifiedName);
            Assert.AreEqual("folder1\\sub1\\file2.png", sub1.Children[0].StoragePath);

            // Copy
            ProjectManager.ProjectItemCopyClipboard(sub1);

            // The item should be in the clipboard, but state unchanged
            Assert.IsTrue(ProjectManager.HaveProjectItemInClipboard());
            Assert.AreEqual(2, folder1.Count);
            Assert.AreEqual(0, folder2.Count);
            Assert.AreEqual(2, sub1.Count);
            Assert.IsFalse(sub1.Children.Any(x => !File.Exists(x.StoragePath)));
            Assert.AreEqual(folder1, sub1.Parent);

            // Paste in folder 2
            ProjectManager.ProjectItemPasteClipboard(folder2);
            Reference sub2;
            Assert.IsTrue(Root.TryGetReference("test/folder2/sub1", out sub2));
            Assert.IsTrue(ProjectManager.HaveProjectItemInClipboard());
            Assert.AreEqual(2, folder1.Count);
            Assert.AreEqual(1, folder2.Count);
            Assert.AreEqual(2, sub1.Count);
            Assert.AreEqual(2, sub2.Count);
            Assert.IsFalse(sub1.Children.Any(x => !File.Exists(x.StoragePath)));
            Assert.IsFalse(sub2.Children.Any(x => !File.Exists(x.StoragePath)));
            Assert.AreEqual(folder1, sub1.Parent);
            Assert.AreEqual(folder2, sub2.Parent);
            Assert.AreEqual("test/folder1/sub1/file2.png", sub1.Children[0].QualifiedName);
            Assert.AreEqual("folder1\\sub1\\file2.png", sub1.Children[0].StoragePath);
            Assert.AreEqual("test/folder2/sub1/file2.png", sub2.Children[0].QualifiedName);
            Assert.AreEqual("folder2\\sub1\\file2.png", sub2.Children[0].StoragePath);
        }

        [TestMethod]
        public void ProjectManagerCopyPasteDirectoryNameConflictTest()
        {
            var folder1 = Root.GetReference("test/folder1");
            var sub1 = Root.GetReference("test/folder1/sub1");

            // Initial state
            Assert.AreEqual(2, folder1.Count);
            Assert.AreEqual(4, Root.Count);
            Assert.AreEqual(2, sub1.Count);
            Assert.IsFalse(sub1.Children.Any(x => !File.Exists(x.StoragePath)));
            Assert.AreEqual(folder1, sub1.Parent);
            Assert.AreEqual("test/folder1/sub1/file2.png", sub1.Children[0].QualifiedName);
            Assert.AreEqual("folder1\\sub1\\file2.png", sub1.Children[0].StoragePath);

            // Copy
            ProjectManager.ProjectItemCopyClipboard(sub1);

            // The item should be in the clipboard, but state unchanged
            Assert.IsTrue(ProjectManager.HaveProjectItemInClipboard());
            Assert.AreEqual(2, folder1.Count);
            Assert.AreEqual(4, Root.Count);
            Assert.AreEqual(2, sub1.Count);
            Assert.IsFalse(sub1.Children.Any(x => !File.Exists(x.StoragePath)));
            Assert.AreEqual(folder1, sub1.Parent);

            // Paste in root
            try
            {
                ProjectManager.ProjectItemPasteClipboard(Root);
                Assert.Fail("Directory already exists, should not overwrite.");
            }
            catch (IOException)
            {
            }

            // Object remains in clipboard, state unchanged
            Assert.IsTrue(ProjectManager.HaveProjectItemInClipboard());
            Assert.AreEqual(2, folder1.Count);
            Assert.AreEqual(4, Root.Count);
            Assert.AreEqual(2, sub1.Count);
            Assert.IsFalse(sub1.Children.Any(x => !File.Exists(x.StoragePath)));
            Assert.AreEqual(folder1, sub1.Parent);
            Assert.AreEqual("test/folder1/sub1/file2.png", sub1.Children[0].QualifiedName);
            Assert.AreEqual("folder1\\sub1\\file2.png", sub1.Children[0].StoragePath);
        }

        [TestMethod]
        public void ProjectManagerCopyPasteDirectorySameDirectoryTest()
        {
            var folder1 = Root.GetReference("test/folder1");
            var sub1 = Root.GetReference("test/folder1/sub1");

            // Initial state
            Assert.AreEqual(2, folder1.Count);
            Assert.AreEqual(2, sub1.Count);
            Assert.IsFalse(sub1.Children.Any(x => !File.Exists(x.StoragePath)));
            Assert.AreEqual(folder1, sub1.Parent);
            Assert.AreEqual("test/folder1/sub1/file2.png", sub1.Children[0].QualifiedName);
            Assert.AreEqual("folder1\\sub1\\file2.png", sub1.Children[0].StoragePath);

            // Copy
            ProjectManager.ProjectItemCopyClipboard(sub1);

            // The item should be in the clipboard, but state unchanged
            Assert.IsTrue(ProjectManager.HaveProjectItemInClipboard());
            Assert.AreEqual(2, folder1.Count);
            Assert.AreEqual(2, sub1.Count);
            Assert.IsFalse(sub1.Children.Any(x => !File.Exists(x.StoragePath)));
            Assert.AreEqual(folder1, sub1.Parent);

            // Paste in folder 2
            try
            {
                ProjectManager.ProjectItemPasteClipboard(folder1);
                Assert.Fail("Directory is same.");
            }
            catch (IOException)
            {
            }

            Assert.IsTrue(ProjectManager.HaveProjectItemInClipboard());
            Assert.AreEqual(2, folder1.Count);
            Assert.AreEqual(2, sub1.Count);
            Assert.IsFalse(sub1.Children.Any(x => !File.Exists(x.StoragePath)));
            Assert.AreEqual(folder1, sub1.Parent);
            Assert.AreEqual("test/folder1/sub1/file2.png", sub1.Children[0].QualifiedName);
            Assert.AreEqual("folder1\\sub1\\file2.png", sub1.Children[0].StoragePath);
        }

        #endregion
    }
}
