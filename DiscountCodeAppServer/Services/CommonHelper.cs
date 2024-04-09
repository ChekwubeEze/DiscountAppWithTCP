using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DiscountCodeAppServer.Services
{
    public static class CommonHelper
    {
        private static readonly string connectionString = "YOUR CONNECTIONSTRING";
        public static string GenerateCode(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var value = new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            return value;
        }

        public static bool GenerateDiscountCodes(int count, int length)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                for (int i = 0; i < count; i++)
                {
                    string code = GenerateCode(length);
                    SqlCommand command2 = new SqlCommand("SELECT * FROM GeneratedCodes", connection);
                    bool exist = command2.Parameters.Contains(code);
                    if (exist)
                    {
                        continue;
                    }
                    SqlCommand command = new SqlCommand("INSERT INTO GeneratedCodes (Code) VALUES (@Code)", connection);
                    command.Parameters.AddWithValue("@Code", code);
                    command.ExecuteNonQuery();
                }
            }
            return true;
        }

        public static byte UseDiscountCode(string code)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM GeneratedCodes WHERE Code = @Code", connection);
                command.Parameters.AddWithValue("@Code", code);
                int count = (int)command.ExecuteScalar();
                if (count > 0)
                {
                    command = new SqlCommand("DELETE FROM GeneratedCodes WHERE Code = @Code", connection);
                    command.Parameters.AddWithValue("@Code", code);
                    command.ExecuteNonQuery();

                    SqlCommand command2 = new SqlCommand("INSERT INTO UsedCodes (Code) VALUES (@Code)", connection);
                    command2.Parameters.AddWithValue("@Code", code);
                    command2.ExecuteNonQuery();

                    Console.WriteLine("Code " + code + " used successfully");
                    return 0; // Success
                }
                else
                {
                    Console.WriteLine("Invalid code: " + code);
                    return 1; // Invalid code
                }
            }
        }
        public static void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string request = Encoding.ASCII.GetString(buffer, 0, bytesRead);

            string[] requestData = request.Split(',');
            string response = "";

            if (requestData[0] == "generate")
            {
                int count = int.Parse(requestData[1]);
                int length = int.Parse(requestData[2]);
                if (length < 7 || length > 8)
                {
                    length = 8;
                }
                response = GenerateDiscountCodes(count, length).ToString();
            }
            else if (requestData[0] == "use")
            {
                string code = requestData[1];
                byte result = UseDiscountCode(code);
                response = result.ToString();
            }

            byte[] responseBuffer = Encoding.ASCII.GetBytes(response);
            stream.Write(responseBuffer, 0, responseBuffer.Length);

            client.Close();
        }

    }
}
