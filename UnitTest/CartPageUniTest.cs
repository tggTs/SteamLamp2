using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using SteamLamp;

namespace SteamLamp.Tests
{
    [TestClass]
    public class CartPageTests
    {
        // ---  ПОЛОЖИТЕЛЬНЫЕ ТЕСТЫ ---

        [TestMethod]
        public void TotalPrice_Calculation_ShouldBeCorrect()
        {
            var items = new List<Game>
            {
                new Game { Price = "500 руб." },
                new Game { Price = "1200 руб." },
                new Game { Price = "Бесплатно" }
            };

            double total = items.Sum(i => {
                if (string.IsNullOrWhiteSpace(i.Price)) return 0;
                string p = i.Price.ToLower().Replace("руб.", "").Replace(".", "").Replace(" ", "").Trim();
                return double.TryParse(p, out double res) ? res : 0;
            });

            Assert.AreEqual(1700, total);
        }

        [TestMethod]
        public void RemoveItem_ListCount_ShouldDecrease()
        {
            var game = new Game { Title = "Hades" };
            var items = new List<Game> { game, new Game { Title = "Portal" } };

            items.Remove(game);

            Assert.AreEqual(1, items.Count);
        }

        [TestMethod]
        public void Path_Formatting_ShouldApplyPackUri()
        {
            string imagePath = "images/game.png";
            string cleanPath = imagePath.Replace("\\", "/");
            if (!cleanPath.StartsWith("/")) cleanPath = "/" + cleanPath;
            string result = $"pack://application:,,,/{cleanPath.TrimStart('/')}";

            Assert.AreEqual("pack://application:,,,/images/game.png", result);
        }

        // --- ОШИБКИ ---

        [TestMethod]
        public void FORCE_FAIL_EmptyCartPurchase_ShouldBeRed()
        {
            var items = new List<Game>();            
            bool canProceedToPayment = items != null && items.Count > 0;
            Assert.IsTrue(canProceedToPayment, "КРАСНЫЙ: Система блокирует оплату пустой корзины, а тест требует продолжения!");
        }

        [TestMethod]
        public void FORCE_FAIL_InvalidPriceFormat_ShouldBeRed()
        {
            var game = new Game { Price = "Цена: 1000" };
            string p = game.Price.ToLower().Replace("руб.", "").Replace(".", "").Replace(" ", "").Trim();
            bool isParsed = double.TryParse(p, out _);

            Assert.IsTrue(isParsed, "КРАСНЫЙ: Парсер не смог обработать нестандартный формат цены!");
        }

        [TestMethod]
        public void FORCE_FAIL_NullReference_Refresh_ShouldBeRed()
        {
            List<Game> items = null;
            bool isInitialized = items != null;

            Assert.IsTrue(isInitialized, "КРАСНЫЙ: Список товаров не инициализирован (Null)!");
        }
    }
}