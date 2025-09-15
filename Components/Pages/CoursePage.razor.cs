using CourseRegisterApp.CourseData;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;

namespace CourseRegisterApp.Components.Pages
{
    public partial class CoursePage
    {
        [Parameter]
        public int Id { get; set; }

        private Course? CourseDetails { get; set; }
        private List<CourseUser> Students { get; set; } = new List<CourseUser>();
        private int _enrolledStudentsCount;
        private int _freePlaces;
        private bool _isStudent;
        private bool _isAdmin;
        private bool _isLecturer;
        private bool _isEnrolled;
        private IList<string> UserRoles { get; set; } = new List<string>();
        private int? _currentUserId;

        // Dialog state
        private bool isRemoveDialogOpen = false;
        private CourseUser? _userToRemove;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                // Get the current user's authentication state
                var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;
                var appUser = await UserManager.GetUserAsync(user);
                if (appUser != null)
                {
                    // Check user roles
                    UserRoles = await UserManager.GetRolesAsync(appUser);
                    _isStudent = UserRoles.Contains("Student");
                    _isAdmin = UserRoles.Contains("Admin");
                    _isLecturer = UserRoles.Contains("Lecturer");
                    // Get the user's ID from the User entity
                    var userData = await DbContext.Users.FirstOrDefaultAsync(u => u.Email == appUser.Email);
                    if (userData != null)
                    {

                        _currentUserId = userData.UserId;
                    }

                    // Fetch the course details, including the lecturer's email
                    CourseDetails = await DbContext.Courses
                        .Include(c => c.LecturerNavigation)
                        .FirstOrDefaultAsync(c => c.CourseId == Id);

                    if (CourseDetails != null)
                    {
                        await LoadStudentsAsync();

                        // Check if the current user is enrolled
                        _isEnrolled = Students.Any(s => s.UserId == _currentUserId);
                    }
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error loading course details: {ex.Message}", Severity.Error);
                CourseDetails = null;
            }
        }

        private async Task LoadStudentsAsync()
        {
            // Fetch the list of students for this course
            Students = await DbContext.CourseUsers
                .Include(cu => cu.User)
                .Where(cu => cu.CourseId == Id)
                .ToListAsync();

            // Calculate enrollment statistics
            _enrolledStudentsCount = Students.Count;
            _freePlaces = CourseDetails.MaxStudents - _enrolledStudentsCount;
        }

        private void OpenRemoveDialog(CourseUser user)
        {
            _userToRemove = user;
            isRemoveDialogOpen = true;
        }

        private void OpenLeaveCourseDialog()
        {
            // For a student leaving the course, the userToRemove is the current user
            _userToRemove = Students.FirstOrDefault(s => s.UserId == _currentUserId);
            isRemoveDialogOpen = true;
        }

        private void CloseDialog()
        {
            isRemoveDialogOpen = false;
            _userToRemove = null; // Clear the user to remove
        }

        private async Task ConfirmRemovalAsync()
        {
            if (_userToRemove == null)
            {
                CloseDialog();
                return;
            }
            if (CourseDetails.CourseEnd < DateOnly.FromDateTime(DateTime.Today))
            {
                Snackbar.Add("Cannot remove student from a course that has already ended.", Severity.Warning);
                CloseDialog();
                return;
            }
            if (CourseDetails.CourseStart <= DateOnly.FromDateTime(DateTime.Today))
            {
                Snackbar.Add("Cannot remove student from a course that has already started.", Severity.Warning);
                CloseDialog();
                return;
            }
            try
            {
                
                // Find the CourseUser entry and remove it
                DbContext.CourseUsers.Remove(_userToRemove);
                await DbContext.SaveChangesAsync();

                // Provide feedback
                Snackbar.Add($"{_userToRemove.User?.Email} has been successfully removed from the course.", Severity.Success);

                // Close the dialog and refresh the data
                CloseDialog();
                await LoadStudentsAsync();
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error removing student: {ex.Message}", Severity.Error);
            }
        }
    }
}
