using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
// more namespaces ...
using System.Net;
using System.Web;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.Serialization;
using System.Reflection;
using System.Runtime.Serialization.Json; 
using System.Xml.Serialization;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Specialized;

//add reference system.runtime.serialization|System.ServiceModel.Web.dll








namespace MRAtaxpayerWS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // start: public variables ...
        // assigned web service url ...
        public string pub_wsurl = "";

        // assign base web service url in case we need it ...
        public string pub_wsbaseurl = "https://www.mra.mw/sandbox/";

        // web service method to use
        public string pub_wsmethod = "";

        // web service task 
        public string pub_wstask = "";

        // web server user name and password
        public string MRAUsername = "";
        public string MRAPassword = "";

        // request string
        public string pub_wsRequeStstring = "";

        // set business registration date ... because we need to change its format for the web request
        // public Date  pub_BusinessRegistrationDate; 
        public string pub_year = "";
        public string pub_month = "";
        public string pub_day = "";
        public string pub_date = "";

        // login variable
        public Boolean pub_isloggedin = false;
 



        // end: public variables ...


        // ;;;

        // test
        //Cookie-Aware WebClient
        public class CookieAwareWebClient : WebClient
        {
            //An aptly named container to store the Cookie
            public CookieContainer CookieContainer { get; private set; }

            public CookieAwareWebClient()
            {
                CookieContainer = new CookieContainer();
            }

            protected override WebRequest GetWebRequest(Uri address)
            {
                //Grabs the base request being made 
                var request = (HttpWebRequest)base.GetWebRequest(address);
                //Adds the existing cookie container to the Request
                request.CookieContainer = CookieContainer;

                return request;
            }



        }


        // create tax payer classes used to deserialize the json strings into objects 
        // so that we can use and render them properly on the form


 





        // the process button to process the web service requests


        private void taxpeopleDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            //this.taxpeopleDataGridView
            //txtService.Text = dgvService.CurrentCell.Value.ToString();
            //label2.Text = dgvService.CurrentCell.Value.ToString();

            MessageBox.Show("Changed!");

        }

        // login
        // OK: login classes. Deserialse json data to these objects or just search the json string...
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
        
        private void buttonLogin_Click(object sender, EventArgs e)
        {

            string Credentials_data = "Email=" + this.textBoxMRAUsername.Text + "&" + "Password=" + this.textBoxMRAPassword.Text;
            using (WebClient client = new WebClient())
            {
                // set content type
                client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";

                // call API with url and user credentials
                string result = client.UploadString("https://www.mra.mw/sandbox/programming/challenge/webservice/auth/login", Credentials_data);

                // show/process/deserialize this json response

                this.textBox1.Text = result;
                this.textBox1.Refresh();

                if (result.Contains("Successful"))
                {
                 MessageBox.Show("Login Successful");
                 pub_isloggedin = true;
                 this.textBoxMRAUsername.Enabled = false;
                 this.textBoxMRAPassword.Enabled = false;
                } else 
                {
                    MessageBox.Show("Login not successful");
                    pub_isloggedin = false;
                }
                //
            }
        
        }

        // exit the app..OK
        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonLogout_Click(object sender, EventArgs e)
        {


            if (pub_isloggedin == false)
            {
                MessageBox.Show("Login First!");
                return;
            }
            string Credentials_data = "Email=" + this.textBoxMRAUsername.Text;
  
            using (WebClient client = new WebClient())
            {

              

                // set content type
                client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
               
                // call API with url and user credentials
                string result = client.UploadString("https://www.mra.mw/sandbox/programming/challenge/webservice/auth/logout", Credentials_data);
              
                // show/process json response
                //if successful - re-enable user text box ...

                this.textBox1.Text = result;
                this.textBox1.Refresh();

                if (result.Contains("Log out Successful"))
                {
                    MessageBox.Show("Logout Successful");
                    pub_isloggedin = false ;
                    this.textBoxMRAUsername.Enabled = true;
                    this.textBoxMRAPassword.Enabled = true;
                    this.textBoxMRAUsername.Clear();
                    this.textBoxMRAPassword.Clear();
                }
                else
                {
                    MessageBox.Show("Logout not successful");
                    pub_isloggedin = true;
                }

                //"Log out Successful"

                this.textBoxMRAUsername.Enabled = true;
            }
        }

        // register a tax payer
        private void buttonRegister_Click(object sender, EventArgs e)
        {
            if (pub_isloggedin == false)
            { MessageBox.Show("Login first."); return; }
            
            // set selected task
            pub_wstask = "REGISTER";

            using (WebClient client = new WebClient())
            {
                var reqparm = new System.Collections.Specialized.NameValueCollection();
                reqparm.Add("TPIN", "55545555");
                reqparm.Add("BusinessCertificateNumber", "HHHHHH");
                reqparm.Add("TradingName", "dddddd");
                reqparm.Add("BusinessRegistrationDate", "2021/11/06");
                reqparm.Add("MobileNumber", "232443433");
                reqparm.Add("Email", "kajanikaunda@gmail.com");
                reqparm.Add("PhysicalLocation", "zombas");
                reqparm.Add("Username", "kajanikaunda@gmail.com");

                reqparm.Add("TPIN", this.textBoxTPIN.Text );
                reqparm.Add("BusinessCertificateNumber", this.textBoxBUSINESSCERTIFICATENUMBER .Text );
                reqparm.Add("TradingName", this.textBoxTRADINGNAME .Text );
                reqparm.Add("BusinessRegistrationDate", "2021/11/06");
                reqparm.Add("MobileNumber", "232443433");
                reqparm.Add("Email", "kajanikaunda@gmail.com");
                reqparm.Add("PhysicalLocation", "zombas");
                reqparm.Add("Username", "kajanikaunda@gmail.com");


                client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                client.Headers.Add("candidateid", "kajanikaunda@gmail.com");
                client.Headers.Add("apikey", "2c798bc8-731a-46ce-81e9-ac8977ddc81c");


                byte[] responsebytes = client.UploadValues("https://www.mra.mw/sandbox/programming/challenge/webservice/Taxpayers/add", "POST", reqparm);
                string responsebody = Encoding.UTF8.GetString(responsebytes);


                MessageBox.Show(responsebody);


                MessageBox.Show(client.ResponseHeaders.ToString());



            }




        }


        // Edit record...
        private void buttonEdit_Click(object sender, EventArgs e)
        {
            if (pub_isloggedin == false)
            { MessageBox.Show("Login first."); return; }

            // set selected task
            pub_wstask = "EDIT";


            using (WebClient client = new WebClient())
            {
                var reqparm = new System.Collections.Specialized.NameValueCollection();
                reqparm.Add("TPIN", "55545555");
                reqparm.Add("BusinessCertificateNumber", "HHHHHH");
                reqparm.Add("TradingName", "edited");
                reqparm.Add("BusinessRegistrationDate", "2021/11/06");
                reqparm.Add("MobileNumber", "232443433");
                reqparm.Add("Email", "kajanikaunda@gmail.com");
                reqparm.Add("PhysicalLocation", "zombas");
                reqparm.Add("Username", "kajanikaunda@gmail.com");

                client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                client.Headers.Add("candidateid", "kajanikaunda@gmail.com");
                client.Headers.Add("apikey", "2c798bc8-731a-46ce-81e9-ac8977ddc81c");


                byte[] responsebytes = client.UploadValues("https://www.mra.mw/sandbox/programming/challenge/webservice/Taxpayers/edit", "POST", reqparm);
                string responsebody = Encoding.UTF8.GetString(responsebytes);


                MessageBox.Show(responsebody);


                MessageBox.Show(client.ResponseHeaders.ToString());


            }

        }


        // Delete record
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (pub_isloggedin == false)
            { MessageBox.Show("Login first."); return; }
            
            // set selected task
            pub_wstask = "DELETE";


            using (WebClient client = new WebClient())
            {
                var reqparm = new System.Collections.Specialized.NameValueCollection();
                reqparm.Add("TPIN", this.textBoxTPIN .Text );
              

                client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                client.Headers.Add("candidateid", this.textBoxMRAUsername .Text );
                client.Headers.Add("apikey", "2c798bc8-731a-46ce-81e9-ac8977ddc81c");


                byte[] responsebytes = client.UploadValues("https://www.mra.mw/sandbox/programming/challenge/webservice/Taxpayers/delete", "POST", reqparm);
                string responsebody = Encoding.UTF8.GetString(responsebytes);

                this.textBox1.Text = responsebody;
                this.textBox1.Refresh();  

            }

        }


        //

        public class Person
        {
            public string Name;
            public int Age;
            public string Location;
        }
        public class Record
        {
            public Person record;
        }


        //public class Person
        //{
        //    //Fields - model the taxpayer data
        //    public string _c_tpin;
        //    public string _c_businesscertificatenumber;
        //    public string _c_tradingname;
        //    public string _c_businessregistrationdate;
        //    public string _c_mobilenumber;
        //    public string _c_email;
        //    public string _c_physicallocation;
        //    public string _c_username;
        //    public Boolean _c_deleted;
        //    public int _c_id;
        //}
        //public class People
        //{
        //    public Person[] people;
        //}

        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
        public class taxperson
        {
            public string TPIN { get; set; }
            public string BusinessCertificateNumber { get; set; }
            public string TradingName { get; set; }
            public string BusinessRegistrationDate { get; set; }
            public string MobileNumber { get; set; }
            public string Email { get; set; }
            public string PhysicalLocation { get; set; }
            public string Username { get; set; }
            public bool Deleted { get; set; }
            public int id { get; set; }
        }



        //public class taxpersonnn
        //{

        //    public string c_tpin { get; set; }
        //    public string c_businesscertificatenumber { get; set; }
        //    public string c_tradingname { get; set; }
        //    public string c_businessregistrationdate { get; set; }
        //    public string c_mobilenumber { get; set; }
        //    public string c_email { get; set; }
        //    public string c_physicallocation { get; set; }
        //    public string c_username { get; set; }
        //    public Boolean c_deleted { get; set; }
        //    public int c_id { get; set; }
        //}

        public class taxpersonCollection
        {
            public taxperson[] taxpeople {get;set;}
        }


        // View all records
        private void buttonViewAll_Click(object sender, EventArgs e)
        {
            if (pub_isloggedin == false)
            { MessageBox.Show("Login first."); return; }


            // set selected task
            pub_wstask = "VIEW";

            using (WebClient client = new WebClient())
            {
                var reqparm = new System.Collections.Specialized.NameValueCollection();

                reqparm.Add("", "");


                client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                client.Headers.Add("candidateid", this.textBoxMRAUsername.Text);
                client.Headers.Add("apikey", "864be0ba-c0dd-4f8e-8af2-cafeff7f8e65");

      

                string responsestring = client.DownloadString("https://www.mra.mw/sandbox/programming/challenge/webservice/Taxpayers/getAll");
        

                MessageBox.Show(responsestring);


                MessageBox.Show(client.ResponseHeaders.ToString());

                //-----------------------------------------------------------------------------------------------------------------------------------------------------------

                //var table = JsonConvert.DeserializeObject<datatable>(responsestring);

                DataTable dt = (DataTable)JsonConvert.DeserializeObject(responsestring, (typeof(DataTable)));
                this.taxpeopleDataGridView .DataSource = dt ;




                //if (taxpeopleDataGridView.SelectedCells.Count > 0)
                //{
                //    string id = taxpeopleDataGridView.SelectedCells[0].Value.ToString();

                //    MessageBox.Show(id.ToString ()+" <=> CELL");
                //}






                //taxpeopleDataGridView.SelectionChanged 



                //this.taxpeopleDataGridView.DataBindings();

                //this.taxpeopleDataGridView.Refresh();

                //this.taxpeopleDataGridView .


                //this.taxpeopleDataGridView.Rows .

                //this.taxpeopleDataGridView.DataSource .;





                //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                //taxpersonCollection tmp = JsonConvert.DeserializeObject<taxpersonCollection>(responsestring);

                //this.taxpeopleDataGridView.DataSource = tmp;
                //this.taxpeopleDataGridView.Refresh ();


                MessageBox.Show("wait");

                //foreach (string typeStr in tmp.taxpeople[2].TPIN.ToString() )//tmp.type[0])
                //{
                //    // Do something with typeStr
                //}


                // &&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&


               // LOP DataContractJsonSerializer serializerObject = new DataContractJsonSerializer(typeof(taxpersonCollection));




                //taxpersonCollection taxpersonCollectionObject = (taxpersonCollection)serializerObject.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(responsestring))); //Jsonstream);

              
                //MessageBox.Show("Count=" + taxpersonCollectionObject.taxpeople.Count().ToString ());
                

                //byte[] byteArray = Encoding.ASCII.GetBytes(responsestring.ToString ());

                //if (byteArray != null && byteArray.Length > 0)
                //{
                //    MessageBox.Show ("not null and not empty");
                //}
                //else
                //{
                //    MessageBox.Show("null");
                //}

                //MemoryStream Jsonstream = new MemoryStream(byteArray);


                //taxpersonCollection taxpersonCollectionObject = (taxpersonCollection)serializerObject.ReadObject(Jsonstream);

                //try { }
                //catch (Exception e)
                //{}

           // LOP     taxpersonCollection taxpersonCollectionObject = (taxpersonCollection)serializerObject.ReadObject(new MemoryStream(File.ReadAllBytes(@"C:\MRA\j.txt")));


                // ok

                //if (taxpersonCollectionObject != null)
                //{
                //    MessageBox.Show ("not null and not empty - again");
                //}
                //else
                //{
                //    MessageBox.Show("null - again");
                //}



                //taxpersonCollection m = new taxpersonCollection();

                //this.taxpeopleDataGridView.DataSource = taxpersonCollectionObject;

                //this.taxpeopleDataGridView.Refresh();


                //m.taxpeople .




                //taxperson f = new List<taxperson>(taxpersonCollectionObject);







                //MessageBox.Show(taxpersonCollectionObject.TPIN.ToString());




                //List<taxpersonCollection> primeNumbers = new List<taxpersonCollection>();

                //primeNumbers = new List<taxpersonCollectionObject> ;



                //taxpersonCollection[] yes = new taxpersonCollectionObject[];

                //string h = new List<taxpersonCollectionObject>(taxperson);


                //taxpersonCollectionObject.taxpeople[].



                //for each (i in taxpersonCollectionObject)


                //int totalElements;
                //string[] animals = { "Cat", "Alligator", "fox", "donkey", "Cat", "alligator" };
                //totalElements = animals.Count(); //6

                //totalElements = taxpersonCollectionObject.taxpeople.Count();


                //MessageBox.Show(taxpersonCollectionObject.taxpeople.Count().ToString());
                //MessageBox.Show(totalElements.ToString());



                //string tpinn = taxpersonCollectionObject.taxpeople[1].c_tpin;
                //MessageBox.Show("Mt TPIN: " + tpinn.ToString());





                //-----------------------------------------------------------------------------------------------------------------------------------------------------------

                // deserialise and put data into objects for view and any other processing

                //((

                //JavaScriptSerializer ser = new JavaScriptSerializer();
                //People Person = new ser.Deserialize<List<People>>(responsestring);

                //JavaScriptSerializer serd = new JavaScriptSerializer();
                //var records = new serd.Deserialize<List<Record>>(responsestring);

                

                //taxpayerCollectionClass taxpayerCollectionClass = js.Deserialize<taxpayerCollectionClass>(responsestring)
                //((


                //===
                //JavaScriptSerializer js = new JavaScriptSerializer();

                ////ok


                //taxpayerCollectionClass  pan = new taxpayerCollectionClass() ;

                


                //taxpayerCollectionClass taxpayerCollectionClass = js.Deserialize<taxpayerCollectionClass>(responsestring);//e

                //pan = js.Deserialize<taxpayerCollectionClass>(responsestring);//e

                

                //MessageBox.Show(pan.taxpayers.Count.ToString ());




                //public object Deserialize (string responsestring, Type taxpayerCollectionClass);





                ////taxpayerCollectionClass taxpayerCollectionClass = js.Deserialize<taxpayerCollectionClass>(responsestring);

                ////BlogSites blogObject = js.Deserialize<BlogSites>(jsonData);

                ////string name = blogObject.Name;Messages
                ////string desk = blogObject.Desk;


                ////string mytpin = taxpayerClass._tpin;
                //MessageBox.Show(taxpayerCollectionClass.taxpayers.Count.ToString());
                //===
                //----

                ////
                //////var jsonData = "{\"Name\":\"john\", \"Desk\":22,\"Name\":\"mary\", \"Desk\":20 }";
                //MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(responsestring));
                //DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(taxpayerCollectionClass));

                ////Type 'MRAtaxpayerWS.taxpayerCollectionClass' cannot be serialized. Consider marking it with 
                ////the DataContractAttribute attribute, and marking all of its members you want serialized with 
                ////the DataMemberAttribute attribute.  If the type is a collection, consider marking it with the
                ////CollectionDataContractAttribute.  See the Microsoft .NET Framework documentation for other supported types.


                //var deserialized = ser.ReadObject(ms) as taxpayerCollectionClass; //e
                //ms.Close();

                ////BlogSites bs = deserialized;

                ////foreach (var item in bs.Desk)
                ////{
                ////    Console.WriteLine(item);
                ////    MessageBox.Show("[-"+item.ToString() );
                ////}
                //-----
                //
                

                //try
                //{
                //    byte[] responsebytes = client.UploadValues("https://www.mra.mw/sandbox/programming/challenge/webservice/Taxpayers/getAll", "GET", reqparm);
                  
                //    string responsebody = Encoding.UTF8.GetString(responsebytes);


                //    MessageBox.Show(responsebody);


                //    MessageBox.Show(client.ResponseHeaders.ToString());

                //}
                //catch (WebException ex)
                //{

                //    MessageBox.Show(ex.Message );


                
                //}





              


            }





        }

        // ###########################

        public class School
        {
        public string SchoolName {get;set;}
        public string SchoolAddress {get;set;}
        public int ZipCode {get;set;}
        public Teacher[] Teachers {get;set;}
        }

        public class Teacher
        {
        public int TeacherId {get;set;}
        public string TeacherName {get;set;}
        }

        //



        //



        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (pub_isloggedin == false)
            { MessageBox.Show("Login first."); return; }
    
        }

        private void buttonClearPanels_Click(object sender, EventArgs e)
        {
            if (pub_isloggedin == false)
            { MessageBox.Show("Login first."); return; }

            // no time to develop. but optional function.
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            if (pub_isloggedin == false)
            { MessageBox.Show("Login first."); return; }
        }
      

     





















    }
}
