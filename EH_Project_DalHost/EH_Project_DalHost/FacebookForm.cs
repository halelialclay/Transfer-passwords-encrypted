using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EH_Project_DalHost
{
    public partial class FacebookForm : Form
    {
        bool firstTime = true;
        string userName = "";
        string password = "";

        public FacebookForm()
        {
            InitializeComponent();
        }

        private void FacebookForm_Load(object sender, EventArgs e)
        {
            SetUserAndPassword();
            webView21.Source = new Uri("https://www.facebook.com");
          
        }

        private void SetUserAndPassword()
        {
            TEEConnector te = new TEEConnector();

            try
            {
                te.DALCreatSession();

                string userid = "{c7cdf2ed-cc9e-45c9-b042-9ca240e207a8}";
                //string userName = "haleli";
                //  te.WriteKeyToTEE(userid, userName);
                KeyValuePair<string,string> a= te.CheckIsAuthorized();

                userName = a.Key;
                password = a.Value;
                
            }
            finally
            {
                te.DALCloseSession();
            }
        }

        private void webView21_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            if (firstTime)
            {
                firstTime = false;
                string script = "var col = document.getElementsByTagName('button'); for(var i=0;i<col.length;i++) {if(col[i].name=='login'){col[i].click();break;}}";

                webView21.CoreWebView2.ExecuteScriptAsync(string.Format("document.getElementById('email').value='{0}'",userName));
                webView21.CoreWebView2.ExecuteScriptAsync(string.Format("document.getElementById('pass').value='{0}'",password));
                webView21.CoreWebView2.ExecuteScriptAsync(script);
            }
        }
    }
}
