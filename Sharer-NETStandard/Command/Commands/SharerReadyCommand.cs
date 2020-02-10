using System;

namespace Sharer.Command
{

    class SharerReadyCommand : SharerSentCommand
    {

        public SharerReadyCommand()
        {
        }

        public override SharerCommandID CommandID
        {
            get
            {
                return SharerCommandID.Ready;
            }
        }

        internal override byte[] ArgumentsToSend()
        {
            return null;
        }

       
        internal override bool DecodeArgument(byte b)
        {
            return true;
        }
    }
}
