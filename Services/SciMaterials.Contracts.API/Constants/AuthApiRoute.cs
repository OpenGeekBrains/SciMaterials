namespace SciMaterials.Contracts.API.Constants;

/// <summary>
/// Маршруты AuthApi
/// </summary>
public static class AuthApiRoute
{
    /// <summary>
    /// Uri адрес сервиса
    /// </summary>
    public const string AuthApiUri = "http://localhost:5185/";
    
    /// <summary>
    /// Контроллер аутентификации
    /// </summary>
    public const string AuthControllerName = "auth/";

    /// <summary>
    /// Регистрация пользователя
    /// </summary>
    public const string Register = "register";
    
    /// <summary>
    /// Авторизация пользователя
    /// </summary>
    public const string Login = "login";
    
    /// <summary>
    /// Выход из системы
    /// </summary>
    public const string Logout = "logout";
    
    /// <summary>
    /// Смена пароля пользователя
    /// </summary>
    public const string ChangePassword = "change_password";

    /// <summary>
    /// Обновление токена пользователя
    /// </summary>
    public const string RefreshToken = "refresh_token";
    
    /// <summary>
    /// Подтверждение почты при регистрации
    /// </summary>
    public const string ConfirmEmail = "confirm_email";
    
    /// <summary>
    /// Создание роли пользователя
    /// </summary>
    public const string CreateRole = "create_role";
    
    /// <summary>
    /// Получение всех ролей в системе
    /// </summary>
    public const string GetAllRoles = "get_all_roles";
    
    /// <summary>
    /// Получение роли по идентификатору
    /// </summary>
    public const string GetRoleById = "get_role_by_id/";

    /// <summary>
    /// Изменение названия роли по идентификатору роли
    /// </summary>
    public const string EditRoleNameById = "edit_role_name_by_id";
    
    /// <summary>
    /// Удаление роли по идентификатору
    /// </summary>
    public const string DeleteRoleById = "delete_role_by_id/";
    
    /// <summary>
    /// Добавление роли к пользователю
    /// </summary>
    public const string AddRoleToUser = "add_role_to_user";
    
    /// <summary>
    /// Удаление роли у пользователя
    /// </summary>
    public const string DeleteUserRoleByEmail = "delete_user_role_by_email/";
    
    /// <summary>
    /// Получение списка всех ролей у конкретного пользователя
    /// </summary>
    public const string GetAllUserRolesByEmail = "get_all_user_roles_by_email/";
    
    /// <summary>
    /// Создание пользователя админом
    /// </summary>
    public const string CreateUser = "create_user";
    
    /// <summary>
    /// Получение инф. о пользовател по email
    /// </summary>
    public const string GetUserByEmail = "get_user_by_email/";
    
    /// <summary>
    /// Получение инф. о всех пользователях в системе
    /// </summary>
    public const string GetAllUsers = "get_all_users";
    
    /// <summary>
    /// Редактирование инф. о пользователе по его email
    /// </summary>
    public const string EditUserByEmail = "edit_user_by_email";
    
    /// <summary>
    /// Удаление пользователя по email
    /// </summary>
    public const string DeleteUserByEmail = "delete_user_by_email/";
    
    /// <summary>
    /// Удаление всех не подтвердивших свою почту пользователей в системе (для чистки БД админом)
    /// </summary>
    public const string DeleteUserWithoutConfirm = "delete_user_without_confirm";
}