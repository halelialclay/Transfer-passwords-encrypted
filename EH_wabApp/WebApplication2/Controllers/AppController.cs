using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Caching;
using System.Web.Http;

namespace WebApplication2.Controllers
{
    public class AppController : ApiController
    {
        private Dictionary<string, string> users
        {
            get
            {
                if (System.Web.HttpContext.Current.Cache["users"] == null)
                    System.Web.HttpContext.Current.Cache["users"] = new Dictionary<string, string>();

                
                return (Dictionary<string, string>)System.Web.HttpContext.Current.Cache["users"];
            }
        }


        // GET: api/App/5
        public string Get(string id)
        {
            AppData returnData = new AppData();
            returnData.id = id;
            returnData.encdata = (string)System.Web.HttpContext.Current.Cache[id];
            return returnData.encdata;
        }

        // POST: api/App
        public bool Post([FromBody]AppData value)
        {
            if (System.Web.HttpContext.Current.Cache[value.id] == null)
                System.Web.HttpContext.Current.Cache.Insert(value.id, value.encdata, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, 5, 0));
            else
                System.Web.HttpContext.Current.Cache[value.id]=value.encdata;


            return true;
        }
    }


    public class AppData
    {
        public string id { get; set; }
        public string encdata { get; set; }
    }
}
