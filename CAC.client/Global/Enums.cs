
namespace CAC.client
{
    enum ChatType
    {
        oneOnOneChat = 0,
        groupChat = 1
    }

    enum NotificationType
    {
        normal = 0,
        mute = 1, 
        blocked = 2
    }

    enum MessageType
    {
        text = 0,
        image = 1,
        code = 2,
        file = 3
    }

    enum NaviItems
    {
        chat, 
        contact,
        settings
    }
}
