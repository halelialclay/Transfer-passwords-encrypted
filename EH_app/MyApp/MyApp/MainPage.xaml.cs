using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyApp
{
    public partial class MainPage : ContentPage
    {
        string urlBase = "https://haleli.bemazaltov.co.il/";
        //string urlBase = "http://10.0.2.2:60469/";
        string userid = "{c7cdf2ed-cc9e-45c9-b042-9ca240e207a8}";
        RSA Rsa;

        // int count = 0;
        public MainPage()
        {
            InitializeComponent();

            StartApp();
        }

        private void StartApp()
        {
            if (!Xamarin.Forms.Application.Current.Properties.ContainsKey("installed"))
            {
                Xamarin.Forms.Application.Current.Properties["installed"] = "true";

                Rsa = new RSACryptoServiceProvider();

                RSAParameters parameters = Rsa.ExportParameters(true);
                byte[] Ex = parameters.Exponent;
                byte[] mod = parameters.Modulus;

                byte[] publicKey = new byte[131];

                publicKey = mod.Concat(Ex).ToArray();

                string strPublicKey = Convert.ToBase64String(publicKey);

                string res;

                string sUrl = urlBase + "api/startapp";

                string json = "{id:\"" + userid + "\", key:\"" + strPublicKey + "\"}";
                //string json = "{id:\"aaa\", key:\"vvvv\"}";
                using (WebClient cli = new WebClient())
                {
                    cli.Headers[HttpRequestHeader.ContentType] = "application/json";
                    res = cli.UploadString(sUrl, "POST", json);
                }

                StartBlock.IsVisible = true;
            }
            else
            {
                ShowFormBlock();
            }
        }

        private void ShowFormBlock()
        {
            FormBlock.IsVisible = true;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {

            string email =  emailEntry.Text;
            string pass = passwordEntry.Text;

            string userid = "{c7cdf2ed-cc9e-45c9-b042-9ca240e207a8}";
            DateTime time = DateTime.UtcNow;
            string s_time = Convert.ToString(time);
            

            string res1 = null;
            if (App.Current.Properties.ContainsKey("symmetric"))
            {
                res1 = (string)Application.Current.Properties["symmetric"];
            }
            byte[] symmetric = Convert.FromBase64String(res1);

            string sUrl = urlBase + "api/app";



            string split ="$$$";

            string strEnc =  email+split+pass+split+ s_time;


            //te[] plainText1 = Convert.FromBase64String(strEnc);
            byte[] plainText1 = System.Text.Encoding.UTF8.GetBytes(strEnc);

            string res = Encrypt( plainText1, symmetric);

            //string json = "{id:\"" + userid + "\",encdata:\"" + res + "\"}";
            AppData data = new AppData() { id = userid, encdata = res };

            using (WebClient cli = new WebClient())
            {
                cli.Headers[HttpRequestHeader.ContentType] = "application/json";
                string response = cli.UploadString(sUrl, JsonConvert.SerializeObject(data));
            }
        }

      

        public static string Encrypt(byte[] plaintext, byte[] key)
        {
            byte[] IV = new byte[16];
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.KeySize = 128;
            aes.BlockSize = 128;
            aes.Padding = PaddingMode.Zeros;
            aes.Mode = CipherMode.ECB;

            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(key, IV), CryptoStreamMode.Write);
            cs.Write(plaintext, 0, plaintext.Length);
            cs.FlushFinalBlock();

            return Convert.ToBase64String(ms.ToArray());

        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            bool flag = false;
            string res1 = null;
            while (flag == false)
            {
                string sUrl = urlBase + "api/Symmetric?id=" + userid;
                using (WebClient cli = new WebClient())
                {
                    cli.Headers[HttpRequestHeader.ContentType] = "application/json";
                    res1 = cli.DownloadString(sUrl);
                }

                if (string.IsNullOrEmpty(res1))
                    Thread.Sleep(30000);
                else
                    flag = true;
            }

            byte[] symmetric = Convert.FromBase64String(res1);
            byte[] decryptedData = new byte[16];
            decryptedData = Rsa.Decrypt(symmetric, RSAEncryptionPadding.Pkcs1);

            string strsymmetricKey = Convert.ToBase64String(decryptedData);

            App.Current.Properties["symmetric"] = strsymmetricKey;

            StartBlock.IsVisible = false;

            ShowFormBlock();
        }
    }

    public class AppData
    {
        public string id { get; set; }
        public string encdata { get; set; }
    }
}
