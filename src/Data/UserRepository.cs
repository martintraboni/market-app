using Minimarket.Exceptions;
using Minimarket.DTOs;
using Models;
using Microsoft.EntityFrameworkCore;

namespace Minimarket.Data
{
    public class UserRepository
    {
        public User? GetUserById(int id)
        {
            using var db = new MinimarketContext();
            return db.Usuarios.Include(u => u.Role).FirstOrDefault(u => u.Id == id);
        }

        public static List<UserListDto> GetAllDto()
        {
            using var db = new MinimarketContext();
            return db.Usuarios
                .Include(u => u.Role)
                .OrderBy(u => u.Username)
                .Select(u => new UserListDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    FullName = u.FullName,
                    Role = u.Role.RoleDescription,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt
                })
                .ToList();
        }

        public User Login(string username, string password)
        {
            using var db = new MinimarketContext();
            var user = db.Usuarios.Include(u => u.Role).FirstOrDefault(u => u.Username == username);

            if (user == null)
                throw new UsuarioNoExisteException();

            if (user.Password != password)
                throw new ContrasenaIncorrectaException();

            if (!user.IsActive)
                throw new Exception("El usuario está inactivo.");

            return user;
        }

        public static void CreateUser(string username, string fullName, string password, int roleId, int currentUserId)
        {
            using var db = new MinimarketContext();
            var user = new User
            {
                Username = username,
                FullName = fullName,
                Password = password,
                RoleId = roleId,
                IsActive = true,
                CreatedAt = DateTime.Now
            };
            db.Usuarios.Add(user);
            db.SaveChanges();

            // Registrar en auditoría
            var audit = new AuditLog
            {
                UserId = currentUserId,
                DateTime = DateTime.Now,
                Event = Constants.AuditEventCreateUser,
                Details = $"Usuario creado: {username} - {fullName}"
            };
            db.Auditoria.Add(audit);
            db.SaveChanges();
        }

        public static void UpdateUser(int userId, string username, string fullName, string password, int roleId, bool isActive, int currentUserId)
        {
            using var db = new MinimarketContext();
            var user = db.Usuarios.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.Username = username;
                user.FullName = fullName;
                if (!string.IsNullOrEmpty(password))
                    user.Password = password;
                user.RoleId = roleId;
                user.IsActive = isActive;
                db.SaveChanges();

                // Registrar en auditoría
                var audit = new AuditLog
                {
                    UserId = currentUserId,
                    DateTime = DateTime.Now,
                    Event = Constants.AuditEventUpdateUser,
                    Details = $"Usuario actualizado: {username} - {fullName}"
                };
                db.Auditoria.Add(audit);
                db.SaveChanges();
            }
        }

        public static void DeleteUser(int userId, int currentUserId)
        {
            using var db = new MinimarketContext();
            var user = db.Usuarios.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                var username = user.Username;
                db.Usuarios.Remove(user);
                db.SaveChanges();

                // Registrar en auditoría
                var audit = new AuditLog
                {
                    UserId = currentUserId,
                    DateTime = DateTime.Now,
                    Event = Constants.AuditEventDeleteUser,
                    Details = $"Usuario eliminado: {username}"
                };
                db.Auditoria.Add(audit);
                db.SaveChanges();
            }
        }

        public static void ToggleActive(int userId, int currentUserId)
        {
            using var db = new MinimarketContext();
            var user = db.Usuarios.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.IsActive = !user.IsActive;
                var estado = user.IsActive ? "Habilitado" : "Inhabilitado";
                db.SaveChanges();

                // Registrar en auditoría
                var audit = new AuditLog
                {
                    UserId = currentUserId,
                    DateTime = DateTime.Now,
                    Event = Constants.AuditEventToggleUserStatus,
                    Details = $"Usuario {user.Username}: {estado}"
                };
                db.Auditoria.Add(audit);
                db.SaveChanges();
            }
        }
    }
}
