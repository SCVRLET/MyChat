using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyChat.Models
{
    public class Chat
    {
        public Chat()
        {
            Messages = new List<Message>();
            Users = new List<ChatUser>();
        }

        public int Id { get; set; }
        public string Name { get; set; } 
        public ChatType Type { get; set; }
        public ICollection<Message> Messages { get; set; }
        public ICollection<ChatUser> Users { get; set; }

        public int JoinId { get; set; }


/*        public List<string> FindAdmins() { 

            var gav = Users.Where(x => x.Role == UserRole.Admin);
            List<ChatUser> ret = new List<ChatUser>();

            foreach (var i in Users){
                var a = i.Role;
                if (a == UserRole.Admin)
                {
                    ret.Add(i);
                }
            }
            return ret.Select(x => x.User.NormalizedUserName).ToList(); 
        }*/
            

    }
}
