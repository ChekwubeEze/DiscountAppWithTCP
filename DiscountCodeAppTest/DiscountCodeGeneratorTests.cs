using DiscountCodeAppServer.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscountCodeAppTest
{
    [TestFixture]
    public class DiscountCodeGeneratorTests
    {
        private const string TestConnectionString = "YOUR CONNECTIONSTRING";
        [Test]
        public void GenerateDiscountCodes_InsertsCodesIntoDatabase()
        {
            // Arrange
            int count = 10;
            int length = 8;

            // Act
            bool result = CommonHelper.GenerateDiscountCodes(count, length);

            // Assert
            Assert.IsTrue(result);
            using (SqlConnection connection = new SqlConnection(TestConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM GeneratedCodes", connection);
                int actualCount = (int)command.ExecuteScalar();

                AssertGeneratedCodesInDatabase(actualCount);
            }

        }

        private void AssertGeneratedCodesInDatabase(int expectedCount)
        {
            using (SqlConnection connection = new SqlConnection(TestConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM GeneratedCodes", connection);
                int actualCount = (int)command.ExecuteScalar();

                Assert.AreEqual(expectedCount, actualCount);
            }
        }


    }
}
