using VirtualHerbarium.AdminPanel.Models;

namespace VirtualHerbarium.AdminPanel.ViewModels
{
    public class UserFriendsViewModel
    {
        public UserFriendsResponse Data { get; }

        public UserFriendsViewModel(UserFriendsResponse data)
        {
            Data = data;
        }

        public object Accepted => Data.accepted;
        public object Incoming => Data.incoming;
        public object Sent => Data.sent;
    }
}
