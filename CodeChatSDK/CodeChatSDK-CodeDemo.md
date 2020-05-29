# **CodeChatSDK使用**

 

## **初始化工作**

```C#
//获取实例
Client client = Client.Instance;

//配置客户端信息
client.ServerHost = "39.97.250.50:6061";
client.SetHttpApi("http://39.97.250.50:6060", "AQEAAAABAAD_rAp4DJh05a1HAwFT3A6K");

//绑定事件
client.LoginSuccessEvent += LoginSuccess;
client.DisconnectedEvent += Disconnected;
client.LoginFailedEvent += LoginFailed;
client.RegisterFailedEvent += RegisterFailed;
client.AddMessageEvent += AddeMessage;
//其他事件绑定省略
```



## **用户**

### **登陆**

```c#
//创建用户对象
Account account = new Account(username, password);

//设置用户名密码
//适用于用户名或密码验证错误后更改密码
//account.Username = username;
//account.Password = password;

account.initDatabase();
await account.Login();
```



### 注册



### **忘记密码**



### **修改密码**

```c#
//修改密码
await account.ChangePassword(password);
```



### **设置用户名**

```c#
//设置用户名
await account.SetFormattedName(formattedName);
```



### **设置头像**

```c#
//打开文件选取器
FileOpenPicker picker = new FileOpenPicker();
picker.CommitButtonText = "Set";
picker.ViewMode = PickerViewMode.Thumbnail;
picker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
picker.FileTypeFilter.Add("*");

//获取选中文件
var file = await picker.PickSingleFileAsync();
if (file != null)
{
    //获取文件属性
    BasicProperties property = await file.GetBasicPropertiesAsync();
    
    //转换成Bytes数组
    IBuffer buffer = await FileIO.ReadBufferAsync(file);
    byte[] bytes = buffer.ToArray();
    
    //设置头像
    await account.SetAvator(file, property.Size, bytes);
}
```



### **添加标签**

```c#
//添加标签
await account.AddTag(tag);
```



### **移除标签**

```c#
//移除标签
await account.RemoveTag(tag);
```

## **订阅者**

### **获取订阅者列表**



### **添加订阅者**



### **移除订阅者**

## **话题**

### **获取话题列表**

```c#
//获取话题列表
ObservableCollection<Topic> TopicObservableCollection = account.TopicObservableCollection;
```



### **刷新话题列表**

```c#
//刷新话题列表
await account.RefreshTopicObservableCollection();
```



### **获取话题**

```c#
//获取当前话题
//来源于ObservableCollectionView的点击项
Topic currentTopic = itemClickEventArgs.ClickedItem as Topic;

//由话题名寻找话题
Topic currentTopic = account.GetTopicByName(topicName);

//由话题列表索引获取话题
Topic currentTopic = account.GetTopicAt(index);
```



### **添加话题**





### **移除话题**





### **置顶话题**

```c#
//置顶话题
account.PinTopic(currentTopic);
```



### **取消置顶话题**

```c#
//取消置顶话题
account.UnpinTopic(currentTopic);
```



### **设置话题备注**

```c#
//设置话题备注
await currentTopic.SetPrivateComment("Dream it possible");
```

## **消息**

### **获取消息列表**

```c#
//获取消息列表
ObservableCollection<ChatMessage> ChatMessageObservableCollection = currentTopic.MessageObservableCollection;
```



#### **发送普通消息**

```C#
//消息主体
string messsge = "This is a message from code chat.";

//创建发送消息对象
ChatMessage chatMessage = new ChatMessage(){ Text = messsge, IsPlainText = true };

//发送消息
await currentTopic.SendMessage(chatMessage);
```



#### **发送代码**

```c#
//代码类型
CodeType codeType = CodeType.JAVA;
//代码主体
string code = "printf(\"HelloWorld!\n\");";

//运用消息构造器构造消息
ChatMessage message = MessageBuilder.BuildCodeMessage(codeType, code);

//发送消息
await currentTopic.SendMessage(message);
```



#### **发送附件**

```c#
//打开文件选取器
FileOpenPicker picker = new FileOpenPicker();
picker.CommitButtonText = "Send";
picker.ViewMode = PickerViewMode.Thumbnail;
picker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
picker.FileTypeFilter.Add("*");

//获取选中文件
var file = await picker.PickSingleFileAsync();
if (file != null)
{
    //获取文件属性
    BasicProperties property = await file.GetBasicPropertiesAsync();
    
	//转换成Bytes数组
    IBuffer buffer = await FileIO.ReadBufferAsync(file);
    byte[] bytes = buffer.ToArray();
    
    //试上传
    UploadedAttachmentInfo uploadedAttachmentInfo = await client.Upload(file, property.Size, bytes);
    
    //判断上传是否成功
    if (uploadedAttachmentInfo != null)
    {
        //附件消息说明（可为空）
        string optionalMessage = "This is an attachment.";
        
        //运用消息构造器构造消息
        ChatMessage chatMessage = MessageBuilder.BuildAttachmentMessage(uploadedAttachmentInfo, optionalMessage);
        
        //发送消息
	    await currentTopic.SendMessage(chatMessage);
	}
	else
	{
        //创建发送消息对象
	    ChatMessage chatMessage = new ChatMessage() { Text = "Fail to send.", IsPlainText = true };
        
        //发送消息
        await currentTopic.SendMessage(chatMessage);
    }
}
```



### **移除消息**





### **获取历史消息**

```c#
//获取历史消息
await currentTopic.LoadMessage();
```

