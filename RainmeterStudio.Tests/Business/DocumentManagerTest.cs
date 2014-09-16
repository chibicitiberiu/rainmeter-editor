using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RainmeterStudio.Business;

namespace RainmeterStudio.Tests.Business
{
    [TestClass]
    public class DocumentManagerTest
    {
        DocumentManager documentManager;

        [TestInitialize]
        public void Initialize()
        {
            documentManager = new DocumentManager();
        }
    }
}
