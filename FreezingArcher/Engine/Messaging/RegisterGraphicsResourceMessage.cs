//using FreezingArcher.Base.Renderer.Interfaces;FIXME
using FreezingArcher.Messaging.Interfaces;

namespace FreezingArcher.Messaging
{
    public class RegisterGraphicsResourceMessage : IMessage
    {
        #region IMessage Member

        public object Source { get; set; }

        public object Destination { get; set; }

        public int MessageId
        {
            get { return 2; }//really? FIXME
        }

        //public IGraphicsResource Resource { get; set; }FIXME
        #endregion
    }
}
