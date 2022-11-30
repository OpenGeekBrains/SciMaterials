using SciMaterials.Contracts.Result;

// ReSharper disable once CheckNamespace
namespace SciMaterials.Contracts;

public static partial class Errors
{
    public static class Identity
    {
        public static class Register
        {
            public static readonly Error Unhandled = new("IDEN000", "Пользователя не удалось зарегистрировать");
            public static readonly Error Fail = new("IDEN001", "Не удалось зарегистрировать пользователя");
        }

        public static class Login
        {
            public static readonly Error Fail = new("IDEN002", "Не удалось авторизовать пользователя");
            public static readonly Error Unhandled = new("IDEN003", "Не удалось авторизовать пользователя");
            public static readonly Error UserNotFound = new("IDEN004", "Некорректно введены данные");
            public static readonly Error EmailNotConfirmed = new("IDEN005", "Email не подтверждён");
        }

        public static class Logout
        {
            public static readonly Error Unhandled = new("IDEN006", "Не удалось выйти из системы");
        }

        public static class ChangePassword
        {
            public static readonly Error Unhandled = new("IDEN007", "Произошла ошибка при смене пароля");
            public static readonly Error EmailNotConfirmed = new("IDEN008", "Email не подтверждён");
            public static readonly Error NotFound = new("IDEN009", "Некорректно введены данные");
            public static readonly Error MissAuthorizationData = new("IDEN010", "Change password request called without authorization data");
            public static readonly Error Fail = new("IDEN011", "Не удалось изменить пароль");
        }

        public static class GetRefreshToken
        {
            public static readonly Error Unhandled = new("IDEN012", "Не удалось обновить токен пользователя");
            public static readonly Error Fail = new("IDEN013", "Не удалось обновить токен пользователя");
        }

        public static class CreateRole
        {
            public static readonly Error Unhandled = new("IDEN014", "Произошла ошибка при создании роли");
            public static readonly Error Fail = new("IDEN015", "Не удалось создать роль");
        }

        public static class GetAllRoles
        {
            public static readonly Error Unhandled = new("IDEN016", "Произошла ошибка при создании роли");
        }

        public static class GetRoleById
        {
            public static readonly Error Unhandled = new("IDEN017", "Произошла ошибка при запросе роли");
            public static readonly Error NotFound = new("IDEN018", "Не удалось получить роль");
        }

        public static class EditRoleNameById
        {
            public static readonly Error Unhandled = new("IDEN019", "Произошла ошибка при редактировании роли");
            public static readonly Error NotFound = new("IDEN020", "Не удалось найти роль");
            public static readonly Error Fail = new("IDEN021", "Не удалось изменить роль");
        }

        public static class DeleteRoleById
        {
            public static readonly Error Unhandled = new("IDEN022", "Произошла ошибка при редактировании роли");
            public static readonly Error NotFound = new("IDEN023", "Не удалось найти роль");
            public static readonly Error Fail = new("IDEN024", "Не удалось удалить роль");
            public static readonly Error TryToDeleteSystemRoles = new("IDEN025", "Произошла ошибка при удалении роли");
        }

        public static class AddRoleToUser
        {
            public static readonly Error Unhandled = new("IDEN022", "Произошла ошибка при присвоении роли пользователю");
            public static readonly Error RoleNotFound = new("IDEN023", "Роль не зарегистрированна");
            public static readonly Error UserNotFound = new("IDEN023", "Пользователь не найден");
            public static readonly Error Fail = new("IDEN024", "Произошла ошибка при присвоении роли пользователю");
            public static readonly Error UserAlreadyInRole = new("IDEN025", "Пользователь уже имеет данную роль");
        }

        public static class RemoveRoleFromUserByEmail
        {
            public static readonly Error Unhandled = new("IDEN026", "Произошла ошибка при присвоении роли пользователю");
            public static readonly Error RoleNotFound = new("IDEN027", "Роль не зарегистрированна");
            public static readonly Error UserNotFound = new("IDEN028", "Пользователь не найден");
            public static readonly Error Fail = new("IDEN029", "Произошла ошибка при присвоении роли пользователю");
            public static readonly Error UserNotInRole = new("IDEN030", "Пользователь не имеет данную роль");
            public static readonly Error TryToDownSuperAdmin = new("IDEN030", "Попытка понизить супер админа в должности");
        }

        public static class GetUserRoles
        {
            public static readonly Error Unhandled = new("IDEN031", "Произошла ошибка при получении списка ролей пользователя");
            public static readonly Error UserNotFound = new("IDEN032", "Пользователь не найден");
        }

        public static class GetUserByEmail
        {
            public static readonly Error Unhandled = new("IDEN033", "Произошла ошибка при получении списка ролей пользователя");
            public static readonly Error NotFound = new("IDEN034", "Пользователь не найден");
        }

        public static class GetAllUsers
        {
            public static readonly Error Unhandled = new("IDEN035", "Произошла ошибка при получении списка ролей пользователя");
        }

        public static class EditUserName
        {
            public static readonly Error Unhandled = new("IDEN036", "Произошла ошибка при получении списка ролей пользователя");
            public static readonly Error NotFound = new("IDEN037", "Пользователь не найден");
            public static readonly Error Fail = new("IDEN038", "Не удалось обновить имя пользователя");
        }

        public static class DeleteUser
        {
            public static readonly Error Unhandled = new("IDEN039", "Произошла ошибка при удалении пользователя");
            public static readonly Error NotFound = new("IDEN040", "Пользователь не найден");
            public static readonly Error Fail = new("IDEN041", "Не удалось удалить пользователя");
        }
    }
}
