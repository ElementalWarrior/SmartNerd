using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartNerd.Classes
{
    public static class Util
    {
        public static string ProductUrl(int id)
        {
            return "/Images/p/" + id + ".png";
        }
    }
}