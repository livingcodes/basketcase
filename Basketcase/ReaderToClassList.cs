using System.Reflection;

namespace Basketcase
{  
   public class ReaderToClassList<T> : IReaderConverter<List<T>>
   {
      public List<T> Convert(IDataReader reader) {
         var list = new List<T>();
         var fields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public);
         var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
         var columns = new GetColumns().From(reader);
         while (reader.Read()) {
            var item = System.Activator.CreateInstance<T>();
            foreach (var property in properties)
               if (columns.Contains(property.Name)) {
                  var value = reader[property.Name];
                  if (value != System.DBNull.Value)
                     property.SetValue(item, value);
               }

            foreach (var field in fields)
               if (columns.Contains(field.Name))
                  if (reader[field.Name] != System.DBNull.Value)
                     field.SetValue(item, reader[field.Name]);
            list.Add(item);
         }
         return list;
      }
   }
}