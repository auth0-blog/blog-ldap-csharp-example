using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDAPClient
{
    public class UserModel
    {
        public UserModel(string dn, string uid, string ou, string userPassword)
        {
            this.dn = dn;
            this.uid = uid;
            this.ou = ou;
            this.userPassword = userPassword;
        }

        public string DN { get { return dn; } }
        public string UID { get { return uid; } }
        public string OU { get { return ou; } }
        public string UserPassword { get { return userPassword; } }

        private string dn;
        private string uid;
        private string ou;
        private string userPassword;
    }
}
