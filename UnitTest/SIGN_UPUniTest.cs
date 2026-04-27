using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SteamLamp;

namespace SteamLamp.Tests
{
    [TestClass]
    public class SignUpTests
    {
        // ---  ПОЛОЖИТЕЛЬНЫЕ ТЕСТЫ  ---

        [TestMethod]
        public void Captcha_Calculation_ShouldBeCorrect()
        {
            int n1 = 5;
            int n2 = 10;
            int expected = 15;

            int actual = n1 + n2;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Validation_FieldsNotEmpty_ShouldPass()
        {
            string login = "user123";
            string nick = "Player";
            string pass = "12345";

            bool isValid = !string.IsNullOrEmpty(login) &&!string.IsNullOrEmpty(nick) &&!string.IsNullOrEmpty(pass);

            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void Session_AfterLogin_ShouldStoreUser()
        {
            var user = new User { Login = "admin", Nickname = "Boss" };
            Session.CurrentUser = user;

            Assert.IsNotNull(Session.CurrentUser);
            Assert.AreEqual("admin", Session.CurrentUser.Login);
        }

        // --- ОШИБКИ) ---

        [TestMethod]
        public void FORCE_FAIL_WrongCaptcha_ShouldProceed_ShouldBeRed()
        {
            int correctCaptcha = 20;
            string userInput = "15";
            bool isCaptchaValid = userInput == correctCaptcha.ToString();

            Assert.IsTrue(isCaptchaValid, "КРАСНЫЙ: Тест требует принять неверную капчу, но система её блокирует!");
        }

        [TestMethod]
        public void FORCE_FAIL_ShortPassword_ShouldBeAllowed_ShouldBeRed()
        {
            string password = "1";
            bool isStrongEnough = password.Length > 3;

            Assert.IsTrue(isStrongEnough, "КРАСНЫЙ: Тест требует сложный пароль, а введён слишком короткий!");
        }

        [TestMethod]
        public void FORCE_FAIL_DuplicateLogin_ShouldBeAllowed_ShouldBeRed()
        {
            bool loginExistsInDb = true;
            bool canRegister = !loginExistsInDb;

            Assert.IsTrue(canRegister, "КРАСНЫЙ: Тест требует разрешить дубликат логина, но база данных это запрещает!");
        }
    }
}