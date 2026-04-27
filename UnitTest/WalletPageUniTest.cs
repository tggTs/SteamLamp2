using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using SteamLamp;

namespace SteamLamp.Tests
{
    [TestClass]
    public class WalletPageTests
    {
        // ---  ПОЛОЖИТЕЛЬНЫЕ ТЕСТЫ  ---

        [TestMethod]
        public void CardNumber_Validation_ShouldPassCorrectNumber()
        {
            string cardNumber = "1234 5678 1234 5678";
            string cleanCard = cardNumber.Replace(" ", "");

            bool isValid = cleanCard.Length == 16 && cleanCard.All(char.IsDigit);

            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void Balance_AddFunds_ShouldCalculateCorrectly()
        {
            decimal currentBalance = 150.00m;
            decimal topUpAmount = 500.00m;

            decimal newBalance = currentBalance + topUpAmount;

            Assert.AreEqual(650.00m, newBalance);
        }

        [TestMethod]
        public void Amount_Parsing_FromTag_ShouldBeCorrect()
        {
            string tagValue = "1000";
            decimal amount = decimal.Parse(tagValue);

            Assert.AreEqual(1000m, amount);
        }

        // --- ОШИБКИ ---

        [TestMethod]
        public void FORCE_FAIL_ShortCardNumber_ShouldBeAllowed_ShouldBeRed()
        {
            string card = "12341234"; 
            bool isAccepted = card.Length >= 16;

            Assert.IsTrue(isAccepted, "КРАСНЫЙ: Система должна принимать только полные номера карт, а тест пытается пропустить короткий!");
        }

        [TestMethod]
        public void FORCE_FAIL_NonDigitCard_ShouldBeAllowed_ShouldBeRed()
        {
            string card = "123456781234567A"; 
            bool isDigitsOnly = card.All(char.IsDigit);
            Assert.IsTrue(isDigitsOnly, "КРАСНЫЙ: В номере карты обнаружены недопустимые символы, которые код обязан блокировать!");
        }

        [TestMethod]
        public void FORCE_FAIL_NegativeTopUp_ShouldBeRed()
        {
            decimal amountToFill = -100.00m;
            bool isValidAmount = amountToFill > 0;
            Assert.IsTrue(isValidAmount, "КРАСНЫЙ: Попытка пополнения баланса на отрицательную сумму!");
        }
    }
}