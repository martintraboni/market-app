using System;

namespace Minimarket.Exceptions
{
    public class UsuarioNoExisteException : Exception
    {
        public UsuarioNoExisteException() : base("El usuario no existe.") { }
    }

    public class ContrasenaIncorrectaException : Exception
    {
        public ContrasenaIncorrectaException() : base("La contraseña es incorrecta.") { }
    }
}
