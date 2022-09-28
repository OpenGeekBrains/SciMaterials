using System;
using System.Collections.Generic;

namespace SciMaterials.DAL.Models
{
    public partial class User : NamedModel
    {
        public string Email { get; set; } = string.Empty;

        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<File> Files { get; set; }

        public User()
        {
            Comments = new HashSet<Comment>();
            Files = new HashSet<File>();
        }
    }
}
