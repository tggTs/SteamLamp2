using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SteamLamp;

namespace SteamLamp.Tests
{
    [TestClass]
    public class PaymentPageTests
    {
        // ---  ПОЛОЖИТЕЛЬНЫЕ ТЕСТЫ ---

        [TestMethod]
        public void Regex_PriceParsing_ShouldHandleSymbols()
        {
            string inputPrice = "1.299,50 руб.";
            string priceRaw = Regex.Replace(inputPrice, @"[^0-9,.]", "").Replace(".", ",");
            bool isParsed = decimal.TryParse(priceRaw, out decimal result);

            Assert.IsTrue(isParsed);
            Assert.AreEqual(1299.50m, result);
        }

        [TestMethod]
        public void Balance_Check_SufficientFunds_ShouldPass()
        {
            decimal userBalance = 1000.00m;
            decimal totalAmount = 750.50m;

            bool canPay = userBalance >= totalAmount;

            Assert.IsTrue(canPay);
        }

        [TestMethod]
        public void Payment_Deduction_Calculation_IsCorrect()
        {
            decimal initialBalance = 500.00m;
            decimal cost = 150.00m;

            decimal finalBalance = initialBalance - cost;

            Assert.AreEqual(350.00m, finalBalance);
        }

        // --- ОШИБК ---

        [TestMethod]
        public void FORCE_FAIL_InsufficientFunds_ShouldProceed_ShouldBeRed()
        {
            decimal userBalance = 50.00m;
            decimal totalAmount = 500.00m;
            bool canProceed = userBalance >= totalAmount;

            Assert.IsTrue(canProceed, "КРАСНЫЙ: Тест требует начать оплату при нехватке денег, но код это запрещает!");
        }

        [TestMethod]
        public void FORCE_FAIL_EmptyGameList_TotalShouldBePositive_ShouldBeRed()
        {
            var items = new List<Game>();

            decimal total = items.Count == 0 ? 0 : 100;

            Assert.IsTrue(total > 0, "КРАСНЫЙ: Сумма оплаты не может быть нулевой, проверьте содержимое корзины!");
        }

        [TestMethod]
        public void FORCE_FAIL_DoubleDotPrice_Parsing_ShouldBeRed()
        {
            string brokenPrice = "100.50.10";
            string priceRaw = Regex.Replace(brokenPrice, @"[^0-9,.]", "").Replace(".", ",");
            bool isParsed = decimal.TryParse(priceRaw, out _);
            Assert.IsTrue(isParsed, "КРАСНЫЙ: Регулярное выражение очистило строку, но формат числа остался неверным для парсинга!");
        }
    }
}