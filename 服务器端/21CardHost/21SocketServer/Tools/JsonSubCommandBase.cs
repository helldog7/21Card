﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase;
using Newtonsoft.Json;

namespace _21SocketServer
{
    public abstract class JsonSubCommandBase<TWebSocketSession, TJsonCommandInfo> : StringCommandBase<TWebSocketSession>
     where TWebSocketSession : AppSession<TWebSocketSession>, new()
    {
        private const string m_QueryTemplateA = "{0}-{1} {2}";
        private const string m_QueryTemplateB = "{0} {1}";

        private bool m_IsPrimitiveType = false;

        private Type m_CommandInfoType;

        public JsonSubCommandBase()
        {
            m_CommandInfoType = typeof(TJsonCommandInfo);

            if (m_CommandInfoType.IsPrimitive)
                m_IsPrimitiveType = true;
        }

        public override void ExecuteCommand(TWebSocketSession session, StringCommandInfo commandInfo)
        {
            if (string.IsNullOrEmpty(commandInfo.Data))
            {
                ExecuteJsonCommand(session, default(TJsonCommandInfo));
                return;
            }

            TJsonCommandInfo jsonCommandInfo;

            //if (!string.IsNullOrEmpty(commandInfo.Token))
            //    session.CurrentToken = commandInfo.Token;

            if (!m_IsPrimitiveType)
                jsonCommandInfo = JsonConvert.DeserializeObject<TJsonCommandInfo>(commandInfo.Data);
            else
                jsonCommandInfo = (TJsonCommandInfo)Convert.ChangeType(commandInfo.Data, m_CommandInfoType);

            ExecuteJsonCommand(session, jsonCommandInfo);
        }

        protected abstract void ExecuteJsonCommand(TWebSocketSession session, TJsonCommandInfo commandInfo);

        protected string SerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        protected string GetJsonResponse(string token, object content)
        {
            return GetJsonResponse(Name, token, content);
        }

        protected string GetJsonResponse(string name, string token, object content)
        {
            string strOutput;

            //Needn't serialize primitive type object
            if (content.GetType().IsPrimitive)
                strOutput = content.ToString();
            else
                strOutput = SerializeObject(content);

            if (string.IsNullOrEmpty(token))
                return string.Format(m_QueryTemplateB, name, strOutput);
            else
                return string.Format(m_QueryTemplateA, name, token, strOutput);
        }
    }
}
