using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices.Protocols;
using System.Net;

namespace LDAPClient
{
    /// <summary>
    /// A sample LDAP client. For simplicity reasons, this clients only uses synchronous requests.
    /// </summary>
    public class Client
    {
        public Client(string username, string domain, string password, string url)
        {
            var credentials = new NetworkCredential(username, password, domain);
            var serverId = new LdapDirectoryIdentifier(url);

            connection = new LdapConnection(serverId, credentials);            
        }

        /// <summary>
        /// Performs a search in the LDAP server. This method is generic in its return value to show the power
        /// of searches. A less generic search method could be implemented to only search for users, for instance.
        /// </summary>
        /// <param name="baseDn">The distinguished name of the base node at which to start the search</param>
        /// <param name="ldapFilter">An LDAP filter as defined by RFC4515</param>
        /// <returns>A flat list of dictionaries which in turn include attributes and the distinguished name (DN)</returns>
        public List<Dictionary<string, string>> search(string baseDn, string ldapFilter)
        {
            var request = new SearchRequest(baseDn, ldapFilter, SearchScope.Subtree, null);
            var response = (SearchResponse)connection.SendRequest(request);

            var result = new List<Dictionary<string, string>>();

            foreach(SearchResultEntry entry in response.Entries)
            {
                var dic = new Dictionary<string, string>();
                dic["DN"] = entry.DistinguishedName;

                foreach (string attrName in entry.Attributes.AttributeNames)
                {
                    //For simplicity, we ignore multi-value attributes
                    dic[attrName] = string.Join(",", entry.Attributes[attrName].GetValues(typeof(string)));
                }

                result.Add(dic);
            }

            return result;
        }

        /// <summary>
        /// Adds a user to the LDAP server database. This method is intentionally less generic than the search one to
        /// make it easier to add meaningful information to the database.
        /// </summary>
        /// <param name="user">The user to add</param>
        public void addUser(UserModel user)
        {
            var request = new AddRequest(user.DN, new DirectoryAttribute[] {
                new DirectoryAttribute("uid", user.UID),
                new DirectoryAttribute("ou", user.OU),
                new DirectoryAttribute("userPassword", user.UserPassword),
                new DirectoryAttribute("objectClass", new string[] { "top", "account", "simpleSecurityObject" })
            });

            connection.SendRequest(request);
        }

        /// <summary>
        /// This method shows how to modify an attribute.
        /// </summary>
        /// <param name="oldUid">Old user UID</param>
        /// <param name="newUid">New user UID</param>
        public void changeUserUid(string oldUid, string newUid)
        {
            var oldDn = string.Format("uid={0},ou=users,dc=example,dc=com", oldUid);
            var newDn = string.Format("uid={0},ou=users,dc=example,dc=com", newUid);

            DirectoryRequest request = new ModifyDNRequest(oldDn, "ou=users,dc=example,dc=com", "uid=" + newUid);
            connection.SendRequest(request);

            request = new ModifyRequest(newDn, DirectoryAttributeOperation.Replace, "uid", new string[] { newUid });
            connection.SendRequest(request);
        }

        /// <summary>
        /// This method shows how to delete anything by its distinguised name (DN).
        /// </summary>
        /// <param name="dn">Distinguished name of the entry to delete</param>
        public void delete(string dn)
        {
            var request = new DeleteRequest(dn);
            connection.SendRequest(request);
        }

        private LdapConnection connection;
    }
}
