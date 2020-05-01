using System;
using System.Collections.Generic;
using System.Text;

namespace CodeChatSDK
{
    class DatabaseManager
    {
        private static DatabaseHelper databaseHelper;

        public static DatabaseHelper GetInstance()
        {
            if (databaseHelper == null)
            {
                databaseHelper = new DatabaseHelper();
            }
            return databaseHelper;
        }
    }
}
