using System;
using FreezingArcher.Messaging.Interfaces;

namespace FreezingArcher.Messaging
{
    /// <summary>
    /// This could be useful, or it could not be useful ;D
    /// </summary>
    public class GenerateContentMessage : IMessage
    {
        #region IMessage Member

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        public object Source { get; set; }

        /// <summary>
        /// Gets or sets the destination.
        /// </summary>
        /// <value>The destination.</value>
        public object Destination { get; set; }

        /// <summary>
        /// Gets the message identifier.
        /// </summary>
        /// <value>The message identifier.</value>
        public int MessageId
        {
            get
            {
                return (int) FreezingArcher.Messaging.MessageId.GenerateContent;
            }
        }

        /// <summary>
        /// Gets or sets the load method.
        /// </summary>
        /// <value>The load method.</value>
        public Action LoadMethod { get; set; }
        #endregion
    }

    /// <summary>
    /// ICH HASSE DOKU
    /// </summary>
    public struct MessageStruct : IMessage
    {

        #region IMessage Member
        object src, dst;

        /// <summary>
        /// Gets or sets the destination.
        /// </summary>
        /// <value>The destination.</value>
        public object Destination
        {
            get { return dst; }
            set { dst = value; }
        }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        public object Source
        {
            get { return src; }
            set { src = value; }
        }

        /// <summary>
        /// Gets the message identifier.
        /// </summary>
        /// <value>The message identifier.</value>
        public int MessageId
        {
            get { return -1; }
        }

        #endregion
    }
}
