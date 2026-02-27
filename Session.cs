using System;

namespace SteamLamp
{
    public static class Session
    {
        public static User CurrentUser { get; set; }
        public static string LoginSource { get; set; }
    }
}