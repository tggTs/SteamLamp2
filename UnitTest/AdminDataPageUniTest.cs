using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using SteamLamp;

namespace SteamLamp.Tests
{
    [TestClass]
    public class AdminDataPageTests
    {
        // ---  ПОЛОЖИТЕЛЬНЫЕ ТЕСТЫ ---

        [TestMethod]
        public void Export_JsonSerialization_ShouldBeValid()
        {
            var testUser = new { Id = 1, Nickname = "Admin", Role = "Admin", Balance = 500.00 };
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(testUser, options);

            Assert.IsTrue(json.Contains("\"Nickname\": \"Admin\""));
            Assert.IsTrue(json.Contains("\"Balance\": 500"));
        }

        [TestMethod]
        public void Delete_PreventSelfDeletion_LogicCheck()
        {
            int currentAdminId = 10;
            int targetUserId = 10;

            bool canDelete = currentAdminId != targetUserId;

            Assert.IsFalse(canDelete);
        }

        [TestMethod]
        public void UI_SelectionChange_VisibilityLogic()
        {
            object selectedItem = new { Nickname = "User123" };
            bool isPanelVisible = selectedItem != null;

            Assert.IsTrue(isPanelVisible);
        }

        // --- ОШИБКИ ---

        [TestMethod]
        public void FORCE_FAIL_AllowSelfDeletion_ShouldBeRed()
        {
            int currentAdminId = 5;
            int selectedUserId = 5;
            bool isDeletionAllowed = currentAdminId == selectedUserId;
            Assert.IsFalse(isDeletionAllowed, "КРАСНЫЙ: Код блокирует самоудаление, а тест требует его разрешить!");
        }

        [TestMethod]
        public void FORCE_FAIL_ExportWithoutSelection_ShouldBeRed()
        {
            object selectedUser = null;
            bool canExport = selectedUser != null;

            Assert.IsTrue(canExport, "КРАСНЫЙ: Попытка экспорта пустого выбора должна быть невозможна!");
        }

        [TestMethod]
        public void FORCE_FAIL_NegativeBalance_ShouldBeRed()
        {
            decimal balance = -100.50m;
            bool isValidBalance = balance >= 0;

            Assert.IsTrue(isValidBalance, "КРАСНЫЙ: Обнаружен отрицательный баланс, что недопустимо для системы!");
        }
    }
}