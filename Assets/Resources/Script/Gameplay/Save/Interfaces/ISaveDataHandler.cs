namespace FlameOfTheLost
{
    public interface ISaveDataHandler<TSaveData> where TSaveData : class
    {
        public TSaveData CaptureSaveData();

        public void RestoreSaveData(TSaveData saveData);
    }
}