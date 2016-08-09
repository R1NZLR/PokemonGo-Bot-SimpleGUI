using System;

namespace PokemonGo.RocketAPI.GUI.Exceptions
{
    public class LoginNotSelectedException : Exception
    {
        public LoginNotSelectedException(string message) : base(message)
        {

        }
    }
}
