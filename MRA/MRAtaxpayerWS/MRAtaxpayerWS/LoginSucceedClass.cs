using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MRAtaxpayerWS
{
    class LoginSucceedClass
    {
        // Class definitions - out side Form1 Class!
        // --------------------------------------------------------------------------------------------------------------------------------
        // login pass message  
        // ================================================================================================================================
        //{
        //    "ResultCode": 1,
        //    "Remark": "Successful",
        //    "Token": {
        //        "Name": "Kajanikaunda@gmail.com",
        //        "Value": "161fd72e-9bfe-4924-a22a-322f66bd5b9d"
        //    },
        //    "Authenticated": true,
        //    "UserDetails": {
        //        "Username": "Kajanikaunda@gmail.com",
        //        "Password": "password000122",
        //        "FirstName": "Kajani",
        //        "LastName": "Kaunda",
        //        "email": "Kajanikaunda@gmail.com"
        //    }
        //}
        // login pass classes
        // ================================================================================================================================
        public class Token
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public class UserDetails
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string email { get; set; }
        }

        public class Root
        {
            public int ResultCode { get; set; }
            public string Remark { get; set; }
            public Token Token { get; set; }
            public bool Authenticated { get; set; }
            public UserDetails UserDetails { get; set; }
        }
        //---------------------------------------------------------------------------------------------------------------------------------


    }
}
