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
    public class DiscountCodeUsageTests
    {
        private const string TestConnectionString = "YOUR CONNECTIONSTRING";

        [Test]
        public void UseDiscountCode_ValidCode_RemovesFromGeneratedCodesAndAddsToUsedCodes()
        {
            // Arrange
            string validCode = "VALIDCODE";
            AddCodeToDatabase(validCode);

            // Act
            byte result = CommonHelper.UseDiscountCode(validCode);

            // Assert
            Assert.AreEqual(0, result);
            AssertCodeMovedToUsedCodes(validCode);
        }

        [Test]
        public void UseDiscountCode_InvalidCode_Returns1()
        {
            // Arrange
            string invalidCode = "INVALIDCODE";

            // Act
            byte result = CommonHelper.UseDiscountCode(invalidCode);

            // Assert
            Assert.AreEqual(1, result);
        }

        private void AddCodeToDatabase(string code)
        {
            using (SqlConnection connection = new SqlConnection(TestConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("INSERT INTO GeneratedCodes (Code) VALUES (@Code)", connection);
                command.Parameters.AddWithValue("@Code", code);
                command.ExecuteNonQuery();
            }
        }

        private void AssertCodeMovedToUsedCodes(string code)
        {
            using (SqlConnection connection = new SqlConnection(TestConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM UsedCodes WHERE Code = @Code", connection);
                command.Parameters.AddWithValue("@Code", code);
                int count = (int)command.ExecuteScalar();

                Assert.AreEqual(1, count);

                command = new SqlCommand("SELECT COUNT(*) FROM GeneratedCodes WHERE Code = @Code", connection);
                command.Parameters.AddWithValue("@Code", code);
                count = (int)command.ExecuteScalar();

                Assert.AreEqual(0, count);
            }
        }


    }
}
