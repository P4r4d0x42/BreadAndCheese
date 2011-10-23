using System;

namespace BreadAndCheese
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (BreadAndCheeseGame game = new BreadAndCheeseGame())
            {
                game.Run();
            }
        }
    }
#endif
}

