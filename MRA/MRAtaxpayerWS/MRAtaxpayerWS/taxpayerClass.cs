using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MRAtaxpayerWS
{
    // class for taxpayer data - individual
    class taxpayerClass
    {
    
        //Fields - model the taxpayer data
        private string _c_tpin;
        private string _c_businesscertificatenumber;
        private string _c_tradingname;
        private string _c_businessregistrationdate;
        private string _c_mobilenumber;
        private string _c_email;
        private string _c_physicallocation;
        private string _c_username;
        private Boolean _c_deleted;
        private int _c_id;

        //Properties - encapsulate - to declare properties from the data
        public string c_tpin {get { return _c_tpin; } set { _c_tpin = value; } }
        public string c_businesscertificatenumber { get { return _c_businesscertificatenumber; } set { _c_businesscertificatenumber = value; } }
        public string c_tradingname { get { return _c_tradingname; } set { _c_tradingname = value; } }
        public string c_businessregistrationdate { get { return _c_businessregistrationdate; } set { _c_businessregistrationdate = value; } }
        public string c_mobilenumber { get { return _c_mobilenumber; } set { _c_mobilenumber = value; } }
        public string c_email { get { return _c_email; } set { _c_email = value; } }
        public string c_physicallocation { get { return _c_physicallocation; } set { _c_physicallocation = value; } }
        public string c_username { get { return _c_username; } set { _c_username = value; } }
        public Boolean c_deleted { get { return _c_deleted; } set { _c_deleted = value; } }
        public int c_id { get { return _c_id; } set { _c_id = value; } }
       
    }
}
