namespace Basketcase
{
    public class ReaderToDataTable : IReaderConverter<DataTable>
    {
        public DataTable Convert(IDataReader reader) {
            var table = new DataTable();
            table.Load(reader);
            return table;
        }
    }
}
