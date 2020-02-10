﻿using Sharer.FunctionCall;
using System.Collections.Generic;

namespace Sharer.Command
{
    public enum SharerCallFunctionStatus : byte
    {
        UnknownStatus = 0xff,
        OK = 0,
        FunctionIdOutOfRange,
        UnknownType,
    }

    public class SharerFunctionReturn<ReturnType>
    {
        public SharerCallFunctionStatus Status = SharerCallFunctionStatus.UnknownStatus;
        public SharerType Type;
        public ReturnType Value;

        public override string ToString()
        {
            if (Status == SharerCallFunctionStatus.OK)
            {
                if (Value == null) return "OK";
                else return Value.ToString();
            }
            else
            {
                return Status.ToString();
            }
        }
    }



    class SharerCallFunctionCommand<ReturnType> : SharerSentCommand
    {
        private byte[] _buffer;
        SharerType _returnType;

        public SharerCallFunctionCommand(byte[] buffer, SharerType returnType)
        {
            _buffer = buffer;
            _returnType = returnType;
            Return.Type = returnType;
        }

        public override SharerCommandID CommandID
        {
            get
            {
                return SharerCommandID.CallFunction;
            }
        }

        internal override byte[] ArgumentsToSend()
        {
            return _buffer;
        }

        private enum Steps
        {
            Status,
            ReturnValue,
            End
        }

        // reception step
        private Steps _receivedStep = Steps.Status;

        public SharerFunctionReturn<ReturnType> Return = new SharerFunctionReturn<ReturnType>();

        private List<byte> _returnBytes = new List<byte>();

        private int _returnSize;

        internal override bool DecodeArgument(byte b)
        {
            switch (_receivedStep)
            {
                case Steps.Status: // receive status
                    Return.Status = (SharerCallFunctionStatus)b;

                    // if the function has a returned value
                    if(Return.Status == SharerCallFunctionStatus.OK && _returnType != SharerType.@void)
                    {
                        _returnBytes.Clear();
                        _returnSize = SharerTypeHelper.Sizeof(_returnType);
                        _receivedStep = Steps.ReturnValue;
                    }
                    else
                    {
                        _receivedStep = Steps.End;
                    }

                    break;
                case Steps.ReturnValue:
                    _returnBytes.Add(b); // add received byte

                    // if enought returned byte, decode it and end
                    if(_returnBytes.Count>= _returnSize)
                    {
                        Return.Value = (ReturnType)SharerTypeHelper.Decode(_returnType, _returnBytes.ToArray());

                        _receivedStep = Steps.End;
                    }
                    break;
            }

            return _receivedStep == Steps.End;
        }
    }
}
