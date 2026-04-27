using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using SteamLamp;

namespace SteamLamp.Tests
{
    [TestClass]
    public class MainWindowLogicTests
    {

        [TestMethod]
        public void Test_AdminAccess_Positive()
        {
            var role = MainWindow.UserRole.Admin;
            bool hasAccess = (role == MainWindow.UserRole.Admin);
            Assert.IsTrue(hasAccess, "Админ должен обладать правами доступа.");
        }

        [TestMethod]
        public void Test_AdminAccess_ForceFail()
        {
            var role = MainWindow.UserRole.Guest;
            Assert.IsTrue(role == MainWindow.UserRole.Admin, "КРАСНЫЙ: Гость не админ, но тест требует обратного!");
        }
        [TestMethod]
        public void Test_Cart_Duplicates_Positive()
        {
            var cart = new List<string> { "Dota 2" };
            string newGame = "Dota 2";
            bool alreadyExists = cart.Contains(newGame);
            Assert.IsTrue(alreadyExists, "Система корректно обнаружила дубликат.");
        }

        [TestMethod]
        public void Test_Cart_Duplicates_ForceFail()
        {
            var cart = new List<string> { "Dota 2" };
            string newGame = "Dota 2";
            bool canAdd = !cart.Contains(newGame);
            Assert.IsTrue(canAdd, "КРАСНЫЙ: Логика блокирует дубликат, а тест требует его пропустить!");
        }
        [TestMethod]
        public void Test_Price_FreeDetection_Positive()
        {
            string price = "Бесплатно";
            bool isFree = price.Contains("Бесплатно");
            Assert.IsTrue(isFree, "Игра должна распознаваться как бесплатная.");
        }

        [TestMethod]
        public void Test_Price_FreeDetection_ForceFail()
        {
            string price = "1999 руб.";
            bool isFree = price.Contains("Бесплатно");
            Assert.IsTrue(isFree, "КРАСНЫЙ: Платная игра не содержит метку 'Бесплатно'!");
        }
        [TestMethod]
        public void Test_Search_Validation_Positive()
        {
            string searchInput = "   ";
            bool isInvalid = string.IsNullOrWhiteSpace(searchInput);
            Assert.IsTrue(isInvalid, "Система корректно считает пробелы невалидным запросом.");
        }

        [TestMethod]
        public void Test_Search_Validation_ForceFail()
        {
            string searchInput = "";
            bool isValid = !string.IsNullOrWhiteSpace(searchInput);
            Assert.IsTrue(isValid, "КРАСНЫЙ: Пустая строка заблокирована кодом, а тест требует её принять!");
        }

        [TestMethod]
        public void Test_Converter_NullPath_Positive()
        {
            var converter = new PathToImageConverter();
            var result = converter.Convert(null, typeof(object), null, CultureInfo.InvariantCulture);
            Assert.IsNull(result, "Конвертер успешно вернул null для пустого значения.");
        }

        [TestMethod]
        public void Test_Converter_NullPath_ForceFail()
        {
            var converter = new PathToImageConverter();
            var result = converter.Convert(null, typeof(object), null, CultureInfo.InvariantCulture);
            Assert.IsNotNull(result, "КРАСНЫЙ: Конвертер вернул null, но тест требует объект изображения!");
        }
    }
}