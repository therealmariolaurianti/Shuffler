namespace ShufflerPro.Upgraded
{
    public static class NextId
    {
        private static int _id;

        public static int GetNext()
        {
            return _id++;
        }
    }
}