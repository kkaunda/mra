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
using Newtonsoft.Json.Linq;
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

        // set business registration date ... because we need to change its format for the web request
        // public Date  pub_BusinessRegistrationDate; 
        public string pub_year = "";
        public string pub_month = "";
        public string pub_day = "";
        public string pub_date = "";

        // data table used in get all 
        public DataTable dt; 
        
        // login variables
        public Boolean pub_isloggedin = false;
 
        public string pub_login_authenticated = "";
        public string pub_login_remark = "";
        public string pub_login_apikey = "";
        public string pub_login_username = "";
        public string pub_login_firstname = "";
        public string pub_login_lastname = "";
                     
        public int pub_logout_ok = 0;
        public string pub_logout_remark = "";

        // json_responsestring
        public string json_responsestring = ""; 

        // end: public variables ...

        // login..ok
        private void buttonLogin_Click(object sender, EventArgs e)
        {
            string Credentials_data = "Email=" + this.textBoxMRAUsername.Text + "&" + "Password=" + this.textBoxMRAPassword.Text;
            using (WebClient client = new WebClient())
            {
                // set content type
                client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";

                // call API with url and user credentials
                string result = client.UploadString("https://www.mra.mw/sandbox/programming/challenge/webservice/auth/login", Credentials_data);
                var myJObject = JObject.Parse(result);
                       
                // comment this section when done
//                JObject myJObject = JObject.Parse(@"{
//               'ResultCode': 1,
//               'Remark': 'Successful',
//               'Token': {
//                        'Name': 'Kajanikaunda@gmail.com',
//                        'Value': '161fd72e-9bfe-4924-a22a-322f66bd5b9d'
//                        },
//               'Authenticated': true,
//               'UserDetails': {
//                              'Username': 'Kajanikaunda@gmail.com',
//                              'Password': 'password000122',
//                              'FirstName': 'Kajani',
//                              'LastName': 'Kaunda',
//                              'email': 'Kajanikaunda@gmail.com'
//                              }
//                }");
                // comment this section when done

                // debugging stuff remove once done
                //this.textBox1.Text = result;
                //this.textBox1.Refresh();

                // check if authenticated
                pub_login_authenticated = (string)myJObject.SelectToken("Authenticated");
                pub_login_remark = (string)myJObject.SelectToken("Remark");

                // get data depending on authentication
                if   (pub_login_authenticated == "True")
                     {
                         pub_login_apikey = (string)myJObject.SelectToken("Token.Value");
                         pub_login_username = (string)myJObject.SelectToken("UserDetails.Username");
                         pub_login_firstname = (string)myJObject.SelectToken("UserDetails.FirstName");
                         pub_login_lastname = (string)myJObject.SelectToken("UserDetails.LastName");
                         //
                         MessageBox.Show("Login Succeeded: " + pub_login_remark);
                         //
                         pub_isloggedin = true;
                         this.textBoxMRAUsername.Enabled = false;
                         this.textBoxMRAPassword.Enabled = false;
                         this.buttonLoggedUser.Text = "Logged in user is: " + pub_login_firstname + " " + pub_login_lastname;

                     }
                else {
                         MessageBox.Show("Login Failed: " + pub_login_remark);
                         pub_isloggedin = false;
                     }
            }
        }

        // exit the app..ok
        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // logout..ok
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
                var myJObject = JObject.Parse(result);


                //if successful - re-enable user text box ...

                // debugging stuff ... remove once done
                //this.textBox1.Text = result;
                //this.textBox1.Refresh();
                // debugging stuff ... remove once done

                //...
                // check if successful
                pub_logout_ok = (int)myJObject.SelectToken("ResultCode");
                pub_logout_remark = (string)myJObject.SelectToken("Remark");

                // get data depending on authentication
                if (pub_logout_ok == 1)
                {
                    //
                    MessageBox.Show("Logout Succeeded: " + pub_logout_remark);
                    //
                    pub_isloggedin = false;
                    this.textBoxMRAUsername.Enabled = true;
                    this.textBoxMRAPassword.Enabled = true;
                    this.buttonLoggedUser.Text = "";
                    this.buttonLoggedUser.Refresh();
                    this.textBoxMRAUsername.Clear();
                    this.textBoxMRAPassword.Clear();
                }
                else
                {
                    MessageBox.Show("Logout Failed: " + pub_logout_remark);
                    pub_isloggedin = true;
                }
             }
        }

        // register a tax payer
        private void buttonRegister_Click(object sender, EventArgs e)
        {
            if (pub_isloggedin == false)
            { MessageBox.Show("Login first."); return; }

            // if any field is empty do not proceed
            if (this.textBoxTPIN.Text == "") { MessageBox.Show("Empty TPIN field. Edit aborted"); return; };
            if (this.textBoxBUSINESSCERTIFICATENUMBER.Text == "") { MessageBox.Show("Empty Business Certificate Number field. Edit aborted"); return; };
            if (this.textBoxTRADINGNAME.Text == "") { MessageBox.Show("Empty Trading Name field. Edit aborted"); return; };

            // vaildate/format date first - date is validated because i am using a date time picker which will always have the correct date

            pub_year = dateTimePickerBUSINESSREGISTRATIONDATE.Value.Year.ToString("0000");
            pub_month = dateTimePickerBUSINESSREGISTRATIONDATE.Value.Month.ToString("00");
            pub_day = dateTimePickerBUSINESSREGISTRATIONDATE.Value.Day.ToString("00");
            pub_date = String.Format("{0:0000}", pub_year) + "/" + String.Format("{0:00}", pub_month) + "/" + String.Format("{0:00}", pub_day);
            //MessageBox.Show(pub_date.ToString());

            if (pub_date.ToString() == "") { MessageBox.Show("Empty Business Registration Date field. Edit aborted"); return; };

            if (this.textBoxMOBILENUMBER.Text == "") { MessageBox.Show("Empty Mobile Number field. Edit aborted"); return; };
            if (this.textBoxEMAIL.Text == "") { MessageBox.Show("Empty Email field. Edit aborted"); return; };
            if (this.textBoxPHYSICALLOCATION.Text == "") { MessageBox.Show("Empty Physical Location field. Edit aborted"); return; };
            if (this.textBoxUSERNAME.Text == "") { MessageBox.Show("Empty User Name field. Edit aborted"); return; };
            if (this.textBoxDELETED.Text == "") { MessageBox.Show("Empty Deleted field. Edit aborted"); return; };
            // more data validations required. eg is email in correct format? does it exist? is physical location is this country? verified by utilty bill?



            using (WebClient client = new WebClient())
            {
                // build parameters to pass to the web request from the App form. 

                var reqparm = new System.Collections.Specialized.NameValueCollection();
                reqparm.Add("TPIN", this.textBoxTPIN.Text);
                reqparm.Add("BusinessCertificateNumber", this.textBoxBUSINESSCERTIFICATENUMBER.Text);
                reqparm.Add("TradingName", this.textBoxTRADINGNAME.Text);
                reqparm.Add("BusinessRegistrationDate", pub_date.ToString());
                reqparm.Add("MobileNumber", this.textBoxMOBILENUMBER.Text);
                reqparm.Add("Email", this.textBoxEMAIL.Text);
                reqparm.Add("PhysicalLocation", this.textBoxPHYSICALLOCATION.Text);
                reqparm.Add("Username", this.textBoxUSERNAME.Text);




                //var reqparm = new System.Collections.Specialized.NameValueCollection();
                //reqparm.Add("TPIN", this.textBoxTPIN.Text );
                //reqparm.Add("BusinessCertificateNumber", this.textBoxBUSINESSCERTIFICATENUMBER.Text );
                //reqparm.Add("TradingName", this.textBoxTRADINGNAME.Text );
                //reqparm.Add("BusinessRegistrationDate", pub_date);
                //reqparm.Add("MobileNumber", this.textBoxMOBILENUMBER.Text );
                //reqparm.Add("Email", this.textBoxEMAIL.Text);
                //reqparm.Add("PhysicalLocation", this.textBoxPHYSICALLOCATION.Text);
                //reqparm.Add("Username", this.textBoxUSERNAME.Text);
                
                client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                client.Headers.Add("candidateid", this.textBoxMRAUsername.Text );
                client.Headers.Add("apikey", pub_login_apikey);
                
                //byte[] responsebytes = client.UploadValues("https://www.mra.mw/sandbox/programming/challenge/webservice/Taxpayers/add", "POST", reqparm);
                //string responsebody = Encoding.UTF8.GetString(responsebytes);

                try 
                {

                    byte[] responsebytes = client.UploadValues("https://www.mra.mw/sandbox/programming/challenge/webservice/Taxpayers/add", "POST", reqparm);
                    string responsebody = Encoding.UTF8.GetString(responsebytes);
                    //MessageBox.Show(responsebody);
                    //MessageBox.Show(client.ResponseHeaders.ToString());
                }
                catch (WebException ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                    return;
                }


                //MessageBox.Show(responsebody);
                //MessageBox.Show(client.ResponseHeaders.ToString());
            }
            MessageBox.Show("Tax payer is now Registered");
        }

        // edit record
        private void buttonEdit_Click(object sender, EventArgs e)
        {
            if (pub_isloggedin == false)
            { MessageBox.Show("Login first."); return; }

            // if any field is empty do not proceed
            if (this.textBoxTPIN.Text == "") { MessageBox.Show("Empty TPIN field. Edit aborted");  return; };
            if (this.textBoxBUSINESSCERTIFICATENUMBER.Text == "") { MessageBox.Show("Empty Business Certificate Number field. Edit aborted"); return; }; 
            if (this.textBoxTRADINGNAME.Text == "") { MessageBox.Show("Empty Trading Name field. Edit aborted"); return; };

            // vaildate/format date first - date is validated because i am using a date time picker which will always have the correct date

            pub_year = dateTimePickerBUSINESSREGISTRATIONDATE.Value.Year.ToString("0000");
            pub_month = dateTimePickerBUSINESSREGISTRATIONDATE.Value.Month.ToString("00");
            pub_day = dateTimePickerBUSINESSREGISTRATIONDATE.Value.Day.ToString("00");
            pub_date = String.Format("{0:0000}", pub_year) + "/" + String.Format("{0:00}", pub_month) + "/" + String.Format("{0:00}", pub_day);
            //MessageBox.Show(pub_date.ToString());
            
            if (pub_date.ToString () == "") { MessageBox.Show("Empty Business Registration Date field. Edit aborted"); return; }; 
  
            if (this.textBoxMOBILENUMBER.Text == "") { MessageBox.Show("Empty Mobile Number field. Edit aborted"); return; };
            if (this.textBoxEMAIL.Text == "") { MessageBox.Show("Empty Email field. Edit aborted"); return; };
            if (this.textBoxPHYSICALLOCATION.Text == "") { MessageBox.Show("Empty Physical Location field. Edit aborted"); return; };
            if (this.textBoxUSERNAME.Text == "") { MessageBox.Show("Empty User Name field. Edit aborted"); return; };
            if (this.textBoxDELETED.Text == "") { MessageBox.Show("Empty Deleted field. Edit aborted"); return; }; 
            // more data validations required. eg is email in correct format? does it exist? is physical location is this country? verified by utilty bill?

             using (WebClient client = new WebClient())
            {
                var reqparm = new System.Collections.Specialized.NameValueCollection();
                reqparm.Add("TPIN", this.textBoxTPIN .Text );
                reqparm.Add("BusinessCertificateNumber", this.textBoxBUSINESSCERTIFICATENUMBER .Text );
                reqparm.Add("TradingName", this.textBoxTRADINGNAME .Text );
                reqparm.Add("BusinessRegistrationDate", pub_date.ToString ());
                reqparm.Add("MobileNumber", this.textBoxMOBILENUMBER .Text );
                reqparm.Add("Email", this.textBoxEMAIL .Text );
                reqparm.Add("PhysicalLocation", this.textBoxPHYSICALLOCATION .Text );
                reqparm.Add("Username", this.textBoxUSERNAME .Text );
                reqparm.Add("Deleted", this.textBoxDELETED .Text );

                client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                client.Headers.Add("candidateid", this.textBoxMRAUsername.Text);
                client.Headers.Add("apikey", pub_login_apikey);


                //byte[] responsebytes = client.UploadValues("https://www.mra.mw/sandbox/programming/challenge/webservice/Taxpayers/edit", "POST", reqparm);

                try
                {
                    byte[] responsebytes = client.UploadValues("https://www.mra.mw/sandbox/programming/challenge/webservice/Taxpayers/edit", "POST", reqparm);
                    string responsebody = Encoding.UTF8.GetString(responsebytes);
                    //MessageBox.Show(responsebody);
                    //MessageBox.Show(client.ResponseHeaders.ToString());

                }
                catch (WebException ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                    return;
                }

                //string responsebody = Encoding.UTF8.GetString(responsebytes);
                //MessageBox.Show(responsebody);
                //MessageBox.Show(client.ResponseHeaders.ToString());
            }

             MessageBox.Show("Record Edited and saved");

        }

        // delete record
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (pub_isloggedin == false)
            { MessageBox.Show("Login first."); return; }
            
            using (WebClient client = new WebClient())
            {
                var reqparm = new System.Collections.Specialized.NameValueCollection();
                reqparm.Add("TPIN", this.textBoxTPIN .Text );
 
                client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                client.Headers.Add("candidateid", this.textBoxMRAUsername .Text );
                client.Headers.Add("apikey", pub_login_apikey);

                try
                {
                    byte[] responsebytes = client.UploadValues("https://www.mra.mw/sandbox/programming/challenge/webservice/Taxpayers/delete", "POST", reqparm);
                    string responsebody = Encoding.UTF8.GetString(responsebytes);


                }
                catch (WebException ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                    return;
                }

                // debugging stuff remove once done
                //this.textBox1.Text = responsebody;
                //this.textBox1.Refresh();
                // debugging stuff remove once done

            }

            MessageBox.Show("Record Deleted");

        }

        // tax payer sub class 
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

        // tax payer class
        public class taxpersonCollection
        {
            public taxperson[] taxpeople {get;set;}
        }

        // view all records
        private void buttonViewAll_Click(object sender, EventArgs e)
        {
            if (pub_isloggedin == false)
            { MessageBox.Show("Login first."); return; }

            using (WebClient client = new WebClient())
            {
                var reqparm = new System.Collections.Specialized.NameValueCollection();

                reqparm.Add("", "");

                client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                client.Headers.Add("candidateid", this.textBoxMRAUsername.Text);
                client.Headers.Add("apikey", pub_login_apikey);

                try
                {
                    json_responsestring = client.DownloadString("https://www.mra.mw/sandbox/programming/challenge/webservice/Taxpayers/getAll");
                }
                catch (WebException ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                    return;
                }

                json_responsestring = client.DownloadString("https://www.mra.mw/sandbox/programming/challenge/webservice/Taxpayers/getAll");

                // debugging stuff remove once done
                //this.textBox1.Text = json_responsestring;
                //this.textBox1.Refresh();

                // deserialise json string returned by web service to a data table
                dt = (DataTable)JsonConvert.DeserializeObject(json_responsestring, (typeof(DataTable)));

                // filter to show only records belonging to logged user
                dt.Select("Username = '" + pub_login_username + "'");
                              
                // attach data table to data grid as its data source
                this.taxpeopleDataGridView.DataSource = dt;
                
                // when row selection changes on the grid, update the data input panel
                // done - implemented in the procedure [taxpeopleDataGridView_CellContentClick]

                // process completion message
                MessageBox.Show("All records retrieved");
            }
        }
    
        // clear data input panel - get it ready for adding etc..ok
        private void buttonClearDataInputPanel_Click(object sender, EventArgs e)
        {
            if (pub_isloggedin == false)
            { MessageBox.Show("Login first."); return; }

            // clear app form data fields
            this.textBoxTPIN.Clear();
            this.textBoxBUSINESSCERTIFICATENUMBER.Clear();
            this.textBoxTRADINGNAME.Clear();
            this.dateTimePickerBUSINESSREGISTRATIONDATE.Value = DateTime.Today ;
            this.textBoxMOBILENUMBER.Clear();
            this.textBoxEMAIL.Clear();
            this.textBoxPHYSICALLOCATION.Clear();
            this.textBoxUSERNAME.Clear();
            this.textBoxDELETED.Clear();
            this.textBoxID.Clear();

            // completion mesaage
            MessageBox.Show("Panel Cleared.");
        }

        // clear data grid panel..ok
        private void buttonClearDataGridPanel_Click(object sender, EventArgs e)
        {
            if (pub_isloggedin == false)
            { MessageBox.Show("Login first."); return; }

            // clear data grid view control
            dt.Rows.Clear();
            this.taxpeopleDataGridView.Refresh ();

            // completion mesaage
            MessageBox.Show("Panel Cleared.");
        }

        public void taxpeopleDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        //    //taxpeopleDataGridView.Refresh();
          
        //        // update the data input form with each change in row.
        //        if (e.RowIndex >= 0)
        //        {
        //            this.textBoxTPIN.Text = this.taxpeopleDataGridView.Rows[e.RowIndex].Cells[0].Value.ToString();
        //            this.textBoxBUSINESSCERTIFICATENUMBER.Text = this.taxpeopleDataGridView.Rows[e.RowIndex].Cells[1].Value.ToString();
        //            this.textBoxTRADINGNAME.Text = this.taxpeopleDataGridView.Rows[e.RowIndex].Cells[2].Value.ToString();
        //            this.dateTimePickerBUSINESSREGISTRATIONDATE.Value = Convert.ToDateTime(this.taxpeopleDataGridView.Rows[e.RowIndex].Cells[3].Value.ToString());
        //            this.textBoxMOBILENUMBER.Text = this.taxpeopleDataGridView.Rows[e.RowIndex].Cells[4].Value.ToString();
        //            this.textBoxEMAIL.Text = this.taxpeopleDataGridView.Rows[e.RowIndex].Cells[5].Value.ToString();
        //            this.textBoxPHYSICALLOCATION.Text = this.taxpeopleDataGridView.Rows[e.RowIndex].Cells[6].Value.ToString();
        //            this.textBoxUSERNAME.Text = this.taxpeopleDataGridView.Rows[e.RowIndex].Cells[7].Value.ToString();
        //            this.textBoxDELETED.Text = this.taxpeopleDataGridView.Rows[e.RowIndex].Cells[8].Value.ToString();
        //            this.textBoxID.Text = this.taxpeopleDataGridView.Rows[e.RowIndex].Cells[9].Value.ToString();
        //        }
        //        taxpeopleDataGridView.Refresh();
        }

        public void taxpeopleDataGridView_CellMouseClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        public void GetRecord()
        {
            
            this.textBoxTPIN.Text = this.taxpeopleDataGridView.CurrentRow .Cells [0].Value .ToString ();
            this.textBoxBUSINESSCERTIFICATENUMBER.Text = this.taxpeopleDataGridView.CurrentRow.Cells[1].Value.ToString();
            this.textBoxTRADINGNAME.Text = this.taxpeopleDataGridView.CurrentRow.Cells[2].Value.ToString();
            this.dateTimePickerBUSINESSREGISTRATIONDATE.Value = Convert.ToDateTime(this.taxpeopleDataGridView.CurrentRow .Cells [3].Value .ToString ());
            this.textBoxMOBILENUMBER.Text = this.taxpeopleDataGridView.CurrentRow .Cells [4].Value .ToString ();
            this.textBoxEMAIL.Text = this.taxpeopleDataGridView.CurrentRow.Cells[5].Value.ToString();
            this.textBoxPHYSICALLOCATION.Text = this.taxpeopleDataGridView.CurrentRow.Cells[6].Value.ToString();
            this.textBoxUSERNAME.Text = this.taxpeopleDataGridView.CurrentRow.Cells[7].Value.ToString();
            this.textBoxDELETED.Text = this.taxpeopleDataGridView.CurrentRow.Cells[8].Value.ToString();
            this.textBoxID.Text = this.taxpeopleDataGridView.CurrentRow.Cells[9].Value.ToString();
    
        }

        // select record for editing. put selected record in data grid to the data input panel.
        private void buttonSelect_Click(object sender, EventArgs e)
        {
            GetRecord();
        }


















    }
}

