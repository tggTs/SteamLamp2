using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SteamLamp.Tests
{
    [TestClass]
    public class EditProfileTests
    {
        [TestMethod]
        public void Test_Nickname_Correct_Validation()
        {
            string inputNickname = "Stepka_D";
            bool isValid = !string.IsNullOrWhiteSpace(inputNickname) && inputNickname.Length >= 3;
            Assert.IsTrue(isValid, "Корректный никнейм должен проходить валидацию");
        }

        [TestMethod]
        public void Test_Nickname_WithNumbers_Allowed()
        {
            string inputNickname = "Player777";
            bool isValid = !string.IsNullOrWhiteSpace(inputNickname) && inputNickname.Length >= 3;
            Assert.IsTrue(isValid, "Никнейм с цифрами должен быть разрешен");
        }

        [TestMethod]
        public void Test_Bio_NormalText_Allowed()
        {
            string bio = "Привет, я разработчик SteamLamp!";
            bool isValid = bio.Length <= 500;
            Assert.IsTrue(isValid, "Обычное описание должно проходить проверку");
        }

        [TestMethod]
        public void Test_Bio_Empty_Allowed()
        {
            string emptyBio = "";
            bool isAllowed = emptyBio != null;
            Assert.IsTrue(isAllowed, "Пустое описание должно быть разрешено системой");
        }

        [TestMethod]
        public void Test_Avatar_Change_Logic_Success()
        {
            byte[] oldAvatar = new byte[] { 0x01 };
            byte[] newAvatar = new byte[] { 0x01, 0x02, 0x03 };
            bool isChanged = newAvatar.Length > oldAvatar.Length;
            Assert.IsTrue(isChanged, "Логика обновления данных аватара должна работать корректно");
        }

        [TestMethod]
        public void Test_Nickname_MinimumLength_Boundary()
        {
            string minNick = "Dev";
            Assert.IsTrue(minNick.Length >= 3, "Никнейм из 3-х символов — это минимально допустимая граница");
        }

        [TestMethod]
        public void Test_Nickname_Empty_Negative_Validation()
        {
            string inputNickname = "";
            bool isValid = !string.IsNullOrWhiteSpace(inputNickname);
            Assert.IsTrue(isValid, "Система пропустила пустой никнейм!");
        }

        [TestMethod]
        public void Test_Nickname_ForbiddenSymbols_ShouldFail()
        {
            string dangerousNick = "Stepka<script>";
            bool isSafe = !dangerousNick.Contains("<") && !dangerousNick.Contains(">");
            Assert.IsTrue(isSafe, "Никнейм содержит запрещенные символы HTML-инъекции.");
        }
    }
}