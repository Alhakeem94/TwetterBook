using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwetterBook.Contracts
{
    public class ApiRoutes
    {
        public const string Root = "api";
        public const string version = "v1";
        public const string Base = Root + "/"+ version;

        public static class posts
        {
            public const string GetAll = Base + "/posts";
            public const string Update = Base + "/posts";
            public const string Delete = Base + "/posts";
            public const string Get    = Base + "/posts/{postId}";
            public const string Create = Base + "/posts";


        }


        public static class Identity 
        {
            public const string Login = Base + "/Login";

            public const string Register = Base + "/Register";

        }


    }
}
