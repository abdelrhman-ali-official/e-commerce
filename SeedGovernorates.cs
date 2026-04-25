using System;
using System.Data.SqlClient;

namespace SeedGovernorates
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Server=db32611.public.databaseasp.net; Database=db32611; User Id=db32611; Password=Zt7-%Pz6C8#c; Encrypt=False; MultipleActiveResultSets=True; Connection Timeout=60; Command Timeout=120;";
            
            var governorates = new[]
            {
                "Cairo", "Alexandria", "Giza", "Qalyubia", "Port Said", "Suez", "Luxor", "Aswan", 
                "Asyut", "Beheira", "Beni Suef", "Dakahlia", "Damietta", "Faiyum", "Gharbia", 
                "Ismailia", "Kafr El Sheikh", "Matruh", "Minya", "Monufia", "New Valley", 
                "North Sinai", "Qena", "Red Sea", "Sharqia", "Sohag", "South Sinai"
            };

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                
                // Check if table has records
                var checkCmd = new SqlCommand("SELECT COUNT(*) FROM GovernorateShippingPrices", connection);
                var count = (int)checkCmd.ExecuteScalar();
                
                if (count > 0)
                {
                    Console.WriteLine($"✅ Database already has {count} governorates");
                    
                    // Show them
                    var selectCmd = new SqlCommand("SELECT GovernorateName, ShippingPrice, DeliveryDays FROM GovernorateShippingPrices ORDER BY GovernorateName", connection);
                    using (var reader = selectCmd.ExecuteReader())
                    {
                        Console.WriteLine("\n📋 Available Governorates:");
                        while (reader.Read())
                        {
                            Console.WriteLine($"  - {reader.GetString(0)} (Price: {reader.GetDecimal(1)} EGP, Days: {reader.GetInt32(2)})");
                        }
                    }
                    return;
                }
                
                // Insert governorates
                Console.WriteLine("🌱 Seeding governorates...");
                foreach (var gov in governorates)
                {
                    var insertCmd = new SqlCommand(
                        "INSERT INTO GovernorateShippingPrices (GovernorateName, ShippingPrice, DeliveryDays, IsActive) VALUES (@name, 70, 7, 1)", 
                        connection);
                    insertCmd.Parameters.AddWithValue("@name", gov);
                    insertCmd.ExecuteNonQuery();
                    Console.WriteLine($"  ✅ Added: {gov}");
                }
                
                Console.WriteLine($"\n✅ Successfully seeded {governorates.Length} governorates!");
            }
        }
    }
}
