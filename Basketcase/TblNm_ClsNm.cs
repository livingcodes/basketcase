namespace Basketcase
{
    /// <summary>Table name equals type name</summary>
    public class TblNm_ClsNm : ITblNm
    {
        public string Get<T>() => typeof(T).Name;
        public string Get(object instance) => instance.GetType().Name;
    }
}
