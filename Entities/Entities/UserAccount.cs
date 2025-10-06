using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Entities
{
    public class UserAccount
    {
        public DateTime? AccountExpires { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string SamAccountName { get; set; } = string.Empty;
        public DateTime? LastBadPasswordAttempt { get; set; }
        public string Division { get; set; }
        public int UserAccountControl { get; set; }
        public string UserAccountControlLegend { get; set; }

        public string EmployeeId { get; set; }

        public DateTime? WhenCreated { get; set; }
        public DateTime? WhenChanged { get; set; }
        public DateTime? LastPasswordSet { get; set; }
        public DateTime? LastLogon { get; set; }
        public DateTime? LastLogonTimestamp { get; set; }
        public double? TimeToPasswordExpiry { get; set; }
        public int BadPwdCount { get; set; }
        public DateTime? LockoutTime { get; set; }
    }
    public class BasicUserData
    {
        public string DisplayName { get; set; }
        public string UserName { get; set; }
    }

}
