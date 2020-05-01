using Google.Protobuf;
using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeChatSDK
{
    public class Callback
    {
        public string Id { get; private set; }
        public string Arg { get; private set; }
        public CallbackType Type { get; private set; }
        public Action<string,MapField<string,ByteString>> Action { get; private set; }
        public Callback(string id,CallbackType type, Action<string, MapField<string, ByteString>> action,string arg="")
        {
            Id = id;
            Type = type;
            Action = action;
            Arg = arg;
        }
    }
}
