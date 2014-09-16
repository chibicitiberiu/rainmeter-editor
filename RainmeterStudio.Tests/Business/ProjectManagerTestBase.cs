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
    /// <summary>
    /// Common stuff for project manager tests
    /// </summary>
    public class ProjectManagerTestBase
    {
        #region Project template

        /// <summary>
        /// A sample project template
        /// </summary>
        protected class TestTemplate : IProjectTemplate
        {
            public string Name
            {
                get { return "TestTemplate"; }
            }

            public IEnumerable<Property> Properties
            {
                get { return Enumerable.Empty<Property>(); }
            }

            public Project CreateProject()
            {
                return new Project();
            }
        }

        #endregion

        /// <summary>
        /// Gets or sets the text context
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Gets or sets the project manager
        /// </summary>
        protected virtual ProjectManager ProjectManager { get; set; }

        /// <summary>
        /// Gets or sets the sample project template
        /// </summary>
        protected virtual IProjectTemplate ProjectTemplate { get; set; }

        /// <summary>
        /// Sets up the test
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            string testDirectory = Path.Combine(TestContext.DeploymentDirectory, TestContext.TestName);

            Directory.CreateDirectory(testDirectory);
            Directory.SetCurrentDirectory(testDirectory);

            // Set up project manager
            ProjectManager = new ProjectManager();

            // Set up project template
            ProjectTemplate = new TestTemplate();
            ProjectManager.RegisterProjectTemplate(ProjectTemplate);

            OnInitialize();
        }

        public virtual void OnInitialize()
        {

        }
    }
}
