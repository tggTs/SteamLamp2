using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SteamLamp.UI.Forms;

namespace SteamLamp.Tests
{
    [TestClass]
    public class AddGameWindowTests
    {

        [TestMethod]
        public void Validation_AllFieldsCorrect_ShouldPass()
        {

            string title = "S.T.A.L.K.E.R. 2";
            string desc = "Heart of Chornobyl";
            string dev = "GSC Game World";
            string preview = "C:/img/preview.png";
            string banner = "C:/img/banner.png";
            string priceStr = "2499";
            bool isPriceValid = decimal.TryParse(priceStr, out _);
            bool isFormValid = !string.IsNullOrWhiteSpace(title) &&!string.IsNullOrWhiteSpace(desc) &&!string.IsNullOrWhiteSpace(dev) &&!string.IsNullOrWhiteSpace(preview) &&!string.IsNullOrWhiteSpace(banner) &&isPriceValid;

            Assert.IsTrue(isFormValid, "Форма заполнена верно, тест должен пройти.");
        }

        [TestMethod]
        public void Progress_TimerStep_CalculationIsCorrect()
        {
            double totalDuration = 5;
            double step = (50.0 / (totalDuration * 1000.0)) * 100.0;

            Assert.AreEqual(1.0, step, 0.001, "Шаг таймера за 50мс при 5сек должен быть ровно 1%.");
        }
        [TestMethod]
        public void Validation_InvalidPrice_ShouldBeBlocked()
        {
            string invalidPrice = "Цена: сто рублей";
            bool isPriceValid = decimal.TryParse(invalidPrice, out _);

            Assert.IsFalse(isPriceValid, "Система не должна принимать текст вместо числа в поле цены.");
        }

        [TestMethod]
        public void Validation_EmptyTitle_ShouldBeBlocked()
        {
            string emptyTitle = "   ";
            bool isTitleValid = !string.IsNullOrWhiteSpace(emptyTitle);

            Assert.IsFalse(isTitleValid, "Пустое название игры должно блокировать отправку.");
        }

        [TestMethod]
        public void FORCE_FAIL_AllowEmptyImages_ShouldBeRed()
        {
            string previewPath = "";
            string bannerPath = "";
            bool isValid = !string.IsNullOrWhiteSpace(previewPath) && !string.IsNullOrWhiteSpace(bannerPath);

            Assert.IsTrue(isValid, "КРАСНЫЙ: Тест требует разрешить публикацию без картинок, но код это блокирует!");
        }

        [TestMethod]
        public void FORCE_FAIL_PriceCannotBeZero_ShouldBeRed()
        {
            decimal price = 0;
            bool isPaid = price > 0;

            Assert.IsTrue(isPaid, "КРАСНЫЙ: Тест требует, чтобы игра была платной, но введена цена 0.");
        }
    }
}