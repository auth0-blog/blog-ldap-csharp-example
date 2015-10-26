using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDAPClient
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length != 4)
            {
                Console.WriteLine("Usage: LDAPClient <username> <domain> <password> <LDAP server URL>");
                Environment.Exit(1);
            }

            var client = new Client(args[0], args[1], args[2], args[3]);

            try
            {
                //Adding a user
                client.addUser(new UserModel("uid=sampleuser,ou=users,dc=example,dc=com",
                    "sampleuser", "users", "plaintextpass"));
            } catch(Exception e)
            {
                //The user may already exist
                Console.WriteLine(e);
            }

            //Searching for all users
            var searchResult = client.search("ou=users,dc=example,dc=com", "objectClass=*");
            foreach(Dictionary<string, string> d in searchResult)
            {
                Console.WriteLine(String.Join("\r\n", d.Select(x => x.Key + ": " + x.Value).ToArray()));
            }

            //Validating credentials
            if(client.validateUser("sampleuser", "plaintextpass"))
            {
                Console.WriteLine("Valid credentials");
            }
            else
            {
                Console.WriteLine("Invalid credentials");
            }

            //Validating credentials using LDAP bind
            //For this to work the server must be configured to map users correctly to its internal database
            if(client.validateUserByBind("sampleuser", "plaintextpass"))
            {
                Console.WriteLine("Valid credentials (bind)");
            }
            else
            {
                Console.WriteLine("Invalid credentials (bind)");
            }

            //Modifying a user
            client.changeUserUid("sampleuser", "newsampleuser");

            //Deleting a user
            client.delete("uid=newsampleuser,ou=users,dc=example,dc=com");
        }
    }
}
