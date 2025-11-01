using Minimarket.Exceptions;
using Models;

namespace Minimarket.Data
{
    public class UserRepository
    {
        public User? GetUserById(int id)
        {
            using var db = new MinimarketContext();
            return db.Usuarios.FirstOrDefault(u => u.Id == id);
        }

        public User Login(string username, string password)
        {
            using var db = new MinimarketContext();
            var user = db.Usuarios.FirstOrDefault(u => u.Username == username);

            if (user == null)
                throw new UsuarioNoExisteException();

            if (user.Password != password)
                throw new ContrasenaIncorrectaException();

            return user;
        }

        public void Create(string username, string password, string role)
        {
            using var db = new MinimarketContext();
            var user = new User
            {
                Username = username,
                Password = password,
                Role = role,
                FullName = ""
            };
            db.Usuarios.Add(user);
            db.SaveChanges();
        }

        public void Edit(User updatedUser)
        {
            using var db = new MinimarketContext();
            var user = db.Usuarios.FirstOrDefault(u => u.Id == updatedUser.Id);
            if (user != null)
            {
                user.Username = updatedUser.Username;
                user.Password = updatedUser.Password;
                user.FullName = updatedUser.FullName;
                user.Role = updatedUser.Role;
                db.SaveChanges();
            }
        }

        public void Delete(int idUsuario)
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
