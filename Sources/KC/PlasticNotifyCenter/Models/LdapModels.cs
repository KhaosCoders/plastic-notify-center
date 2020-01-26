using System.Collections.Generic;

namespace PlasticNotifyCenter.Models
{
    public class LdapUser
    {
        public string UserName { get; set; }
        public string Email { get; set; }
    }

    public class LdapGroup
    {
        public string Name { get; set; }
        public IList<string> Users { get; set; } = new List<string>();
    }
}