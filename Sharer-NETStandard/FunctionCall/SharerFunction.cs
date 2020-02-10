using System.Collections.Generic;

namespace Sharer.FunctionCall
{
    public class SharerFunction
    {
        public string Name;

        public List<SharerFunctionArgument> Arguments = new List<SharerFunctionArgument>();

        public SharerType ReturnType = SharerType.@void;
    }
}
