using Intel.Dal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace EH_Project_DalHost
{
    public class TEEConnector
    {
        // This is the UUID of this Trusted Application (TA).
        //The UUID is the same value as the applet.id field in the Intel(R) DAL Trusted Application manifest.
        string appletID = "09b869cf-1c56-4572-b0d7-b19cba3fe862";
        string urlBase = "https://haleli.bemazaltov.co.il/";
        //string urlBase = "http://localhost:60469/";

        private Jhi jhi;// = Jhi.Instance;
        JhiSession session = null;


        public bool DALCreatSession()
        {
#if AMULET
            // When compiled for Amulet the Jhi.DisableDllValidation flag is set to true 
            // in order to load the JHI.dll without DLL verification.
            // This is done because the JHI.dll is not in the regular JHI installation folder, 
            // and therefore will not be found by the JhiSharp.dll.
            // After disabling the .dll validation, the JHI.dll will be loaded using the Windows search path
            // and not by the JhiSharp.dll (see http://msdn.microsoft.com/en-us/library/7d83bc18(v=vs.100).aspx for 
            // details on the search path that is used by Windows to locate a DLL) 
            // In this case the JHI.dll will be loaded from the $(OutDir) folder (bin\Amulet by default),
            // which is the directory where the executable module for the current process is located.
            // The JHI.dll was placed in the bin\Amulet folder during project build.
            Jhi.DisableDllValidation = true;
#endif
            jhi = Jhi.Instance;

            // This is the path to the Intel Intel(R) DAL Trusted Application .dalp file that was created by the Intel(R) DAL Eclipse plug-in.
            string appletPath = "C:/Users/User/eclipse/java-2019-12/eclipse\\EH_Project_Dal\\bin\\EH_Project_Dal.dalp";
            // Install the Trusted Application
            //Console.WriteLine("Installing the applet.");
            //JHI_SESSION_INFO info;
            //appletID = "5d6664a0-5e65-4016-9e8c-17e8fa75f894";
            // This is the path to the Intel Intel(R) DAL Trusted Application .dalp file that was created by the Intel(R) DAL Eclipse plug-in.
            //appletPath = "C:/Users/User/eclipse/java-2019-12/eclipse\\Targil3\\bin\\Targil3.dalp";

            //uint count;
            //jhi.Uninstall(appletID);
            //jhi.GetSessionInfo(session, out info);
            //jhi.GetSessionsCount(appletID, out count);
            try
            {
                jhi.Install(appletID, appletPath);
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message.ToString());
            }

            // Start a session with the Trusted Application
            byte[] initBuffer = new byte[] { }; // Data to send to the applet onInit function
            // Console.WriteLine("Opening a session.");
            jhi.CreateSession(appletID, JHI_SESSION_FLAGS.None, initBuffer, out session);

            // Send and Receive data to/from the Trusted Application
            byte[] sendBuff = UTF32Encoding.UTF8.GetBytes("Hello"); // A message to send to the TA
            byte[] recvBuff = new byte[2000]; // A buffer to hold the output data from the TA
            int responseCode; // The return value that the TA provides using the IntelApplet.setResponseCode method
            int cmdId = 1; // The ID of the command to be performed by the TA
            // Console.WriteLine("Performing send and receive operation.");
            jhi.SendAndRecv2(session, cmdId, sendBuff, ref recvBuff, out responseCode);
            //Console.Out.WriteLine("Response buffer is " + UTF32Encoding.UTF8.GetString(recvBuff));

            return true;
        }

      
        public bool WriteKeyToTEE(string id, string username)
        {
            string userid = id;
            string sUrl = urlBase + "api/startapp?id=" + userid;


           string query;
            using (WebClient cli = new WebClient())
            {
                cli.Headers[HttpRequestHeader.ContentType] = "application/json";
                query = cli.DownloadString(sUrl);
            }



            int responseCode;
            byte[] symmetricKey = new byte[131];
         
            byte[] publicKey = Convert.FromBase64String(query);
          
 
         



            jhi.SendAndRecv2(session, 3, publicKey, ref symmetricKey, out responseCode);
            symmetricKey = new byte[131];
            jhi.SendAndRecv2(session, 5, publicKey, ref symmetricKey, out responseCode);

            string strsymmetricKey = Convert.ToBase64String(symmetricKey);


            sUrl = urlBase + "api/Symmetric";
            string res = null;
            string json = "{id:\"" + userid + "\", symmetric:\"" + strsymmetricKey + "\"}";
            using (WebClient cli = new WebClient())
            {
                cli.Headers[HttpRequestHeader.ContentType] = "application/json";
                res =cli.UploadString(sUrl, "POST", json);
            }




            return true;
        }


        public KeyValuePair<string,string> CheckIsAuthorized()
        {

            int responseCode;
            string userid = "{c7cdf2ed-cc9e-45c9-b042-9ca240e207a8}";
            string res1 = null;
            string sUrl = urlBase + "api/app?id=" + userid;
            using (WebClient cli = new WebClient())
                {
                    cli.Headers[HttpRequestHeader.ContentType] = "application/json";
                    res1 = cli.DownloadString(sUrl);
                }


         

            byte[] EncData = new byte[1000];
         
            res1 = res1.Substring(1, res1.Length - 2);
            EncData = Convert.FromBase64String(res1);
            byte[] SubEncData = new byte[16];
            byte[] DecData = new byte[548];

         
            jhi.SendAndRecv2(session, 7, EncData, ref DecData, out responseCode);

            string strData=System.Text.Encoding.UTF8.GetString(DecData);
            
           
            string[] stringSeparators = new string[] { "$$$" };
            string[] result =new string[3];
            result = strData.Split(stringSeparators, StringSplitOptions.None);
            DateTime appTime= DateTime.Parse(result[2]); 
            
            if (isTheGoodTime(appTime) == true) {
                KeyValuePair<string, string> userAndPass = new KeyValuePair<string, string>(result[0], result[1]);
                return userAndPass;
            }
            //DateTime appTime = new DateTime();
            
            



            KeyValuePair<string, string> NonGoodTime = new KeyValuePair<string, string>("","");

            return NonGoodTime;




        }

        private string dec(byte[] strToDec, byte[] key)
        {
            byte[] IV = new byte[16];
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.KeySize = 128;
            aes.BlockSize = 128;
            aes.Padding = PaddingMode.Zeros;

            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(key, IV), CryptoStreamMode.Write);
            cs.Write(strToDec, 0, strToDec.Length);
            cs.FlushFinalBlock();

            return Encoding.UTF8.GetString(ms.ToArray());
            

        }

        private bool isTheGoodTime(DateTime appTime)
        {
            appTime=appTime.AddMinutes(5);
            if(appTime>= DateTime.UtcNow)
                return true;
            return false;
        }

        public bool DALCloseSession()
        {
            
            jhi.CloseSession(session);

            
            jhi.Uninstall(appletID);

            
            return true;
        }
    }
}
