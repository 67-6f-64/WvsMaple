﻿using System;

namespace WvsLogin.Maple
{
    public class NoAccountException : Exception
    {
        public override string Message
        {
            get
            {
                return "The specified account does not exist.";
            }
        }
    }
}
