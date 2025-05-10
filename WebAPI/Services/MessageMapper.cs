using System.Collections.Generic;

namespace WebAPI.Services
{
    public class MessageMapper
    {
        public Dictionary<string, Dictionary<string, string>> ErrorMessages { get; }
        public Dictionary<string, string> SuccessMessages { get; }

        public MessageMapper()
        {
            ErrorMessages = new Dictionary<string, Dictionary<string, string>>
            {
                ["Generic"] = new Dictionary<string, string>
                {
                    ["GenericError"] = "An unexpected error occurred."
                },
                ["EntityBase"] = new Dictionary<string, string>
                {
                    ["InvalidID"] = "Invalid ID.",
                    ["NotFound"] = "Record not found.",
                    ["DuplicateEntry"] = "Duplicate record detected.",
                    ["ProtectedDelete"] = "Record cannot be deleted as it is being referenced.",
                    ["NullEntity"] = "Null entity object.",
                    ["AlreadyActive"] = "Record is already active."
                },
                ["Operations"] = new Dictionary<string, string>
                {
                    ["SaveFailed"] = "Error saving record.",
                    ["UpdateFailed"] = "Error updating record.",
                    ["DeleteFailed"] = "Error deleting record.",
                    ["RestoreFailed"] = "Error restoring record.",
                    ["DbException"] = "Database exception occurred."
                },
                ["User"] = new Dictionary<string, string>
                {
                    ["InvalidRoleID"] = "Invalid user role ID.",
                    ["MissingEmail"] = "Email field is required.",
                    ["EmailInUse"] = "Email is already registered.",
                    ["MissingRole"] = "User role is required.",
                    ["LoginFailed"] = "Login failed.",
                    ["InvalidCredentials"] = "Invalid email/password."
                },
                ["Client"] = new Dictionary<string, string>
                {
                    ["MissingName"] = "Full name field is required.",
                    ["InvalidDocFormat"] = "Invalid document format.",
                    ["InvalidEmail"] = "Invalid email format.",
                    ["InvalidPhone"] = "Invalid phone format.",
                    ["MissingPassword"] = "Password field is required."
                },
                ["Auth"] = new Dictionary<string, string>
                {
                    ["InvalidCredentials"] = "Invalid credentials.",
                    ["SessionExpired"] = "Session expired.",
                    ["InsufficientPermissions"] = "Insufficient permissions.",
                    ["TokenInvalid"] = "Invalid token.",
                    ["TokenExpired"] = "Token has expired."
                }
            };

            SuccessMessages = new Dictionary<string, string>
            {
                ["GenericSuccess"] = "Operation completed successfully.",
                ["SaveSuccess"] = "Record saved successfully.",
                ["UpdateSuccess"] = "Record updated successfully.",
                ["DeleteSuccess"] = "Record deleted successfully.",
                ["RestoreSuccess"] = "Record restored successfully.",
                ["LoginSuccess"] = "Login successful.",
                ["LogoutSuccess"] = "Logout successful.",
                ["PasswordChanged"] = "Password updated successfully.",
                ["TokenRefreshed"] = "Token refreshed successfully."
            };
        }
    }
}