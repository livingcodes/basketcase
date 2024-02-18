namespace Basketcase;
public class ReaderToDataTable : IReaderConverter<DataTable>
{
  public DataTable Convert(IDataReader rdr) {
    var tbl = new DataTable();
    tbl.Load(rdr);
    return tbl;
  }
}