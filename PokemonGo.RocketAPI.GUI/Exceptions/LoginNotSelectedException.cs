using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.RocketAPI.GUI.Exceptions
{
    public class LoginNotSelectedException : Exception
    {
        public LoginNotSelectedException(string message) : base(message)
        {

        }
    }
}
