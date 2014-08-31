using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RainmeterStudio.Storage;
using System.IO;
using RainmeterStudio.Core.Model;
using Version = RainmeterStudio.Core.Utils.Version;

namespace RainmeterStudio.Tests.Storage
{
    /// <summary>
    /// Tests the ProjectStorage class
    /// </summary>
    [TestClass]
    public class ProjectStorageTest
    {
        private ProjectStorage ProjectStorage = new ProjectStorage();

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            Directory.SetCurrentDirectory(TestContext.DeploymentDirectory);
        }

        [TestMethod]
        public void ProjectStorageSmokeTest()
        {
            string filename = TestContext.TestName + ".rsproj";

            // Create project
            Project project = CreateProject();

            // Save and load
            ProjectStorage.Save(project, filename);
            Project res = ProjectStorage.Load(filename);

            // Verify results
            Assert.IsNotNull(res);
            Assert.AreEqual(project.Author, res.Author);
            Assert.AreEqual(project.AutoLoadFile, res.AutoLoadFile);
            Assert.AreEqual(project.MinimumRainmeter, res.MinimumRainmeter);
            Assert.AreEqual(project.MinimumWindows, res.MinimumWindows);
            Assert.AreEqual(project.Name, res.Name);
            Assert.AreEqual(project.Root, res.Root);
            Assert.IsTrue(project.VariableFiles.SequenceEqual(res.VariableFiles));
            Assert.AreEqual(project.Version, res.Version);
        }

        [TestMethod]
        public void ProjectStorageEmptyProjectSmokeTest()
        {
            string filename = TestContext.TestName + ".rsproj";

            // Create a project
            Project project = new Project();

            // Save and load project
            ProjectStorage.Save(project, filename);
            Project res = ProjectStorage.Load(filename);

            // Test results
            Assert.IsNotNull(res);
            Assert.AreEqual(project.Author, res.Author);
            Assert.AreEqual(project.AutoLoadFile, res.AutoLoadFile);
            Assert.AreEqual(project.MinimumRainmeter, res.MinimumRainmeter);
            Assert.AreEqual(project.MinimumWindows, res.MinimumWindows);
            Assert.AreEqual(project.Name, res.Name);
            Assert.AreEqual(project.Root, res.Root);
            Assert.IsTrue(project.VariableFiles.SequenceEqual(res.VariableFiles));
            Assert.AreEqual(project.Version, res.Version);
        }

        private Project CreateProject()
        {
            // Create some file references
            Reference folder1 = new Reference("folder1", "folder1");
            Reference folder2 = new Reference("folder2", "folder2");
            Reference file1 = new Reference("file1.txt", "folder1/file1.txt");
            Reference file2 = new Reference("file2.ini", "folder2/file2.ini");
            Reference file3 = new Reference("file3.bmp", "file3.bmp");

            // Create a project
            Project project = new Project();
            project.Author = "Tiberiu Chibici";
            project.MinimumRainmeter = new Version("3.1");
            project.MinimumWindows = new Version("5.1");
            project.Name = "My project";
            project.Version = new Version("1.0.1");

            project.AutoLoadFile = file2;
            project.VariableFiles.Add(file1);

            // Set project references
            project.Root.Add(folder1);
            project.Root.Add(folder2);
            folder1.Add(file1);
            folder2.Add(file2);
            project.Root.Add(file3);

            return project;
        }
    }
}
