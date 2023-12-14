
namespace MauiApp4
{
    using MauiApp4.Models;
    using Microsoft.Extensions.Configuration;
    using SQLite;

    public partial class MainPage : ContentPage
    {
        private const string DatabaseFilename = "UserData.db3";
        private SQLiteAsyncConnection _database;

        private const string RememberMeKey = "RememberMe";
        private const string UsernameKey = "Username";
        private const string PasswordKey = "Password";

        public MainPage()
        {
            InitializeComponent();
            _ = InitializeDatabaseAsync();
            // Load saved data if available
            if (Preferences.Get(RememberMeKey, false))
            {
                UsernameEntry.Text = Preferences.Get(UsernameKey, string.Empty);
                PasswordEntry.Text = Preferences.Get(PasswordKey, string.Empty);
                RememberMeCheckbox.IsChecked = true;
            }
        }
        private async Task InitializeDatabaseAsync()
        {
            string databasePath = Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
            _database = new SQLiteAsyncConnection(databasePath);
            await _database.CreateTableAsync<UserData>();

            // Insert an example user for testing
            var user1 = new UserData
            {
                Username = "testuser",
                Password = "testpassword"
            };
            await _database.InsertAsync(user1);
            var user2 = new UserData
            {
                Username = "user",
                Password = "password"
            };
            await _database.InsertAsync(user2);
        }

        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            string username = UsernameEntry.Text;
            string password = PasswordEntry.Text;
            bool rememberMe = RememberMeCheckbox.IsChecked;

            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                // Authenticate user (replace with your authentication logic)
                bool isAuthenticated = await AuthenticateUserAsync(username, password);

                if (isAuthenticated)
                {
                    if (rememberMe)
                    {
                        // Save authentication data to preferences
                        Preferences.Set(UsernameKey, username);
                        Preferences.Set(PasswordKey, password);
                        Preferences.Set(RememberMeKey, true);
                    }
                    else
                    {
                        // If not remembering data, remove from preferences
                        Preferences.Remove(UsernameKey);
                        Preferences.Remove(PasswordKey);
                        Preferences.Set(RememberMeKey, false);
                    }
                    await Navigation.PushAsync(new SuccessPage());
                }
                else
                {
                    await DisplayAlert("Authentication Failed", "Invalid username or password.", "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", "Please enter both username and password.", "OK");
            }
        }

        private async Task<bool> AuthenticateUserAsync(string username, string password)
        {
            // Query the database for the user's credentials and perform authentication
            UserData user = await _database.Table<UserData>()
                                            .Where(u => u.Username == username && u.Password == password)
                                            .FirstOrDefaultAsync();

            return user != null; // Return true if user is found and authenticated
        }

    }
}
