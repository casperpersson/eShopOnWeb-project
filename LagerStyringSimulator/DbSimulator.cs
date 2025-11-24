using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockControleSimulator
{
    public class DbSimulator
    {
        private Dictionary<int, int> DbSim;
        private Dictionary<int, (int Quantity, DateTime Expiry, string BasketId)> ReservedItems = new();

        public DbSimulator()
        {
            DbSim = [];
            ReservedItems = [];
            SeedDB();
            StartExpiryMonitor();
        }

        public bool ReserveItem(int id, int quantity, string basketId)
        {
            if (DbSim.ContainsKey(id) && DbSim[id] >= quantity)
            {
                DbSim[id] -= quantity;
                ReservedItems[id] = (quantity, DateTime.UtcNow.AddMinutes(10), basketId);
                return true;
            }
            return false;
        }

        public void ReleaseExpiredReservations()
        {
            var now = DateTime.UtcNow;
            var expired = ReservedItems.Where(r => r.Value.Expiry <= now).ToList();
            foreach (var kvp in expired)
            {
                DbSim[kvp.Key] += kvp.Value.Quantity;
                ReservedItems.Remove(kvp.Key);
                Console.WriteLine($"Released {kvp.Value.Quantity} of item {kvp.Key} from basket {kvp.Value.BasketId}");
            }
        }

        public void StartExpiryMonitor()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    ReleaseExpiredReservations();
                    await Task.Delay(TimeSpan.FromMinutes(1));
                }
            });
        }


        private void SeedDB()
        {
            if (DbSim != null)
            {
                foreach (var item in items)
                {
                    DbSim.Add(item.Id, 50);
                }
            }
        }



        private readonly List<CatalogItem> items = new List<CatalogItem>() {

                // Constructor: (catalogTypeId, catalogBrandId, description, name, price, pictureUri)
                new(1, 2, 2, ".NET Bot Black Sweatshirt", ".NET Bot Black Sweatshirt", 19.5M, "http://catalogbaseurltobereplaced/images/products/1.png"),
                new(2, 1, 2, ".NET Black & White Mug", ".NET Black & White Mug", 8.50M, "http://catalogbaseurltobereplaced/images/products/2.png"),
                new(3, 2, 5, "Prism White T-Shirt", "Prism White T-Shirt", 12M, "http://catalogbaseurltobereplaced/images/products/3.png"),
                new(4, 2, 2, ".NET Foundation Sweatshirt", ".NET Foundation Sweatshirt", 12M, "http://catalogbaseurltobereplaced/images/products/4.png"),
                new(5, 3, 5, "Roslyn Red Sheet", "Roslyn Red Sheet", 8.5M, "http://catalogbaseurltobereplaced/images/products/5.png"),
                new(6, 2, 2, ".NET Blue Sweatshirt", ".NET Blue Sweatshirt", 12M, "http://catalogbaseurltobereplaced/images/products/6.png"),
                new(7, 2, 5, "Roslyn Red T-Shirt", "Roslyn Red T-Shirt", 12M, "http://catalogbaseurltobereplaced/images/products/7.png"),
                new(8, 2, 5, "Kudu Purple Sweatshirt", "Kudu Purple Sweatshirt", 8.5M, "http://catalogbaseurltobereplaced/images/products/8.png"),
                new(9, 1, 5, "Cup<T> White Mug", "Cup<T> White Mug", 12M, "http://catalogbaseurltobereplaced/images/products/9.png"),
                new(10, 3, 2, ".NET Foundation Sheet", ".NET Foundation Sheet", 12M, "http://catalogbaseurltobereplaced/images/products/10.png"),
                new(11, 3, 2, "Cup<T> Sheet", "Cup<T> Sheet", 8.5M, "http://catalogbaseurltobereplaced/images/products/11.png"),
                new(12, 2, 5, "Prism White TShirt", "Prism White TShirt", 12M, "http://catalogbaseurltobereplaced/images/products/12.png")
        };


        private record CatalogItem(int Id, int CatelogTypeId, int CatelogBrandId, string Description, string Name, decimal Price, string PictureUri);
    }

}
