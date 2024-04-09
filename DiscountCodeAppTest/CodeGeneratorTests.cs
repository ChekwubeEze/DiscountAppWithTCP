using DiscountCodeAppServer.Services;
using NUnit.Framework;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscountCodeAppTest
{
    [TestFixture]
    public class CodeGeneratorTests
    {
        [Test]
        public void GenerateCode_ReturnsCorrectLength()
        {
            // Arrange
            int length = 10;

            // Act
            string code = CommonHelper.GenerateCode(length);

            // Assert
            Assert.AreEqual(length, code.Length);
        }

        [Test]
        public void GenerateCode_ReturnsOnlyUpperCaseLetters()
        {
            // Arrange
            int length = 10;

            // Act
            string code = CommonHelper.GenerateCode(length);

            // Assert
            Assert.IsTrue(System.Text.RegularExpressions.Regex.IsMatch(code, "^[A-Z]+$"));
        }
    }
}
