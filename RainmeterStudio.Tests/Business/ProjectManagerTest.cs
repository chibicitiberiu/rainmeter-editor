using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RainmeterStudio.Business;
using RainmeterStudio.Core.Model;

namespace RainmeterStudio.Tests.Business
{
    [TestClass]
    public class ProjectManagerTest : ProjectManagerTestBase
    {
        /// <summary>
        /// Tests if the sample template is registered
        /// </summary>
        [TestMethod]
        public void ProjectManagerTemplatesTest()
        {
            Assert.AreEqual(1, ProjectManager.ProjectTemplates.Count());
            Assert.AreEqual(ProjectTemplate, ProjectManager.ProjectTemplates.First());
        }

        /// <summary>
        /// Tests the project create functionality
        /// </summary>
        [TestMethod]
        public void ProjectManagerCreateProjectTest()
        {
            bool loaded = false;
            string projName = "test";
            string projPath = TestContext.TestName + ".rsproj";
            
            ProjectManager.ActiveProjectChanged += new EventHandler((sender, e) => loaded = true);
            ProjectManager.CreateProject(projName, projPath, ProjectTemplate);

            Assert.IsTrue(loaded);
            Assert.AreEqual(projName, ProjectManager.ActiveProject.Name);
            Assert.AreEqual(projPath, ProjectManager.ActiveProject.Path);
            Assert.IsTrue(File.Exists(projPath));
        }

        /// <summary>
        /// Tests the open project functionality
        /// </summary>
        [TestMethod]
        public void ProjectManagerOpenProjectTest()
        {
            // Create a new project
            bool changed = false;
            string projName = "test";
            string projPath = TestContext.TestName + ".rsproj";

            ProjectManager.ActiveProjectChanged += new EventHandler((sender, e) => changed = true);
            ProjectManager.CreateProject(projName, projPath, ProjectTemplate);

            // Reopen new project
            changed = false;
            ProjectManager.OpenProject(projPath);
            Assert.IsTrue(changed);

            // Close project
            changed = false;
            ProjectManager.Close();
            Assert.IsTrue(changed);

            // Open a copy
            changed = false;
            Directory.CreateDirectory("projectDir");
            string proj2Path = Path.Combine("projectDir", TestContext.TestName + "2.rsproj");
            File.Copy(projPath, proj2Path);
            ProjectManager.OpenProject(proj2Path);
            Assert.AreEqual(proj2Path, ProjectManager.ActiveProject.Path);
            Assert.AreEqual(projName, ProjectManager.ActiveProject.Name);
        }
    }
}
