﻿namespace ShufflerPro.Core
{
    public static class NextId
    {
        private static int _id = 0;
        public static int GetNext(this int x)
        {
            return _id++;
        }
    }
}