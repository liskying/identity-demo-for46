using System.Threading.Tasks;
// <copyright file="ApplicationUserTestTest.cs" company="Microsoft">Copyright © Microsoft 2013</copyright>

using System;
using Identity.Test;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Identity.Test.Tests
{
    [TestClass]
    [PexClass(typeof(ApplicationUserTest))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class ApplicationUserTestTest
    {

        [PexMethod(MaxBranches = 20000)]
        public Task ApplicationUserCreateTest([PexAssumeUnderTest]ApplicationUserTest target)
        {
            Task result = target.ApplicationUserCreateTest();
            return result;
            // TODO: 将断言添加到 方法 ApplicationUserTestTest.ApplicationUserCreateTest(ApplicationUserTest)
        }
    }
}
