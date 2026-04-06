using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ServiceBookingApp
{
    /// <summary>
    /// SessionManager is a static class responsible
    /// for managing the current logged in user (either a Business or a Customer) and 
    /// persisting the session across application restarts using a local text file.
    /// </summary>
    public static class SessionManager
    {
        // Static properties to hold the current logged in business or customer
        public static Business CurrentBusiness { get; private set; }
        public static Customer CurrentCustomer { get; private set; }
        public static bool IsLoggedIn => CurrentBusiness != null || CurrentCustomer != null;
        // Using a text file for session persistence.
        private static readonly string SessionFile = "../../User_Session.txt";
        public static void LogIn(Business business)
        {
            // Clear local storage before setting new session
            if (File.Exists(SessionFile))
            {
                File.Delete(SessionFile);
            }
            // Set the current business and clear any customer session
            CurrentBusiness = business;
            CurrentCustomer = null;
            // Save to local storage
            File.WriteAllText(SessionFile, $"Business:{business.BusinessId}");
        } // Log in a business and save the session to local storage

        public static void LogIn(Customer customer)
        {
            // Clear local storage before setting new session
            if (File.Exists(SessionFile))
            {
                File.Delete(SessionFile);
            }
            CurrentCustomer = customer;
            CurrentBusiness = null;
            // Save to local storage
            File.WriteAllText(SessionFile, $"Customer:{customer.CustomerId}");
        } // Log in a customer and save the session to local storage

        public static void LogOut()
        {
            CurrentBusiness = null;
            CurrentCustomer = null;

            if (File.Exists(SessionFile))
            {
                File.Delete(SessionFile);
            }
        } // Log out the user and clear the session from local storage

        public static void LoadSession()
        {
            if (!File.Exists(SessionFile)) return;

            if (IsLoggedIn) return; // Already logged in, no need to load

            try
            {
                string[] sessionData = File.ReadAllText(SessionFile).Split(':');
                if (sessionData.Length == 2)
                {
                    // sessionData[0] is the role (Business or Customer), sessionData[1] is the ID
                    string role = sessionData[0];
                    int id = int.Parse(sessionData[1]);
                    // Load the user from the database based on the role and ID from usersesssion
                    using (var db = new ServiceBookingContext())
                    {
                        if (role == "Business")
                        {
                            CurrentBusiness = db.Businesses.FirstOrDefault(b => b.BusinessId == id);
                        }
                        else if (role == "Customer")
                        {
                            CurrentCustomer = db.Customers.FirstOrDefault(c => c.CustomerId == id);
                        }
                    }
                }
            }
            catch
            {
                // If the file is corrupted or DB fails. clear it and force a new login
                LogOut();
            }
        }  // Load the session from local storage if it exists and set the current user accordingly
    }
}