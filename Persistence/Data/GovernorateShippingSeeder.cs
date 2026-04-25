using Domain.Entities.OrderEntities;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;

namespace Persistence.Data
{
    public static class GovernorateShippingSeeder
    {
        public static async Task SeedGovernoratesAsync(StoreContext context)
        {
            // Check if governorates already exist
            if (await context.GovernorateShippingPrices.AnyAsync())
                return;

            var governorates = new List<GovernorateShippingPrice>
            {
                new GovernorateShippingPrice { GovernorateName = "Cairo", ShippingPrice = 70, DeliveryDays = 7, IsActive = true },
                new GovernorateShippingPrice { GovernorateName = "Alexandria", ShippingPrice = 70, DeliveryDays = 7, IsActive = true },
                new GovernorateShippingPrice { GovernorateName = "Giza", ShippingPrice = 70, DeliveryDays = 7, IsActive = true },
                new GovernorateShippingPrice { GovernorateName = "Qalyubia", ShippingPrice = 70, DeliveryDays = 7, IsActive = true },
                new GovernorateShippingPrice { GovernorateName = "Port Said", ShippingPrice = 70, DeliveryDays = 7, IsActive = true },
                new GovernorateShippingPrice { GovernorateName = "Suez", ShippingPrice = 70, DeliveryDays = 7, IsActive = true },
                new GovernorateShippingPrice { GovernorateName = "Luxor", ShippingPrice = 70, DeliveryDays = 7, IsActive = true },
                new GovernorateShippingPrice { GovernorateName = "Aswan", ShippingPrice = 70, DeliveryDays = 7, IsActive = true },
                new GovernorateShippingPrice { GovernorateName = "Asyut", ShippingPrice = 70, DeliveryDays = 7, IsActive = true },
                new GovernorateShippingPrice { GovernorateName = "Beheira", ShippingPrice = 70, DeliveryDays = 7, IsActive = true },
                new GovernorateShippingPrice { GovernorateName = "Beni Suef", ShippingPrice = 70, DeliveryDays = 7, IsActive = true },
                new GovernorateShippingPrice { GovernorateName = "Dakahlia", ShippingPrice = 70, DeliveryDays = 7, IsActive = true },
                new GovernorateShippingPrice { GovernorateName = "Damietta", ShippingPrice = 70, DeliveryDays = 7, IsActive = true },
                new GovernorateShippingPrice { GovernorateName = "Faiyum", ShippingPrice = 70, DeliveryDays = 7, IsActive = true },
                new GovernorateShippingPrice { GovernorateName = "Gharbia", ShippingPrice = 70, DeliveryDays = 7, IsActive = true },
                new GovernorateShippingPrice { GovernorateName = "Ismailia", ShippingPrice = 70, DeliveryDays = 7, IsActive = true },
                new GovernorateShippingPrice { GovernorateName = "Kafr El Sheikh", ShippingPrice = 70, DeliveryDays = 7, IsActive = true },
                new GovernorateShippingPrice { GovernorateName = "Matruh", ShippingPrice = 70, DeliveryDays = 7, IsActive = true },
                new GovernorateShippingPrice { GovernorateName = "Minya", ShippingPrice = 70, DeliveryDays = 7, IsActive = true },
                new GovernorateShippingPrice { GovernorateName = "Monufia", ShippingPrice = 70, DeliveryDays = 7, IsActive = true },
                new GovernorateShippingPrice { GovernorateName = "New Valley", ShippingPrice = 70, DeliveryDays = 7, IsActive = true },
                new GovernorateShippingPrice { GovernorateName = "North Sinai", ShippingPrice = 70, DeliveryDays = 7, IsActive = true },
                new GovernorateShippingPrice { GovernorateName = "Qena", ShippingPrice = 70, DeliveryDays = 7, IsActive = true },
                new GovernorateShippingPrice { GovernorateName = "Red Sea", ShippingPrice = 70, DeliveryDays = 7, IsActive = true },
                new GovernorateShippingPrice { GovernorateName = "Sharqia", ShippingPrice = 70, DeliveryDays = 7, IsActive = true },
                new GovernorateShippingPrice { GovernorateName = "Sohag", ShippingPrice = 70, DeliveryDays = 7, IsActive = true },
                new GovernorateShippingPrice { GovernorateName = "South Sinai", ShippingPrice = 70, DeliveryDays = 7, IsActive = true }
            };

            await context.GovernorateShippingPrices.AddRangeAsync(governorates);
            await context.SaveChangesAsync();

            Console.WriteLine($"✅ Seeded {governorates.Count} Egyptian governorates with default shipping (70 EGP, 7 days)");
        }
    }
}
