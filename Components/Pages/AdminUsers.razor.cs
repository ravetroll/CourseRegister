using CourseRegisterApp.Data;
using Microsoft.AspNetCore.Identity;

namespace CourseRegisterApp.Components.Pages
{
    public partial class AdminUsers
    {
        private IEnumerable<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        private Dictionary<string, IList<string>> UserRoles { get; set; } = new();
        private List<IdentityRole> AllRoles { get; set; } = new List<IdentityRole>();
        private ApplicationUser? currentUser;
        private bool isDialogOpen = false;
        private List<string> selectedRoles = new();
        private string searchString = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            Users = UserManager.Users.ToList();
            AllRoles = RoleManager.Roles.ToList();

            UserRoles.Clear();
            foreach (var user in Users)
            {
                var roles = await UserManager.GetRolesAsync(user);
                UserRoles.Add(user.Id, roles);
            }
        }

        private void EditUserRoles(ApplicationUser user)
        {
            currentUser = user;
            // Load current roles for the user and prepare for the dialog
            selectedRoles = UserRoles.TryGetValue(user.Id, out var roles) ? roles.ToList() : new List<string>();
            isDialogOpen = true;
        }

        private void CloseDialog()
        {
            isDialogOpen = false;
        }

        private void ToggleRole(string roleName, bool isChecked)
        {
            if (isChecked && !selectedRoles.Contains(roleName))
            {
                selectedRoles.Add(roleName);
            }
            else if (!isChecked && selectedRoles.Contains(roleName))
            {
                selectedRoles.Remove(roleName);
            }
        }

        private async Task SaveRolesAsync()
        {
            if (currentUser == null) return;

            var currentRoles = await UserManager.GetRolesAsync(currentUser);

            // Add roles that were selected but not currently assigned
            var rolesToAdd = selectedRoles.Except(currentRoles);
            foreach (var role in rolesToAdd)
            {
                await UserManager.AddToRoleAsync(currentUser, role);
            }

            // Remove roles that are currently assigned but not selected
            var rolesToRemove = currentRoles.Except(selectedRoles);
            foreach (var role in rolesToRemove)
            {
                await UserManager.RemoveFromRoleAsync(currentUser, role);
            }

            isDialogOpen = false;
            await LoadDataAsync(); // Refresh the table to show the changes
        }
    }
}
