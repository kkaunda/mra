using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MRAtaxpayerWS
{
    class LoginFailClass
    {
        // login fail message 
        // ================================================================================================================================

        //{"ResultCode":1,"Remark":"Wrong Username or Password","Token":null,"Authenticated":false,"UserDetails":null}

        // login fail classes 
        // ================================================================================================================================
        public class Root
        {
            public int ResultCode { get; set; }
            public string Remark { get; set; }
            public object Token { get; set; }
            public bool Authenticated { get; set; }
            public object UserDetails { get; set; }
        }
        //---------------------------------------------------------------------------------------------------------------------------------

        protected List<Root> list = new List<Root>();


    }
}
