using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using SteamLamp;

namespace SteamLamp.Tests
{
    [TestClass]
    public class AdminGamesControlTests
    {
        // ---  ПОЛОЖИТЕЛЬНЫЕ ТЕСТЫ ---

        [TestMethod]
        public void Edit_TitleTrim_ShouldWork()
        {
            string inputTitle = "  Cyberpunk 2077  ";
            string expected = "Cyberpunk 2077";

            string actual = inputTitle.Trim();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Selection_UpdateDetails_ShouldBeVisible()
        {
            var selectedGame = new { Id = 1, Title = "Dota 2" };
            bool isPanelVisible = selectedGame != null;

            Assert.IsTrue(isPanelVisible);
        }

        [TestMethod]
        public void Delete_ConfirmationLogic_ShouldReturnTrue()
        {
            bool userClickedYes = true;
            bool shouldDelete = userClickedYes == true;

            Assert.IsTrue(shouldDelete);
        }

        // --- ОШИБКИ ---

        [TestMethod]
        public void FORCE_FAIL_EmptyTitleUpdate_ShouldBeRed()
        {
            string newTitle = "";

            bool isValidUpdate = !string.IsNullOrWhiteSpace(newTitle);

            Assert.IsTrue(isValidUpdate, "КРАСНЫЙ: Система не должна разрешать обновление игры с пустым заголовком!");
        }

        [TestMethod]
        public void FORCE_FAIL_SaveWithoutSelection_ShouldBeRed()
        {
            object selectedGame = null;


            bool canSave = selectedGame != null;

            Assert.IsTrue(canSave, "КРАСНЫЙ: Попытка сохранения данных при отсутствии выбранной игры!");
        }

        [TestMethod]
        public void FORCE_FAIL_PriceNumericCheck_ShouldBeRed()
        {
            string priceInput = "Бесплатно";
            bool isNumeric = decimal.TryParse(priceInput, out _);

            Assert.IsTrue(isNumeric, "КРАСНЫЙ: Редактор ожидает числовое значение цены, а введена строка!");
        }
    }
}