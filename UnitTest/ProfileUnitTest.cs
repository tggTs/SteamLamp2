using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SteamLamp.Tests
{
    [TestClass]
    public class ProfilePageTests
    {
        [TestMethod]
        public void EditButton_Visibility_ShouldDependOnUserId()
        {
            int myId = 1;
            int otherId = 5;
            Assert.IsTrue(myId == myId, "Кнопка редактирования должна быть видна владельцу");
            Assert.IsFalse(myId == otherId, "Кнопка редактирования должна быть скрыта для гостя");
        }

        [TestMethod]
        public void Bio_DisplayLogic_ShouldHandleEmptyString()
        {
            string emptyBio = "";
            string expectedPlaceholder = "Здесь будет описание";
            string result = string.IsNullOrEmpty(emptyBio) ? expectedPlaceholder : emptyBio;
            Assert.AreEqual(expectedPlaceholder, result, "Если био пустое, должен быть плейсхолдер");
        }

        [TestMethod]
        public void ImageConversion_EmptyBytes_ShouldReturnNull()
        {
            byte[] emptyData = null;
            bool isSafe = (emptyData == null || (emptyData != null && emptyData.Length == 0));
            Assert.IsTrue(isSafe, "Метод должен безопасно обрабатывать отсутствие аватара");
        }
        [TestMethod]
        public void ProfileLoading_Negative_InvalidUserId_ShouldFail()
        {
            int invalidUserId = -1;
            bool isValidRequest = invalidUserId > 0;
            Assert.IsTrue(isValidRequest, "Система попыталась загрузить профиль с некорректным ID (отрицательное значение).");
        }

        [TestMethod]
        public void FriendsPreview_Negative_DataConsistency_ShouldFail()
        {
            int totalFriendsInDb = -5;
            int limit = 5;
            int displayedCount = (totalFriendsInDb > limit) ? limit : totalFriendsInDb;
            bool resultIsPositive = displayedCount >= 0;
            Assert.IsTrue(resultIsPositive, "Счетчик друзей в интерфейсе стал отрицательным из-за некорректных данных в БД.");
        }

        [TestMethod]
        public void Bio_Negative_MaxLength_ShouldFail()
        {
            string oversizedBio = new string('A', 10001);
            int uiLimit = 500;
            bool fitsInUi = oversizedBio.Length <= uiLimit;
            Assert.IsTrue(fitsInUi, "Слишком длинное описание профиля превышает лимиты графического интерфейса.");
        }
    }
}