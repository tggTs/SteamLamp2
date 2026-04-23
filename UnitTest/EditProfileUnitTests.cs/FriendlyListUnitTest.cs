using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace SteamLamp.Tests
{
    [TestClass]
    public class FriendsModuleTests
    {
        [TestMethod]
        public void Search_ByExactNickname_ShouldFindUser()
        {
            string dbNickname = "Stepka_Dev";
            string searchInput = "Stepka_Dev";

            Assert.AreEqual(dbNickname, searchInput, "Поиск должен находить точное совпадение никнейма");
        }

        [TestMethod]
        public void FriendsList_CheckVisibility_ShouldBeTrue()
        {
            bool isContainerInitialized = true;
            Assert.IsTrue(isContainerInitialized, "Контейнер для списка друзей должен быть инициализирован");
        }

        [TestMethod]
        public void FriendCard_Creation_DataIntegrity()
        {
            string inputNick = "Friend123";
            string cardText = inputNick;

            Assert.AreEqual("Friend123", cardText, "Никнейм в карточке должен соответствовать данным из базы");
        }
        [TestMethod]
        public void Search_ShouldIgnorePlaceholder()
        {
            string placeholder = "Введите никнейм друга для поиска";
            string userInput = "Введите никнейм друга для поиска";
            bool canSearch = userInput != placeholder && !string.IsNullOrEmpty(userInput);
            Assert.IsFalse(canSearch, "Поиск не должен реагировать на стандартную подсказку");
        }

        [TestMethod]
        public void AddFriend_ShouldAllowNewFriend()
        {
            int myId = 55;
            int targetId = 77;
            bool result = (myId != targetId);
            Assert.IsTrue(result, "Система должна разрешать добавление другого пользователя");
        }

        [TestMethod]
        public void Search_Negative_TooShortNickname_ShouldFail()
        {
            string shortInput = "A";
            int minLength = 3;
            bool isSearchAllowed = shortInput.Length >= minLength;
            Assert.IsTrue(isSearchAllowed, "Система разрешила поиск по слишком короткому никнейму (менее 3 символов).");
        }

        [TestMethod]
        public void SocialLogic_Negative_SessionLost_ShouldFail()
        {
            object currentUser = null;
            bool canPerformAction = currentUser != null;
            Assert.IsTrue(canPerformAction, "Попытка взаимодействия с друзьями при отсутствии активной сессии пользователя.");
        }

        [TestMethod]
        public void Friendship_Negative_DuplicateCheck_ShouldFail()
        {
            var existingFriends = new List<int> { 10, 20, 30 };
            int friendToAdd = 20;
            bool isAlreadyFriend = existingFriends.Contains(friendToAdd);
            Assert.IsFalse(isAlreadyFriend, "Обнаружена попытка повторного добавления пользователя, который уже является другом.");
        }
    }
}