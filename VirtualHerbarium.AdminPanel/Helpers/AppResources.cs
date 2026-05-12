using System.Windows;

public static class AppResources
{
    public static string Get(string key) =>
        (string)Application.Current.Resources[key];

    public static string Login_Error_InvalidCredentials => Get("Login_Error_InvalidCredentials");
    public static string Login_Error_NoAdminRights => Get("Login_Error_NoAdminRights");

    public static string Placeholder_SelectOption => Get("Placeholder_SelectOption");

    public static string Error_FetchCollections => Get("Error_FetchCollections");
    public static string Error_DeleteCollection => Get("Error_DeleteCollection");
    public static string Confirm_DeleteCollection => Get("Confirm_DeleteCollection");

    public static string Error_FetchPlants => Get("Error_FetchPlants");
    public static string Error_DeletePlant => Get("Error_DeletePlant");
    public static string Confirm_DeletePlant => Get("Confirm_DeletePlant");

    public static string Users_Success_Banned => Get("Users_Success_Banned");
    public static string Users_Success_Unbanned => Get("Users_Success_Unbanned");
    public static string Users_Success_MadeAdmin => Get("Users_Success_MadeAdmin");
    public static string Users_Success_RemovedAdmin => Get("Users_Success_RemovedAdmin");
    public static string Users_Success_WarningMock => Get("Users_Success_WarningMock");
    public static string Users_Success_WarningSent => Get("Users_Success_WarningSent");

    public static string Error_SessionExpired => Get("Error_SessionExpired");
    public static string Error_NoPermission => Get("Error_NoPermission");
    public static string Error_UserNotFound => Get("Error_UserNotFound");
    public static string Error_Server => Get("Error_Server");
    public static string Error_Generic => Get("Error_Generic");

    public static string Notifications_Error_EmptyFields => Get("Notifications_Error_EmptyFields");
    public static string Notifications_Error_SendFailed => Get("Notifications_Error_SendFailed");
    public static string Notifications_Success_Sent => Get("Notifications_Success_Sent");

    public static string Error_Title => Get("Error_Title");
    public static string Success_Title => Get("Success_Title");
    public static string Confirm_Title => Get("Confirm_Title");
}
