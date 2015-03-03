using System;
using FreezingArcher.Messaging.Interfaces;

namespace FreezingArcher.Messaging
{
    public class GenerateContentMessage : IMessage
    {
        #region IMessage Member

        public object Source { get; set; }

        public object Destination { get; set; }

        public int MessageId
        {
            get
            {
                return (int) FreezingArcher.Messaging.MessageId.GenerateContent;
            }
        }

        public Action LoadMethod { get; set; }
        #endregion
    }

    public struct MessageStruct : IMessage
    {

        #region IMessage Member
        object src, dst;

        public object Destination
        {
            get { return dst; }
            set { dst = value; }
        }

        public object Source
        {
            get { return src; }
            set { src = value; }
        }

        public int MessageId
        {
            get { return -1; }
        }

        #endregion
    }
}
