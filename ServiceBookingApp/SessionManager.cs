using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ServiceBookingApp
{
    public static class SessionManager
    {
        public static Business CurrentBusiness { get; private set; }
        public static Customer CurrentCustomer { get; private set; }
        public static bool IsLoggedIn => CurrentBusiness != null || CurrentCustomer != null;

        // Path to store local session file in the user's AppData folder
        private static readonly string SessionFile = "../../ServiceBooking_Session.txt";


        public static void Login(Business business)
        {
            // Clear local storage
            if (File.Exists(SessionFile))
            {
                File.Delete(SessionFile);
            }

            CurrentBusiness = business;
            CurrentCustomer = null;
            // Save to local storage
            File.WriteAllText(SessionFile, $"Business:{business.BusinessId}");
        }

        public static void Login(Customer customer)
        {
            CurrentCustomer = customer;
            CurrentBusiness = null;
            // Save to local storage
            File.WriteAllText(SessionFile, $"Customer:{customer.CustomerId}");
        }

        public static void Logout()
        {
            CurrentBusiness = null;
            CurrentCustomer = null;

            if (File.Exists(SessionFile))
            {
                File.Delete(SessionFile);
            }
        }

        // Call this when the app starts to automatically log the user back in
        public static void LoadSession()
        {
            if (!File.Exists(SessionFile)) return;

            try
            {
                string[] sessionData = File.ReadAllText(SessionFile).Split(':');
                if (sessionData.Length == 2)
                {
                    string role = sessionData[0];
                    int id = int.Parse(sessionData[1]);

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
                Logout();
            }
        }
    }
}