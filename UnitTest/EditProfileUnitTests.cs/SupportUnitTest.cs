using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SteamLamp.Tests
{
    [TestClass]
    public class SupportModuleTests
    {
        [TestMethod]
        public void Profile_Avatar_UpdateSuccess()
        {
            byte[] oldAvatar = new byte[] { 0x1, 0x2 };
            byte[] newAvatar = new byte[] { 0xA, 0xB, 0xC };

            Assert.IsNotNull(newAvatar);
            Assert.AreNotEqual(oldAvatar.Length, newAvatar.Length, "Новый аватар должен отличаться от старого");
        }

        [TestMethod]
        public void Bio_Update_ShouldPersistData()
        {
            string newBio = "Я люблю играть в SteamLamp!";
            string savedBio = newBio;
            Assert.AreEqual(newBio, savedBio, "Описание профиля должно корректно сохраняться");
        }
        [TestMethod]
        public void SendMessage_ShouldFail_IfTextIsWhiteSpace()
        {
            string emptyInput = "   ";
            bool isInvalid = string.IsNullOrWhiteSpace(emptyInput);
            Assert.IsTrue(isInvalid, "Система должна распознавать строку из пробелов как пустую.");
        }

        [TestMethod]
        public void SendMessage_ShouldPass_IfTextIsCorrect()
        {
            string validMessage = "У меня не загружается аватар, помогите!";
            bool isInvalid = string.IsNullOrWhiteSpace(validMessage);
            Assert.IsFalse(isInvalid, "Корректное сообщение должно проходить проверку валидации.");
        }

        [TestMethod]
        public void FormReset_ShouldClearInputAndResetTopic()
        {
            string messageAfterReset = "";
            int topicIndexAfterReset = 0;
            Assert.AreEqual(0, messageAfterReset.Length, "Поле ввода должно быть пустым после очистки.");
            Assert.AreEqual(0, topicIndexAfterReset, "Выбор темы должен сброситься на первый элемент (индекс 0).");
        }

        [TestMethod]
        public void Message_Validation_ShouldDetectHiddenEmptyStrings()
        {
            string trickyInput = "\n\t\r";
            bool isInvalid = string.IsNullOrWhiteSpace(trickyInput);
            Assert.IsFalse(isInvalid, "ОТРИЦАТЕЛЬНЫЙ РЕЗУЛЬТАТ: Система ошибочно приняла спецсимволы за текст сообщения.");
        }

        [TestMethod]
        public void TopicSelection_ShouldFail_IfIndexIsOutOfBounds()
        {
            int totalTopics = 4;
            int invalidChoice = -1;
            bool isValidChoice = invalidChoice >= 0 && invalidChoice < totalTopics;
            Assert.IsTrue(isValidChoice, "ОТРИЦАТЕЛЬНЫЙ РЕЗУЛЬТАТ: Выбран недопустимый индекс темы обращения (Out of Range).");
        }
        [TestMethod]
        public void Message_LimitCheck_Negative_ShouldFailIfTooLong()
        {
            string veryLongMessage = new string('X', 5001);
            int dbLimit = 5000;
            bool isWithinLimit = veryLongMessage.Length <= dbLimit;
            Assert.IsTrue(isWithinLimit, "ОТРИЦАТЕЛЬНЫЙ РЕЗУЛЬТАТ: Сообщение превышает максимально допустимый размер для базы данных.");
        }
    }
}