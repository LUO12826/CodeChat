using CAC.client.Common;
using System.Threading.Tasks;

namespace CAC.client.MessagePage
{
    class MessagePageViewModel : BaseViewModel
    {
        public static bool initialized = false;

        public static ChatListViewModel ChatListViewModel = new ChatListViewModel();

        public static ChatPanelViewModel ChatPanelViewModel = new ChatPanelViewModel();

        public MessagePageViewModel()
        {
            
        }

        public static async void OnNavigateTo()
        {
            if(!initialized) {
                await Task.Delay(1500);
                ChatListViewModel.ReloadChatList();
                initialized = true;
            }
        }

    }
}
