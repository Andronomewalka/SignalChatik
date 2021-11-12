using System;
using System.Text;

namespace SignalChatik.Helpers
{
    public class Salt
    {
        public static string CreateSalt()
        {
            Random rand = new Random();
            StringBuilder saltBuilder = new StringBuilder();
            saltBuilder.Length = rand.Next(10, 25);
            for (int i = 0; i < saltBuilder.Length; i++)
                saltBuilder[i] = (char)rand.Next(48, 126);
            return saltBuilder.ToString();
        }
    }
}
