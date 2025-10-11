using Minimarket.Exceptions;
using Minimarket.Models;

namespace Minimarket.Data
{
    public static class UserRepository
    {
        public static User Login(string username, string password)
        {
            using var db = new MinimarketContext();
            var user = db.Usuarios.FirstOrDefault(u => u.Username == username);

            if (user == null)
                throw new UsuarioNoExisteException();

            if (user.Password != password)
                throw new ContrasenaIncorrectaException();

            return user;
        }

        public static void Create(string username, string password, string role)
        {
            using var db = new MinimarketContext();
            var user = new User
            {
                Username = username,
                Password = password,
                Rol = role,
                Nombre = "" // Puedes ajustar esto si tienes el nombre disponible
            };
            db.Usuarios.Add(user);
            db.SaveChanges();
        }

        public static void Edit(User updatedUser)
        {
            using var db = new MinimarketContext();
            var user = db.Usuarios.FirstOrDefault(u => u.Id == updatedUser.Id);
            if (user != null)
            {
                user.Username = updatedUser.Username;
                user.Password = updatedUser.Password;
                user.Nombre = updatedUser.Nombre;
                user.Rol = updatedUser.Rol;
                db.SaveChanges();
            }
        }

        public static void Delete(int idUsuario)
        {
            using var db = new MinimarketContext();
            var user = db.Usuarios.FirstOrDefault(u => u.Id == idUsuario);
            if (user != null)
            {
                db.Usuarios.Remove(user);
                db.SaveChanges();
            }
        }
    }
}
